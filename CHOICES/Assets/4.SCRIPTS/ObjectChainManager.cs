using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using static EventLog;

public class ObjectChain<T> : IDisposable where T : InteractibleObject
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

    public float Evaluate()
    {
        // chain root doesn't count towards chaos
        float retval = Count - 1;
        if (Count < 3)
            return retval;

        List<Vector3> positions = new List<Vector3>();
        foreach(var o in objects) { positions.Add(o.transform.position); }
        float orderMatchScore = OTCShapeChecker.GetShapeMatching(positions);

        Debug.Log("Shape matching score : " + orderMatchScore);
        if (orderMatchScore > 0f) // order
            return -retval * orderMatchScore;
        else // chaos
            return retval;
    }
}

public class ObjectChainManager : MonoBehaviour, IFeedbackEval
{
    [Header("Refs")]
    public GameObject prefab_ChainLineRenderer;
    public GameFeedback otc_feedback;

    [Header("Internals")]
    public List<ObjectChain<InteractibleObject>> chains = new List<ObjectChain<InteractibleObject>>();

    void Start()
    {
        if (otc_feedback == null)
            Debug.LogError("otc_feedback ref missing on ObjectChainManager.");
        otc_feedback.Init(this);
    }

    public virtual float feedbackEvaluator()
    {
        if (chains == null)
            return 0f;

        float retval = 0f;
        foreach (var chain in chains)
        { retval += chain.Evaluate(); }

        return retval * GameSettings.Instance.DuplicationMulFactor;
    }

    public void RefreshFeedback()
    {
        otc_feedback.Refresh();
    }

    public bool CreateChain(InteractibleObject iObj)
    {
        var newChain = new ObjectChain<InteractibleObject>(iObj);
        if (chains.Contains(newChain))
        {
            FAIL("Create chain for " + iObj.gameObject.name);
            return false;
        }

        chains.Add(newChain);
        OK("Create chain for " + iObj.gameObject.name);
        return true;
    }

    public bool AddToChain(InteractibleObject iTargetedChainRefElem, InteractibleObject iToAdd)
    {
        int chainIndex = GetChainIndex(iTargetedChainRefElem);
        if (chainIndex < 0)
        {
            FAIL("Add chain for " + iToAdd.gameObject.name+" with chainIndex " + chainIndex);
            return false;
        }
            
        chains[chainIndex].objects.Add(iToAdd);
        chains[chainIndex].fXLinks.Add(FXLink(iTargetedChainRefElem, iToAdd));

        otc_feedback.Refresh();
        OK("Add chain for " + iToAdd.gameObject.name+" with chainIndex " + chainIndex);
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

        OK("DeleteLastFromChain " + iChainMember.gameObject.name+" with chainIndex " + chainIndex);

        otc_feedback.Refresh();
    }

    public bool DestroyChain(InteractibleObject iObj)
    {
        int chainIndex = GetChainIndex(iObj);
        if (chainIndex < 0)
        {
            FAIL("Delete chain for " + iObj.gameObject.name );
            return false;
        }

        chains[chainIndex].Dispose();
        chains.RemoveAt(chainIndex);
        chains = chains.Where(e => e != null).ToList();
        OK("Delete chain for " + iObj.gameObject.name );

        otc_feedback.Refresh();

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
