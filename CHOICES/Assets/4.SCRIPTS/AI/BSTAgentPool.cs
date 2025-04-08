using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSTAgentPool : MonoBehaviour
{
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

            insectBST.playMode = true;
        }
    }

    void Update()
    {
        if (insectBST!=null)
        {
            insectBST.playMode = playInsects;
            insectBST.Update();
        }
    }


    #region INSECTS
    public bool playInsects = false;
    public List<InsectBehaviour> insects;
    public BST<InsectToken, InsectChecks, InsectActionPool> insectBST;


    #endregion

}