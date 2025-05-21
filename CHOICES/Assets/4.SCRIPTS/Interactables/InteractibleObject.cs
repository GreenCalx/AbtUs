using System;
using System.Collections;
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
    public PLAYER_ACTIONS[] availableActions;
    public Transform targetedTransfrom;
    [Header("Optional References")]
    public Puzzle puzzle;

    [Header("Internals")]
    private PLAYER_ACTIONS selectedAction;
    private UnityEvent startAction;
    private UnityEvent continueAction;
    private UnityEvent cancelAction;
    private PlayerController player;
    private float distFromPlayer = 0f;
    public Rigidbody RB;
    protected Coroutine ActionCo;
    public bool IsInActionChain = false;

    private bool isMovedByPlayer = false;
    
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
        if (availableActions.Length == 4)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL4);
            IsInActionChain = true;
        }
        else if (availableActions.Length == 3)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL3);
            IsInActionChain = true;
        }
        else if (availableActions.Length == 2)
        {
            ChangeSelectedAction(PLAYER_ACTIONS.WHEEL2);
            IsInActionChain = true;
        }
        else if (availableActions.Length == 1)
        {
            ChangeSelectedAction(availableActions[0]);
            IsInActionChain = false;
        }
    }

    private void PostAction()
    {
        ResetMultiAction();
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
                break;
            case PLAYER_ACTIONS.SHATTER:
                selectedAction = iAction;
                IsInActionChain = false;
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
        if (RB != null)
        {
            RB.isKinematic = true;
            RB.useGravity = false;
        }

        // Clamp pos to center of screen
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        ActionCo = StartCoroutine(MoveCo());
    }

    public virtual void StopMove()
    {
        isMovedByPlayer = false;
        if (RB != null)
        {
            RB.isKinematic = false;
            RB.useGravity = true;
        }

        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
            PostAction();
        }

        UIGame.Instance.UpdateCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
    }

    public IEnumerator MoveCo()
    {
        while (isMovedByPlayer)
        {
            Vector3 worldPos = player.FPSCamera.GetRayFromScreenCenter().GetPoint(distFromPlayer);
            targetedTransfrom.position = worldPos;
            yield return null;
        }
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
        GameObject duplicata = GameObject.Instantiate(gameObject);
        duplicata.transform.parent = transform.parent;

    }
    #endregion

    #region WHEEL


    public virtual void ActionWheel()
    {
        UIGame.Instance.EnterActionWheelMode(availableActions);
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
