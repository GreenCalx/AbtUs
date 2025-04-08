using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class BSTNode
{
    public bool isRoot = false;
    public BSTState state= BSTState.IDLE;
    public UnityAction nodeEnterCallbacks;
    public UnityAction nodeStayCallbacks;
    public UnityAction nodeExitCallbacks;

    public List<BSTArc> arcs;

    public void OnNodeEnter() 
    {
        if (nodeEnterCallbacks!=null)
            nodeEnterCallbacks.Invoke();
    }
    
    public void OnNodeStay()
    {
        if (nodeStayCallbacks!=null)
            nodeStayCallbacks.Invoke();
    }
    public void OnNodeExit() 
    {
        if (nodeExitCallbacks!=null)
            nodeExitCallbacks.Invoke();
    }

    public BSTNode(BSTState iState, bool iIsRoot)
    {
        state = iState;
        isRoot = iIsRoot;
        arcs = new List<BSTArc>();
    }

    public string GetName() { return state.ToString(); }
}
