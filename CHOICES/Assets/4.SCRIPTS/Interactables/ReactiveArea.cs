using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;


[System.Serializable]
public class ReactUnit
{
    public EActiveObject type;
    public IntEvent func;
}

public class ReactiveArea : MonoBehaviour
{
    [Header("Tweaks")]
    public List<ReactUnit> reactUnits_OnEnter;
    public List<ReactUnit> reactUnits_OnExit;

    [Header("Internals")]
    public List<ActiveObject> activeObjects = new List<ActiveObject>();

    private void RunEnterEffect(ActiveObject iObj)
    {
        foreach(ReactUnit ru in reactUnits_OnEnter)
        {
            if (ru.type == iObj.type)
            {
                ru.func.Invoke(activeObjects.Count);
                if (iObj.consumeObject)
                {
                    RemoveActiveObject(iObj);
                    iObj.Consume();
                }
            }
        }
    }

    private void RunExitEffect(ActiveObject iObj)
    {
        foreach(ReactUnit ru in reactUnits_OnExit)
        {
            if (ru.type == iObj.type)
            {
                ru.func.Invoke(activeObjects.Count);
            }
        }
    }

    public void AddActiveObject(ActiveObject iObj)
    {
        activeObjects.Add(iObj);

        // Don't execute effect if needs to be dropped
        if (iObj.needToBeDropped)
        {
            if (iObj.interact != null)
            {
                if (iObj.interact.IsInAction())
                    return;
            }
        }

        RunEnterEffect(iObj);
    }

    public void RemoveActiveObject(ActiveObject iObj)
    {
        activeObjects.Remove(iObj);
        activeObjects = activeObjects.Where(e => e!=null).ToList();

        if (iObj.consumeObject)
            return;
        
        RunExitEffect(iObj);
    }

    void OnTriggerEnter(Collider iCol)
    {
        ActiveObject as_ao = Utils.GetComp<ActiveObject>(iCol.gameObject);
        if (as_ao)
        {
            if (activeObjects.Contains(as_ao))
                return;
            AddActiveObject(as_ao);
        }
    }

    void OnTriggerExit(Collider iCol)
    {
        ActiveObject as_ao = Utils.GetComp<ActiveObject>(iCol.gameObject);
        if (as_ao)
        {
            if (!activeObjects.Contains(as_ao))
                return;
            RemoveActiveObject(as_ao);
        }
    }
}
