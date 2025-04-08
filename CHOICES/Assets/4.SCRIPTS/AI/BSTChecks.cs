using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BSTChecks<T> where T : BSTToken
{ 
    public T target;
    public BSTChecks() {}
}

public class InsectChecks : BSTChecks<InsectToken>
{
    public InsectChecks()
    {
        target = null;
    }

    public InsectChecks(InsectToken iTarget)
    {
        target = iTarget;
    }

    public bool DeathCond()
    {
        return target.behaviour.isDead;
    }

    public bool FrozenCond()
    {
        return target.behaviour.isFrozen;
    }

    public bool UnFrozenCond()
    {
        return !target.behaviour.isFrozen;
    }

    public bool GoSeekCond()
    {
        return false;
    }
}

public class DreamCatcherChecks : BSTChecks<DreamCatcherToken>
{
    public DreamCatcherChecks()
    {
        target = null;
    }

    public DreamCatcherChecks(DreamCatcherToken iTarget)
    {
        target = iTarget;
    }

    public bool DeathCond()
    {
        return target.behaviour.isDead;
    }

    public bool FrozenCond()
    {
        return target.behaviour.isFrozen;
    }

    public bool UnFrozenCond()
    {
        return !target.behaviour.isFrozen;
    }

    public bool GoSeekCond()
    {
        return false;
    }
}