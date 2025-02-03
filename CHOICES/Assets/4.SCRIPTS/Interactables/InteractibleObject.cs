using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum PLAYER_ACTIONS { NONE =0, MOVE =1, INFO =2, TALK =3}

[Serializable]
public class InteractibleObject : MonoBehaviour
{
    [Header("Tweaks")]
    public PLAYER_ACTIONS[] availableActions;

    [Header("Internals")]
    private PLAYER_ACTIONS selectedAction;
    private UnityEvent startAction;
    private UnityEvent stopAction;
    private PlayerController player;
    private float distFromPlayer = 0f;
    public Rigidbody RB;
    private Coroutine ActionCo;

    void Start()
    {
        if (RB==null)
        { RB = GetComponent<Rigidbody>(); }

        if (availableActions.Length >= 1)
        {
            ChangeSelectedAction(availableActions[0]);
        }
    }

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

                stopAction = new UnityEvent();
                stopAction.AddListener(StopMove);
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

    public void OnStopInteract(PlayerController iPlayer)
    {
        if (iPlayer!=player)
            return;
        
        stopAction.Invoke();
    }

    public void Move()
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
        ActionCo = StartCoroutine(MoveCo());
    }

    public void StopMove()
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
}
