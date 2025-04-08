using UnityEngine;
using System;
using System.Collections;

public enum BSTState
{
    DEAD=0,
    IDLE=1,
    FROZEN=2,
    PATROL=3,
    SEEK=4,
}

public enum InsectState
{
    DEAD = BSTState.DEAD,
    IDLE = BSTState.IDLE,
    FROZEN = BSTState.FROZEN,
    SEEK = BSTState.SEEK

}

public enum DreamCatcherState
{
    DEAD = BSTState.DEAD,
    IDLE = BSTState.IDLE,
    FROZEN = BSTState.FROZEN,
    PATROL = BSTState.PATROL,
    SEEK = BSTState.SEEK

}