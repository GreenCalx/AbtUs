using UnityEngine;
using System.Collections.Generic;

public class OWCEnabler : OWCListener
{
    public float minOWCSpawnRange = 0;
    public float maxOWCSpawnRange = 1;

    bool isOWCInRange;

    private List<GameObject> enablerChildren;
    protected override void Init(float axis_value)
    {
        enablerChildren = new List<GameObject>();
        foreach(Transform child in GetComponentInChildren<Transform>()) // Todo on editor 
        {
            GameObject childObj = child.gameObject;
            if (childObj != this.gameObject)
            {
                enablerChildren.Add(childObj);
                isOWCInRange = axis_value > minOWCSpawnRange && axis_value < maxOWCSpawnRange;
                childObj.SetActive(isOWCInRange);
            }
        }
    }

    public override void Call(float axis_value)
    {
        if(isOWCInRange == (axis_value > minOWCSpawnRange && axis_value < maxOWCSpawnRange)) { return; }
        isOWCInRange = !isOWCInRange;

        foreach (GameObject child in GetComponentInChildren<Transform>()) // Todo on editor 
        {
            if (child != this.gameObject)
            {
                child.SetActive(isOWCInRange);
            }
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
