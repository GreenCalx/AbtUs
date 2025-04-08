using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public class BSTActionPool<T> where T : BSTToken
{
    public T target;
    public virtual void OnIdle() {}
    public virtual void OnDeath() {}
    public virtual void OnSeek() {}
    public virtual void Freeze() {}
    public virtual void UnFreeze() {}
}

public class InsectActionPool : BSTActionPool<InsectToken>
{
    public override void OnIdle()
    {
        target.behaviour.navAgent.enabled = true;
        target.behaviour.Idle();
    }

    public override void OnDeath()
    {
        target.behaviour.Kill();
    }

    public override void Freeze()
    {
        target.behaviour.navAgent.enabled = false;
    }
    public override void UnFreeze()
    {
        target.behaviour.navAgent.enabled = true;
        target.behaviour.ResetDestination();
    }
    public override void OnSeek()
    {

    }
}

public class DreamCatcherActionPool : BSTActionPool<DreamCatcherToken>
{
    public override void OnIdle()
    {
        target.behaviour.navAgent.enabled = true;
        target.behaviour.Idle();
    }

    public override void OnDeath()
    {
        target.behaviour.Kill();
    }

    public override void Freeze()
    {
        target.behaviour.navAgent.enabled = false;
    }
    public override void UnFreeze()
    {
        target.behaviour.navAgent.enabled = true;
        target.behaviour.ResetDestination();
    }
    public override void OnSeek()
    {

    }
}