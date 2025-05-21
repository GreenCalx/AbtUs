using UnityEngine;
using System.Collections;

public class InteractibleCreature : InteractibleObject
{
    public Creature target;

    void Awake()
    {
        if (availableActions.Length >= 1)
        {
            ChangeSelectedAction(availableActions[0]);
        }
    }

    public override void Move()
    {
        target.isFrozen = true;
        target.modelTransform.position = target.transform.position;
        target.transform.up = Vector3.up;
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }

        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        ActionCo = StartCoroutine(MoveCo());
    }

    public override void StopMove()
    {
        target.isFrozen = false;
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.UpdateCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
    }
}