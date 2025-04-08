using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class BSTArc
{
    public BSTState A;
    public BSTState B;
    public Func<bool> triggers;

    public BSTArc( BSTState iA, BSTState iB, Func<bool> iTrigg)
    {
        A = iA;
        B = iB;
        triggers = iTrigg;
    }
}