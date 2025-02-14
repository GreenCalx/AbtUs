using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum PLAYER_ACTIONS { NONE =0, MOVE =1, INFO =2, TALK =3, PUZZLE=4}

[Serializable]
public class InteractibleObject : MonoBehaviour
{
    [Header("Tweaks")]
    public PLAYER_ACTIONS[] availableActions;
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

    void Start()
    {
        if (RB==null)
        { RB = GetComponent<Rigidbody>(); }

        if (availableActions.Length >= 1)
        {
            ChangeSelectedAction(availableActions[0]);
        }
    }
    
    public PLAYER_ACTIONS GetSelectedAction() { return selectedAction; }

    public void ChangeSelectedAction(PLAYER_ACTIONS iAction)
    {
        if (iAction==selectedAction)
            return;
        selectedAction = iAction;
        switch (selectedAction)
        {
            case PLAYER_ACTIONS.MOVE:
                startAction = new UnityEvent();
                startAction.AddListener(Move);

                continueAction = new UnityEvent();
                continueAction.AddListener(StopMove);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(StopMove);
                break;
            case PLAYER_ACTIONS.PUZZLE:
                startAction = new UnityEvent();
                startAction.AddListener(SolvePuzzle);

                continueAction = new UnityEvent();
                //stopAction.AddListener(TryValidatePuzzle);

                cancelAction = new UnityEvent();
                cancelAction.AddListener(StopPuzzle);
                break;
            default:
                break;
        }
    }

    public void OnInteract(PlayerController iPlayer)
    {
        if (startAction!=null)
        {
            player = iPlayer;
            distFromPlayer = Vector3.Distance(transform.position, iPlayer.transform.position);
            distFromPlayer = Mathf.Clamp(distFromPlayer, 0.1f, iPlayer.actionDistance);

            startAction.Invoke();
        }
    }

    public void OnContinueInteract(PlayerController iPlayer)
    {
        if (iPlayer!=player)
            return;
        
        continueAction.Invoke();
    }

    public void OnCancelInteract(PlayerController iPlayer)
    {
        if (iPlayer!=player)
            return;
        cancelAction.Invoke();
    }

    #region MOVE
    public virtual void Move()
    {
        if (RB!=null)
        {
            RB.isKinematic = true;
            RB.useGravity = false;
        }
        

        // Clamp pos to center of screen
        if (ActionCo!=null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.ForceCursorToCloseHand();
        ActionCo = StartCoroutine(MoveCo());
    }

    public virtual void StopMove()
    {
        if (RB!=null)
        {
            RB.isKinematic = false;
            RB.useGravity = true;
        }

        if (ActionCo!=null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.ForceCursorToOpenHand();
    }

    public IEnumerator MoveCo()
    {
        while (player.playerInAction)
        {
            Vector3 worldPos = player.FPSCamera.GetRayFromScreenCenter().GetPoint(distFromPlayer);
            transform.position = worldPos;
            yield return null;
        }
    }
    #endregion

    #region PUZZLE
    public void SolvePuzzle()
    {

        if (ActionCo!=null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        puzzle.StartPuzzle(player);
        ActionCo = StartCoroutine(SolvePuzzleCo());
    }

    public void StopPuzzle()
    {
        if (ActionCo!=null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        puzzle.StopPuzzle();
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

}
