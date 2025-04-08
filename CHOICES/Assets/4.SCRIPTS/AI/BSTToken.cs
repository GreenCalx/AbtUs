
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class BSTToken
{
    private BSTNode CurrNode;
    public BSTNode currNode
    {
        set { 
            CurrNode = value;  
            if (agent!=null)
            {agent.currState = CurrNode.state;}
        }
        get { return CurrNode;}
    }
    public BSTAgent agent;
    public BSTToken(BSTNode iStartNode, BSTAgent iAgent)
    {
        agent = iAgent;
        currNode = iStartNode;
    }
}

public class InsectToken : BSTToken
{
    public InsectBehaviour behaviour;

    public InsectToken(BSTNode iStartNode, BSTAgent iAgent, InsectBehaviour iBehaviour) : base (iStartNode, iAgent)
    {
        behaviour = iBehaviour;
    }
}