using UnityEngine;
using System.Collections.Generic;

public abstract class OWCListener :  MonoBehaviour
{
    [Header("OWCListener : Tweaks")]
    public AxisConstraint axisConstraint;

    private void Start()
    {
        foreach(AxisConstraintUnit a in axisConstraint.constraints)
        {
            OverWorldControl.Instance.SubscribeListener(this, a.axis);
        }
        Init();
    }

    protected abstract void Init();
    public abstract void Call();

}
