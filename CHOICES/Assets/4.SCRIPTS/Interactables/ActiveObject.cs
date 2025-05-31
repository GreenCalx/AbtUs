using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
public enum EActiveObject
{
    NONE=0,
    SHROOM=1
}

public class ActiveObject : MonoBehaviour
{
    [Header("Tweaks")]
    public EActiveObject type;
    [Tooltip("No Exit effect is triggered")]
    public bool consumeObject = false;
    public bool needToBeDropped = false;

    [Header("Refs")]
    public InteractibleObject interact;

    [Header("Internals")]
    private List<ReactiveArea> areas = new List<ReactiveArea>();

    public void Consume()
    {
        Destroy(gameObject);
    }

    public void SubscribeToArea(ReactiveArea iArea)
    {
        if (!areas.Contains(iArea))
            areas.Add(iArea);
        iArea.AddActiveObject(this);
    }

    public void UnsubscribeToArea(ReactiveArea iArea)
    {
        if (areas.Contains(iArea))
        {
            areas.Remove(iArea);
            iArea.RemoveActiveObject(this);
        }
        areas = areas.Where(e => e != null).ToList();
    }

    public void UnsubscribeToAllArea()
    {
        foreach (var area in areas)
        {
            area.RemoveActiveObject(this);
        }
        areas.Clear();
    }

    void Update()
    {
        if (interact.isMovedByPlayer)
        {
            UnsubscribeToAllArea();
        }
    }
}
