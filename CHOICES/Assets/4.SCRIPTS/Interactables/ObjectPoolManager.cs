using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using static EventLog;
public enum OBJ_NATURE {
    NONE = WORLD_AXIS.ZERO,
    MINERAL = WORLD_AXIS.MINERAL,
    ORGANIC = WORLD_AXIS.ORGANIC
};

public interface IPoolable
{
    public Transform GetTransform();
    public OBJ_NATURE GetNature();
    public string GetName();
    public bool UseInFeedback();
    public void OnPoolSleep();
    public void OnPoolAwake();
}

[ExecuteInEditMode]
public class ObjectPoolManager : MonoBehaviour, IFeedbackEval
{
    [Header("MAND REFS")]
    public List<ObjectPool> pools;
    public int startPoolID = 0;
    public GameObject prefab_mtoFeedback;
    private GameFeedback mto_feedback;

    [Header("Internals")]
    public List<int> activePools;


    private int mineralPool = 0;
    private int organicPool = 0;

    public void Init()
    {
        int index = 0;
        foreach (ObjectPool pool in pools)
        {
            pool.id = index;
            index++;
        }

        mto_feedback = Instantiate(prefab_mtoFeedback, transform).GetComponent<GameFeedback>();
        mto_feedback.Init(this);

        activePools = new List<int>();
        activePools.Add(startPoolID);
    }

    void OnDestroy()
    {

    }

    public virtual float feedbackEvaluator()
    {
        return organicPool - mineralPool;
    }

    public void AddObject(IPoolable iObj)
    {
        INFO("Add Poolable object " + iObj.GetName() + " by seekind ID in parent via manager");
        int poolID = -1;

        //ObjectPool parentPool = iObj.GetTransform().gameObject.GetComponentInParent<ObjectPool>();
        ObjectPool parentPool = Utils.GetCompInParent<ObjectPool>(iObj.GetTransform().gameObject);
        if (parentPool == null)
        {
            FAIL("Retrieve ObjectPool for " + iObj.GetName() + " in parent hierarchy");
            return;
        }
        poolID = parentPool.id;
        AddObject(iObj, poolID);
    }                 

    public void AddObject(IPoolable iObj, int poolID)
    {
        INFO("Add Poolable object " + iObj.GetName() + " to pool ID " + poolID + " via manager");
        pools[poolID]?.Add(iObj);

        if (iObj.UseInFeedback())
        {
            if (iObj.GetNature() == OBJ_NATURE.MINERAL) { mineralPool++; }
            else if (iObj.GetNature() == OBJ_NATURE.ORGANIC) { organicPool++; }
        }
        
        mto_feedback?.Refresh();
    }

    public void RemoveObject(IPoolable iObj)
    {
        INFO("Remove Poolable object " + iObj.GetName() + " by seekind ID in parent via manager");
        int poolID = -1;

        ObjectPool parentPool = iObj.GetTransform().gameObject.GetComponentInParent<ObjectPool>();
        if (parentPool == null)
        {
            FAIL("Retrieve ObjectPool for " + iObj.GetName() + " in parent hierarchy");
            return;
        }
        poolID = parentPool.id;
        RemoveObject(iObj, poolID);
    }

    public void RemoveObject(IPoolable iObj, int poolID)
    {
        INFO("Remove Poolable object " + iObj.GetName() + " to pool ID " + poolID + " via manager");
        pools[poolID]?.Remove(iObj);
        if (iObj.UseInFeedback())
        {
            if (iObj.GetNature() == OBJ_NATURE.MINERAL) { mineralPool++; }
            else if (iObj.GetNature() == OBJ_NATURE.ORGANIC) { organicPool++; }
        }
        mto_feedback?.Refresh();
    }

    public void Enable(int iPoolID)
    {
        if (activePools.Contains(iPoolID))
        {
            FAIL("Enable pool id : " + iPoolID + " via manager");
            return;
        }
            
        activePools.Add(iPoolID);
        pools[iPoolID].OnEnable();
    }

    public void Disable(int iPoolID)
    {
        if (!activePools.Contains(iPoolID))
        {
            FAIL("Disable pool id : " + iPoolID + " via manager");
            return;
        }
        pools[iPoolID].OnDisable();
        activePools.Remove(iPoolID);
        //activePools = activePools.Where(e => e != null).ToList();
    }

    public void EnableSolo(int iPoolID)
    {
        foreach (int i in activePools)
        { pools[i].OnDisable(); }

        activePools.Clear();

        activePools.Add(iPoolID);
        pools[iPoolID].OnEnable();
        INFO("EnableSolo to pool ID " + iPoolID + " via manager");
    }

}
