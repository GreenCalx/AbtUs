using UnityEngine;
using System.Collections.Generic;

public class OWCEnabler : OWCListener
{
    [Header("Tweaks")]
    public AxisConstraint axisConstraint;

    [Header("Targets")]
    public bool useChildren = true;
    public List<GameObject> enablerChildren;
    [Header("Internals")]
    public bool isOWCInRange;

    protected override void Init(float axis_value)
    {
        if (useChildren)
        {
            enablerChildren = new List<GameObject>();
            foreach(Transform child in transform)
            {
                enablerChildren.Add(child.gameObject);
                child.gameObject.SetActive(axisConstraint.checkAll());
            }
        }
    }

    public override void Call(float axis_value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(axisConstraint.checkAll());
        }
    }

    public void Remove(GameObject obj)
    {
        enablerChildren.Remove(obj);
    }

    public void Add(GameObject obj)
    {
        enablerChildren.Add(obj);
    }
}
