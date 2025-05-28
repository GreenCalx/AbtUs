using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PLAYER_ACTIONS
{
    NONE = 0, MOVE = 1, INFO = 2, TALK = 3, PUZZLE = 4, DUPLICATE = 5, SHATTER = 6,
    WHEEL2 = 7, WHEEL3 = 8, WHEEL4 = 9, DEFAULT = 10,
};

[Serializable]
public class InteractibleObject : MonoBehaviour
{
    [Header("Tweaks")]
    public ItemSO def;
    public Transform targetedTransfrom;
    public Renderer targetRend;
    public LayerMask intersectionCheckMask;
    [Header("Optional References")]
    public Puzzle puzzle;
    public SelectionFX selectionFX;

    [Header("Internals")]
    private PLAYER_ACTIONS selectedAction;
    private UnityEvent startAction;
    private UnityEvent continueAction;
    private UnityEvent cancelAction;
    private PlayerController player;
    private float distFromPlayer = 0f;
    [Header("Move action refs")]
    public Rigidbody RB;
    public Collider mainCollider;
    protected Coroutine ActionCo;
    public bool IsInActionChain = false;
    private bool isMovedByPlayer = false;
    private bool isValidOperation = true;
    
    
    void Start()
    {
        if (RB == null)
        { RB = GetComponent<Rigidbody>(); }

        ResetMultiAction();

        if (targetedTransfrom == null)
        { targetedTransfrom = transform; }
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
        ResetMultiAction();
        //UIGame.Instance.UpdateCursorFromPlayerAction(selectedAction);
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
            case  PLAYER_ACTIONS.INFO:
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
                selectedAction = iAction;

                IsInActionChain = false;

                startAction = new UnityEvent();
                startAction.AddListener(SolvePuzzle);

                continueAction = new UnityEvent();

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
                cancelAction.AddListener(StopDuplicate);
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

            if (IsInActionChain)
                iPlayer.freeze_inputs = true;
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
    }

    public IEnumerator MoveCo(Transform iTarget, Rigidbody iTargetRB)
    {
        if (iTargetRB != null)
        {
            iTargetRB.isKinematic = true;
            iTargetRB.useGravity = false;
            mainCollider.enabled = false;
        }
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
            iTargetRB.isKinematic = false;
            iTargetRB.useGravity = true;
            mainCollider.enabled = true;
        }
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
        List<Collider> cols = Physics.OverlapBox(b.center, b.extents / 2f, Quaternion.identity, intersectionCheckMask,QueryTriggerInteraction.Ignore).ToList();
        int n = cols.Where(e => e.gameObject != gameObject).ToArray().Length;
        return (n == 0);

    }
    #endregion

    #region INFO
    public void ShowInfo()
    {
        UIInfoPanel panel = UIGame.Instance.infoPanel;
        panel.title.text = def.name;
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
        while (player.playerInAction)
        {
            // 
            yield return null;
        }
    }

    #endregion

    #region DUPLICATE
    public virtual void Duplicate()
    {
        if (RB != null)
        {
            RB.useGravity = false;
            RB.isKinematic = true;
        }
        GameObject duplicata = GameObject.Instantiate(gameObject);
        duplicata.transform.parent = transform.parent;

        Rigidbody rb = duplicata.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = duplicata.GetComponent<InteractibleObject>()?.RB;
        }

        isMovedByPlayer = true;

        if (selectionFX != null)
        {
            InteractibleObject as_obj = duplicata.GetComponent<InteractibleObject>();
            as_obj.selectionFX.Init();

            selectionFX.Deselect();
            as_obj.selectionFX.Select();
        }
        

        // Clamp pos to center of screen
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        ActionCo = StartCoroutine(MoveCo(duplicata.transform, rb));
    }

    public virtual void StopDuplicate()
    {
        isMovedByPlayer = false;
        player.playerInAction = false;
        if (RB != null)
        {
            RB.isKinematic = false;
            RB.useGravity = true;
        }
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

        Destroy(gameObject);
    }

    public virtual void StopShatter()
    {
        PostAction();
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

}
