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
}