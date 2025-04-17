using UnityEngine;
using System.Collections.Generic;

public class OWCEnabler : OWCListener
{


    [Header("Targets")]
    public bool useChildren = true;
    public List<GameObject> enablerChildren;
    [Header("Internals")]
    public bool isOWCInRange;

    protected override void Init()
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

    public override void Call()
    {
        foreach (GameObject child in enablerChildren)
        {
            child.SetActive(axisConstraint.checkAll());
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
