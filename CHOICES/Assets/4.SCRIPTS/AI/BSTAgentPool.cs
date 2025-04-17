using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSTAgentPool : MonoBehaviour
{
    public bool autoPlayAll = true;
    
    [Header("Insects")]
    public bool playInsects = false;
    public List<InsectBehaviour> insects;
    public BST<InsectToken, InsectChecks, InsectActionPool> insectBST;

    [Header("DreamCatchers")]
    public bool playDreamCatchers = false;
    public List<DreamCatcherBehaviour> dreamcatchers;
    public BST<DreamCatcherToken, DreamCatcherChecks, DreamCatcherActionPool> dreamcatcherBST;

    void Start()
    {
        init();
    }

    void init()
    {
        InitInsects();
        InitDreamCatchers();
    }

    void InitInsects()
    {
        if (insects.Count > 0)
        {
            insectBST = BSTFactory.MakeInsectGraph();
            foreach(InsectBehaviour i in insects)
            {
                InsectToken tok = new InsectToken( insectBST.GetNodeFromState(BSTState.IDLE), i, i);
                insectBST.AddToken( tok );
            }

            playInsects = autoPlayAll;
        }
    }

    void InitDreamCatchers()
    {
        if (dreamcatchers.Count > 0)
        {
            dreamcatcherBST = BSTFactory.MakeDreamCatcherGraph();
            foreach(DreamCatcherBehaviour dc in dreamcatchers)
            {
                DreamCatcherToken tok = new DreamCatcherToken( dreamcatcherBST.GetNodeFromState(BSTState.IDLE), dc, dc);
                dreamcatcherBST.AddToken( tok );
            }

            playDreamCatchers = autoPlayAll;
        }
    }

    void Update()
    {
        if (insectBST!=null)
        {
            insectBST.playMode = playInsects;
            insectBST.Update();
        }

        if (dreamcatcherBST!=null)
        {
            dreamcatcherBST.playMode = playDreamCatchers;
            dreamcatcherBST.Update();
        }
    }

    public void SubscribeInsect(InsectBehaviour iAgent)
    {
        if (!insects.Contains(iAgent))
            insects.Add(iAgent);
        
        if (insectBST==null)
        { InitInsects(); }
        else
        {
            InsectToken tok = new InsectToken( insectBST.GetNodeFromState(BSTState.IDLE), iAgent, iAgent);
            insectBST.AddToken( tok );
        }
    }

    public void UnSubscribeInsect(InsectBehaviour iAgent)
    {
        if (!insects.Contains(iAgent))
            return;
        insectBST.RemoveToken( insectBST.GetAgentToken(iAgent));
        insects.Remove(iAgent);
    }

    public void SubscribeDreamCatcher(DreamCatcherBehaviour iAgent)
    {
        if (!dreamcatchers.Contains(iAgent))
            dreamcatchers.Add(iAgent);
        
        if (dreamcatcherBST==null)
        { InitDreamCatchers(); }
        else
        {
            DreamCatcherToken tok = new DreamCatcherToken( dreamcatcherBST.GetNodeFromState(BSTState.IDLE), iAgent, iAgent);
            dreamcatcherBST.AddToken( tok );
        }
    }

    public void UnSubscribeDreamCatcher(DreamCatcherBehaviour iAgent)
    {
        if (!dreamcatchers.Contains(iAgent))
            return;
        dreamcatcherBST.RemoveToken( dreamcatcherBST.GetAgentToken(iAgent));
        dreamcatchers.Remove(iAgent);
    }
}