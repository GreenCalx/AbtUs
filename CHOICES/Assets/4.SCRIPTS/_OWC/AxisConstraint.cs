using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class AxisConstraintUnit
{
    public WORLD_AXIS axis;
    [Range(0f,1f)]
    public float minMagnitude;
    [Range(0f,1f)]
    public float maxMagnitude;

    public bool check()
    {
        float mag = OverWorldControl.Instance.GetAxisMagnitude(axis);
        return ((mag>=minMagnitude)&&(mag<=maxMagnitude));
    }
}
[System.Serializable]
public class AxisConstraint
{
    public List<AxisConstraintUnit> constraints;

    public bool checkAll()
    {
        foreach(AxisConstraintUnit u in constraints)
        {
            if (!u.check()) { return false; }
        }
        return true;
    }
}


