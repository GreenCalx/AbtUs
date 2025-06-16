using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EventLog;
public enum PLAYER_ACTIONS
{
    NONE = 0, MOVE = 1, INFO = 2, TALK = 3, PUZZLE = 4, DUPLICATE = 5, SHATTER = 6,
    WHEEL2 = 7, WHEEL3 = 8, WHEEL4 = 9, DEFAULT = 10,
};



[Serializable]
public class InteractibleObject : MonoBehaviour, IPoolable
{
    [Header("Tweaks")]
    public ItemSO def;
    public Transform targetedTransfrom;
    public Renderer targetRend;
    public LayerMask intersectionCheckMask;
    public GameFeedback feedback;
    public OBJ_NATURE nature;

    [Header("Optional References")]
    public Puzzle puzzle;
    public SelectionFX selectionFX;
    public GFXWrapper shaderCom;

    [Header("Internals")]
    private bool initDone = false;
    private PLAYER_ACTIONS selectedAction;
    private UnityEvent startAction;
    private UnityEvent continueAction;
    private UnityEvent cancelAction;
    private List<UnityEvent<InteractibleObject,bool>> moveActionListeners;
    private List<UnityEvent<InteractibleObject>> shatterActionListeners;
    protected PlayerController player;
    private float distFromPlayer = 0f;
    [Header("Move action refs")]
    public Rigidbody RB;
    public Collider mainCollider;
    protected Coroutine ActionCo;
    public bool IsInActionChain = false;
    public bool isMovedByPlayer
    {
        get; protected set;
    }
    private bool isValidOperation = true;
    [Header("Duplicate Refs")]
    public bool isDuplicata = false;


    void Start()
    {
        if (!initDone)
            Init();
        Managers.Instance.ObjectPools.AddObject(this);
    }

    void OnDestroy()
    {
        Managers.Instance.ObjectPools.RemoveObject(this);
    }

    public void Init()
    {
        moveActionListeners = new List<UnityEvent<InteractibleObject, bool>>();
        shatterActionListeners = new List<UnityEvent<InteractibleObject>>();

        if (!isDuplicata)
            Managers.Instance.ObjectChains.CreateChain(this);

        if (RB == null)
        { RB = GetComponent<Rigidbody>(); }

        ResetMultiAction();

        if (targetedTransfrom == null)
        { targetedTransfrom = transform; }

        if (shaderCom != null)
        {
            shaderCom.InitShader();
        }

        initDone = true;
    }

    public virtual void Select()
    {
        if (selectionFX != null)
            selectionFX.Select();
    }

    public virtual void Deselect()
    {
        if (selectionFX != null)
            selectionFX.Deselect();
    }

    public void ResetMultiAction()
    {
        if (def.availableActions.Length == 4)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL4);
            IsInActionChain = true;
        }
        else if (def.availableActions.Length == 3)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL3);
            IsInActionChain = true;
        }
        else if (def.availableActions.Length == 2)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL2);
            IsInActionChain = true;
        }
        else if (def.availableActions.Length == 1)
        {
            ChangeSelectedAction(def.availableActions[0]);
            IsInActionChain = false;
        }
    }

    private void PostAction()
    {
        player.playerInAction = false;
        player.targetedInteractibleObject = null;
        player = null;
        Deselect();
        ResetMultiAction();
        //UIGame.Instance.UpdateCursorFromPlayerAction(selectedAction);
    }

    bool IsPlayerTarget()
    {
        return player.targetedInteractibleObject.gameObject == gameObject;
    }

    public PLAYER_ACTIONS GetSelectedAction() { return selectedAction; }

    public void ChangeSelectedAction(PLAYER_ACTIONS iAction)
    {
        if (iAction == selectedAction)
            return;

        switch (iAction)
        {
            case PLAYER_ACTIONS.MOVE:
                selectedAction = iAction;

                startAction = new UnityEvent();
                startAction.AddListener(Move);

                continueAction = new UnityEvent();
                continueAction.AddListener(StopMove);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(StopMove);

                IsInActionChain = false;
                break;
            case PLAYER_ACTIONS.INFO:
                selectedAction = iAction;

                startAction = new UnityEvent();
                startAction.AddListener(ShowInfo);

                continueAction = new UnityEvent();
                continueAction.AddListener(HideInfo);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(HideInfo);

                IsInActionChain = false;
                break;
            case PLAYER_ACTIONS.PUZZLE:
                if (puzzle.puzzleSolved)
                    break;

                selectedAction = iAction;

                IsInActionChain = false;

                startAction = new UnityEvent();
                startAction.AddListener(SolvePuzzle);

                continueAction = new UnityEvent();
                continueAction.AddListener(TryValidatePuzzle);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(StopPuzzle);
                break;
            case PLAYER_ACTIONS.DUPLICATE:
                selectedAction = iAction;
                IsInActionChain = false;

                startAction = new UnityEvent();
                startAction.AddListener(Duplicate);

                continueAction = new UnityEvent();
                continueAction.AddListener(StopDuplicate);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(CancelDuplicate);
                break;
            case PLAYER_ACTIONS.SHATTER:
                selectedAction = iAction;
                IsInActionChain = false;

                startAction = new UnityEvent();
                startAction.AddListener(StartShatter);

                //continueAction = new UnityEvent();
                //continueAction.AddListener(StopShatter);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(StopShatter);

                break;
            case PLAYER_ACTIONS.WHEEL2:
            case PLAYER_ACTIONS.WHEEL3:
            case PLAYER_ACTIONS.WHEEL4:
                selectedAction = iAction;
                IsInActionChain = true;

                startAction = new UnityEvent();
                startAction.AddListener(ActionWheel);

                continueAction = new UnityEvent();
                continueAction.AddListener(ExecSelectionActionInWheel);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(ExitActionWheelMode);

                break;
            default:
                // selected action remains unchanged and thus cancel is called.
                break;
        }
    }

    public void OnInteract(PlayerController iPlayer)
    {
        if (startAction != null)
        {
            player = iPlayer;
            distFromPlayer = Vector3.Distance(transform.position, iPlayer.transform.position);
            distFromPlayer = Mathf.Clamp(distFromPlayer, 0.1f, iPlayer.actionDistance);

            startAction.Invoke();

            iPlayer.freeze_inputs = IsInActionChain && IsPlayerTarget();
        }
    }

    public bool OnContinueInteract(PlayerController iPlayer)
    {
        if (iPlayer != player)
            return false;

        if (!isValidOperation)
        {
            return false;
        }

        continueAction.Invoke();

        if (!IsInActionChain)
        {
            iPlayer.freeze_inputs = false;
            return false;
        }
        return true;
    }

    public void OnCancelInteract(PlayerController iPlayer)
    {
        if (iPlayer != player)
            return;

        iPlayer.freeze_inputs = false;

        cancelAction.Invoke();

        PostAction();
    }

    public bool IsInAction()
    {
        return (ActionCo != null) && !IsInActionChain;
    }

    #region MOVE
    public virtual void Move()
    {
        isMovedByPlayer = true;
        IsInActionChain = false;

        // Clamp pos to center of screen
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        ActionCo = StartCoroutine(MoveCo(targetedTransfrom, RB));
    }

    public virtual void StopMove()
    {
        isMovedByPlayer = false;


        Managers.Instance.ObjectChains.RefreshFeedback();
        INFO("InteractibleObject " + gameObject.name + " StopMove");
    }

    public IEnumerator MoveCo(Transform iTarget, Rigidbody iTargetRB)
    {
        // if interactible is already kinematic, dont change it after the moveCo.
        bool makeKinematic = !iTargetRB.isKinematic;

        if (iTargetRB != null)
        {
            if (makeKinematic)
                iTargetRB.isKinematic = true;
            iTargetRB.useGravity = false;
            mainCollider.enabled = false;
        }
        
        foreach (var l in moveActionListeners)
        { l.Invoke(this, true); }

        if (selectionFX != null)
            selectionFX.intersectionOperationCheck = true;
        isValidOperation = true;

        while (isMovedByPlayer)
        {
            Vector3 worldPos = player.FPSCamera.GetRayFromScreenCenter().GetPoint(distFromPlayer);
            iTarget.position = worldPos;
            isValidOperation = ValidateMoveOp();
            if (selectionFX != null)
                selectionFX.operationIsValid = isValidOperation;
            yield return null;
        }

        if (selectionFX != null)
            selectionFX.intersectionOperationCheck = false;
        isValidOperation = true;

        if (iTargetRB != null)
        {
            if (makeKinematic)
                iTargetRB.isKinematic = false;
            iTargetRB.useGravity = true;
            mainCollider.enabled = true;
        }

        foreach (var l in moveActionListeners)
        { l.Invoke(this, false); }

        PostAction();
    }

    bool ValidateMoveOp()
    {
        // is under map?
        float height = Terrain.activeTerrain.SampleHeight(targetedTransfrom.position);
        if (height > targetedTransfrom.position.y)
            return false;

        // renderer intersects other models flagged in layermask
        Bounds b = targetRend.bounds;
        List<Collider> cols = Physics.OverlapBox(b.center, b.extents / 2f, Quaternion.identity, intersectionCheckMask, QueryTriggerInteraction.Ignore).ToList();
        int n = cols.Where(e => e.gameObject != gameObject).ToArray().Length;
        return (n == 0);

    }

    public void AddMoveListener(UnityEvent<InteractibleObject, bool> iListener)
    {
        if (!moveActionListeners.Contains(iListener))
            moveActionListeners.Add(iListener);
    }
    public void RemoveMoveListener(UnityEvent<InteractibleObject,bool> iListener)
    {
        if (moveActionListeners.Contains(iListener))
        {
            moveActionListeners.Remove(iListener);
            //moveActionListeners = moveActionListeners.Where(e => e != null);
        }
    }
    #endregion

    #region INFO
    public void ShowInfo()
    {
        UIInfoPanel panel = UIGame.Instance.infoPanel;
        panel.title.text = def.itemName;
        panel.body.text = def.info;
        UIGame.Instance.infoPanel.gameObject.SetActive(true);
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        ActionCo = StartCoroutine(InfoCo());
        UIGame.Instance.UpdateCursorFromPlayerAction(PLAYER_ACTIONS.INFO);
    }

    public void HideInfo()
    {
        UIGame.Instance.infoPanel.gameObject.SetActive(false);
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        PostAction();
    }

    public IEnumerator InfoCo()
    {
        float initTime = Time.time;
        while ((Time.time - initTime) < def.showInfoDuration)
        {
            yield return null;
        }
        HideInfo();
    }
    #endregion

    #region PUZZLE
    public void SolvePuzzle()
    {
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        puzzle.StartPuzzle(player);
        ActionCo = StartCoroutine(SolvePuzzleCo());
    }

    public void TryValidatePuzzle()
    {
        if (puzzle.TryValidatePuzzle())
        {
            puzzle.OnPuzzleSolved();
            PostAction();
        }
    }

    public void StopPuzzle()
    {
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        puzzle.StopPuzzle();
        PostAction();
    }

    public IEnumerator SolvePuzzleCo()
    {
        while (puzzle.playerInPuzzle)
        {
            // 
            yield return null;
        }
    }

    #endregion

    #region DUPLICATE
    public virtual void Duplicate()
    {
        // bool makeKinematic = false;

        // if (RB != null)
        // {
        //     makeKinematic = !RB.isKinematic;
        //     if (makeKinematic)
        //         RB.isKinematic = true;
        //     RB.useGravity = false;
        // }
        Managers.Instance.ObjectChains.ClearChainState(this);

        GameObject duplicata = GameObject.Instantiate(gameObject);
        duplicata.transform.parent = transform.parent;

        InteractibleObject as_obj = duplicata.GetComponent<InteractibleObject>();
        as_obj.isDuplicata = true;
        as_obj.Init();
        Managers.Instance.ObjectChains.AddToChain(this, as_obj);

        if (selectionFX != null)
        {
            selectionFX.Deselect();
        }
        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        SwapTo(as_obj, PLAYER_ACTIONS.MOVE);
    }

    public virtual void StopDuplicate()
    {

    }

    public virtual void CancelDuplicate()
    {
        // if (duplicates.Count > 0)
        // {
        //     Destroy(duplicates[duplicates.Count - 1].gameObject);
        // }
        Managers.Instance.ObjectChains.DeleteLastFromChain(this);
        player.playerInAction = false;
    }

    public void ForceInteract(PLAYER_ACTIONS iAction)
    {
        player.targetedInteractibleObject = this;
        player.playerInAction = true;

        if (selectionFX != null)
        {
            selectionFX.Init();
            selectionFX.Select();
        }

        ChangeSelectedAction(iAction);
        OnInteract(player);
    }
    public void SwapTo(InteractibleObject iOtherObj, PLAYER_ACTIONS iAction)
    {
        iOtherObj.player = player;
        iOtherObj.ForceInteract(iAction);
        ResetMultiAction();
    }
    #endregion

    #region SHATTER

    public virtual void StartShatter()
    {
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }

        player.playerInAction = false;
        player.targetedInteractibleObject = null;

        Managers.Instance.ObjectChains.DestroyChain(this);
        Deselect();
    }

    public virtual void StopShatter()
    {
        PostAction();
    }

    public virtual bool ShatterAnim()
    {
        if (shaderCom == null)
            return false;
        StartCoroutine(ShatterCo());
        return true;
    }

    IEnumerator ShatterCo()
    {
        foreach (var l in shatterActionListeners)
        { l.Invoke(this); }

        float elapsed = 0f;
        while (elapsed <= def.shatterTime)
        {
            float shatVal = elapsed / def.shatterTime;
            shaderCom.SetShatter(shatVal);
            elapsed += Time.deltaTime;

            yield return null;
        }
        Kill();
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }
    public void AddShatterListener(UnityEvent<InteractibleObject> iListener)
    {
        if (!shatterActionListeners.Contains(iListener))
            shatterActionListeners.Add(iListener);
    }
    public void RemoveShatterListener(UnityEvent<InteractibleObject> iListener)
    {
        if (shatterActionListeners.Contains(iListener))
        {
            shatterActionListeners.Remove(iListener);
        }
    }
    #endregion

    #region WHEEL


    public virtual void ActionWheel()
    {
        UIGame.Instance.EnterActionWheelMode(def.availableActions);
    }
    public virtual void ExecSelectionActionInWheel()
    {
        PLAYER_ACTIONS act = UIGame.Instance.GetSelectedAction();
        ExitActionWheelMode();
        ChangeSelectedAction(act);

        if (selectedAction == act)
            OnInteract(player);
        else
            OnCancelInteract(player);
    }

    public virtual void ExitActionWheelMode()
    {
        UIGame.Instance.ExitActionWheelMode();
    }
    #endregion

    #region IPoolable
    public string GetName() { return gameObject.name; }
    public OBJ_NATURE GetNature() { return nature; }
    public void OnPoolSleep() { }

    public void OnPoolAwake() { }

    public bool UseInFeedback() { return true; }

    public virtual Transform GetTransform() { return transform; }
    #endregion

}
