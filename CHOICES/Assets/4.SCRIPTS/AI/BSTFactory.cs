using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class BSTFactory
{
    public static BST<InsectToken, InsectChecks, InsectActionPool> MakeInsectGraph()
    {
        BST<InsectToken, InsectChecks, InsectActionPool> bst = new BST<InsectToken, InsectChecks, InsectActionPool>();

        // Nodes
        bst.Build(BSTState.IDLE, Enum.GetValues(typeof(InsectState)).Cast<BSTState>().ToList());


        // connections
        bst.AddConnection(BSTState.IDLE, BSTState.DEAD, bst.checks.DeathCond);
        bst.AddConnection(BSTState.SEEK, BSTState.DEAD, bst.checks.DeathCond);
        bst.AddConnection(BSTState.FROZEN, BSTState.DEAD, bst.checks.DeathCond);

        bst.AddConnection(BSTState.IDLE, BSTState.FROZEN, bst.checks.FrozenCond);
        bst.AddConnection(BSTState.SEEK, BSTState.FROZEN, bst.checks.FrozenCond);

        bst.AddConnection(BSTState.IDLE, BSTState.SEEK, bst.checks.GoSeekCond);

        bst.AddConnection(BSTState.SEEK, BSTState.IDLE, bst.checks.FrozenCond);
        bst.AddConnection(BSTState.FROZEN, BSTState.IDLE, bst.checks.UnFrozenCond);

        // Node properties callbacks
        bst.EditNodeStayCB(BSTState.IDLE,       bst.nodeActionPool.OnIdle);

        bst.EditNodeEnterCB(BSTState.DEAD,      bst.nodeActionPool.Freeze);
        bst.EditNodeEnterCB(BSTState.DEAD,      bst.nodeActionPool.OnDeath);

        bst.EditNodeEnterCB(BSTState.FROZEN,     bst.nodeActionPool.Freeze);
        bst.EditNodeExitCB(BSTState.FROZEN,      bst.nodeActionPool.UnFreeze);

        return bst;
    }
}