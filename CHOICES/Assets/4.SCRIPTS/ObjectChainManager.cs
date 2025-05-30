using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


public class ObjectChain<T> : IFeedbackEval, IDisposable where T : InteractibleObject
{
    public List<T> objects;
    public List<ObjectChainLR> fXLinks;

    public int Count
    {
        get { return objects.Count; }
    }
    public ObjectChain(T iChainRoot)
    {
        objects = new List<T>();
        objects.Add(iChainRoot);

        fXLinks = new List<ObjectChainLR>();
    }

    public virtual float feedbackEvaluator()
    {
        if (objects == null)
            return 0f;
        return objects.Count * 0.05f;
    }

    public void Dispose()
    {
        int n = Count;
        for (int i = 0; i < n; i++)
        {
            foreach (ObjectChainLR lnk in fXLinks)
                GameObject.Destroy(lnk.gameObject);

            //
            if (!objects[i].ShatterAnim())
            {
                GameObject.Destroy(objects[i].gameObject);
            }
        }
        GC.SuppressFinalize(this);
    }

    public T PeekAndRemoveLast()
    {
        T obj = objects[Count - 1];
        objects.RemoveAt(Count - 1);
        return obj;
    }
}

public class ObjectChainManager : MonoBehaviour
{
    [Header("Refs")]
    public GameObject prefab_ChainLineRenderer;
    [Header("Internals")]
    public List<ObjectChain<InteractibleObject>> chains = new List<ObjectChain<InteractibleObject>>();

    public bool CreateChain(InteractibleObject iObj)
    {
        var newChain = new ObjectChain<InteractibleObject>(iObj);
        if (chains.Contains(newChain))
            return false;
        chains.Add(newChain);
        return true;
    }

    public bool AddToChain(InteractibleObject iTargetedChainRefElem, InteractibleObject iToAdd)
    {
        int chainIndex = GetChainIndex(iTargetedChainRefElem);
        if (chainIndex < 0)
            return false;
        chains[chainIndex].objects.Add(iToAdd);
        chains[chainIndex].fXLinks.Add(FXLink(iTargetedChainRefElem, iToAdd));
        return true;
    }

    public ObjectChainLR FXLink(InteractibleObject iHolder, InteractibleObject iTarget)
    {
        GameObject link_inst = GameObject.Instantiate(prefab_ChainLineRenderer);
        link_inst.transform.parent = null;
        link_inst.transform.position = Vector3.zero;

        ObjectChainLR as_oclr = link_inst.GetComponent<ObjectChainLR>();
        as_oclr.Init(iHolder.transform, iTarget.transform);
        return as_oclr;
    }

    public void DeleteLastFromChain(InteractibleObject iChainMember)
    {
        int chainIndex = GetChainIndex(iChainMember);

        int last_elem = chains[chainIndex].Count - 1;
        InteractibleObject to_rm = chains[chainIndex].PeekAndRemoveLast();
        Destroy(to_rm.gameObject);
    }

    public bool DestroyChain(InteractibleObject iObj)
    {
        int chainIndex = GetChainIndex(iObj);
        if (chainIndex < 0)
            return false;

        chains[chainIndex].Dispose();
        chains.RemoveAt(chainIndex);
        chains = chains.Where(e => e != null).ToList();
        return true;
    }


    int GetChainIndex(InteractibleObject iObj)
    {
        int idx = -1;
        for (int i = 0; i < chains.Count; i++)
        {
            if (chains[i].objects.Contains(iObj))
            {
                idx = i;
                break;
            }
        }
        return idx;
    }
}
