using UnityEngine;
using System.Collections;
using static EventLog;

public class InteractibleCreature : InteractibleObject
{
    public Creature target;

    // void Awake()
    // {
    //     if (def.availableActions.Length >= 1)
    //     {
    //         ChangeSelectedAction(def.availableActions[0]);
    //     }
    // }

    public override void Move()
    {
        isMovedByPlayer = true;
        IsInActionChain = false;
        target.isFrozen = true;

        target.modelTransform.position = target.transform.position;
        target.transform.up = Vector3.up;
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }

        UIGame.Instance.UpdateAltCursorFromPlayerAction(PLAYER_ACTIONS.MOVE);
        ActionCo = StartCoroutine(MoveCo(targetedTransfrom, RB));
    }

    public override void StopMove()
    {
        target.isFrozen = false;
        isMovedByPlayer = false;

        Managers.Instance.ObjectChains.RefreshFeedback();
        INFO("InteractibleCreature " + gameObject.name + " StopMove");
    }

    public override void Kill()
    {
        target.isFrozen = false;
        target.isDead = true;

        player.OnCreatureKill();
    }

    public override void Select()
    {
        target.isFrozen = true;
        if (selectionFX != null)
            selectionFX.Select();
    }

    public override void Deselect()
    {
        target.isFrozen = false;
        if (selectionFX != null)
            selectionFX.Deselect();
    }
}