using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;


public class BST<T, C, V>   where  T : BSTToken
                            where  C : BSTChecks<T>, new()
                            where  V : BSTActionPool<T>, new()
{
    public bool playMode = false;

    [Header("Graph")]
    public BSTNode RootNode;
    public List<BSTNode> graph;
    public List<BSTArc> arcs;
    public List<T> agents;
    public C checks;
    public V nodeActionPool;

    public BST() {}

    public void Build(BSTState iInitState, List<BSTState> iNodes)
    {
        graph = new List<BSTNode>();
        foreach ( BSTState s in iNodes)
        {
            BSTNode newNode = new BSTNode(s, s==iInitState);
            graph.Add(newNode);
            if (s==iInitState)
            { RootNode = newNode; }
            
        }
        arcs = new List<BSTArc>();
        agents = new List<T>();

        checks = new C();
        nodeActionPool = new V();
    }

    public BSTNode GetNodeFromState(BSTState iState)
    {
        foreach ( BSTNode n in graph)
        {
            if (n.state == iState)
            { return n; }
        }
        return null;
    }

    public void EditNodeEnterCB(BSTState iState, UnityAction iCB)
    {
        BSTNode n = GetNodeFromState(iState);
        if (n!=null)
        {
            n.nodeEnterCallbacks += iCB;
        }
    }

    public void EditNodeStayCB(BSTState iState, UnityAction iCB)
    {
        BSTNode n = GetNodeFromState(iState);
        if (n!=null)
        {
            n.nodeStayCallbacks += iCB;
        }
    }

    public void EditNodeExitCB(BSTState iState, UnityAction iCB)
    {
        BSTNode n = GetNodeFromState(iState);
        if (n!=null)
        {
            n.nodeExitCallbacks = iCB;
        }
    }

    public void AddToken(T iTok)
    {
        if (!agents.Contains(iTok))
            agents.Add(iTok);
    }

    public void RemoveToken(T iTok)
    {
        if (iTok==null)
        { Debug.LogWarning("Removing null token in BST"); }
        agents = agents.Where(e => e != iTok).ToList();
    }

    public T GetAgentToken(BSTAgent iAgent)
    {
        foreach(T tok in agents)
        {
            if (tok.agent == iAgent)
                return tok;
        }
        return null;
    }

    public void AddConnection(BSTState iStateA, BSTState iStateB, Func<bool> iTriggers)
    {
        if (graph==null)
        { return; }

        BSTArc newArc = new BSTArc(iStateA, iStateB, iTriggers);
        if (arcs.Contains(newArc))
            return;
        
        arcs.Add(newArc);
        foreach(BSTNode n in graph)
        {
            if (n.state == iStateA)
            {
                if (!n.arcs.Contains(newArc))
                {
                    n.arcs.Add(newArc);
                }
            }
        }
    }


    public void ChangeState(T iAgent, BSTNode iDestination)
    {
        iAgent.currNode.OnNodeExit();

        iAgent.currNode = iDestination;

        iAgent.currNode.OnNodeEnter();
    }

    public void Play(bool iShouldPlay)
    {
        playMode = iShouldPlay;
    }
    public void Update()
    {
        if (!playMode)
            return;
        
        foreach( T tok in agents)
        {
            checks.target = tok;
            nodeActionPool.target = tok;
            
            bool stateChanged = false;
            foreach( BSTArc arc in tok.currNode.arcs)
            {
                if (arc.triggers())
                {
                    ChangeState(tok, GetNodeFromState(arc.B));
                    stateChanged = true;
                }
            }

            if (!stateChanged)
            {
                tok.currNode.OnNodeStay();
            }

        }
    }
}