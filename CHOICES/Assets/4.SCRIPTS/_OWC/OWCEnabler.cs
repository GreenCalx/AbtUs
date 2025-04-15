using UnityEngine;
using System.Collections.Generic;

public class OWCEnabler : OWCListener
{
    [Header("Tweaks")]
    public float minOWCSpawnRange = 0;
    public float maxOWCSpawnRange = 1;
    [Header("Targets")]
    public bool useChildren = true;
    public List<GameObject> enablerChildren;
    [Header("Internals")]
    public bool isOWCInRange;

    protected override void Init(float axis_value)
    {
        isOWCInRange = axis_value > minOWCSpawnRange && axis_value < maxOWCSpawnRange;
        if (useChildren)
        {
            enablerChildren = new List<GameObject>();
            foreach(Transform child in transform) // Todo on editor 
            {
                enablerChildren.Add(child.gameObject);
                child.gameObject.SetActive(isOWCInRange);
            }
        }
    }

    public override void Call(float axis_value)
    {
        isOWCInRange = (axis_value > minOWCSpawnRange && axis_value < maxOWCSpawnRange);
        foreach (Transform child in transform) // Todo on editor 
        {
            child.gameObject.SetActive(isOWCInRange);
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
