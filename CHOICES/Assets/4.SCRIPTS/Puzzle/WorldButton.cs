using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class WorldButton : SignalSource
{
    public bool forcePress = false;
    public bool forceRelease = false;

    private List<InteractibleObject> pressers;
    public bool isPressed = false;

    void Start()
    {
        pressers = new List<InteractibleObject>();
    }

    void Update()
    {
        if (forcePress)
        {
            Emit();
            forcePress = false;
        }
        if (forceRelease)
        {
            StopEmit();
            forceRelease = false;
        }
    }

    void RemovePresser(InteractibleObject iObj)
    {
        if (!pressers.Contains(iObj))
            return;
        pressers.Remove(iObj);
        pressers = pressers.Where(e => e != null).ToList();

        if (isPressed && (pressers.Count <= 0))
        {
            StopEmit();
            isPressed = false;
        }
    }

    void AddPresser(InteractibleObject iObj)
    {
        if (pressers.Contains(iObj))
            return;
        pressers.Add(iObj);
        UnityEvent<InteractibleObject, bool> ev = new UnityEvent<InteractibleObject, bool>();
        ev.AddListener(PresserIsMoving);

        UnityEvent<InteractibleObject> ev_shatt = new UnityEvent<InteractibleObject>();
        ev_shatt.AddListener(PresserIsShattered);

        iObj.AddMoveListener(ev);
        iObj.AddShatterListener(ev_shatt);

        if (!isPressed && (pressers.Count > 0))
        {
            Emit();
            isPressed = true;
        }
    }

    public void PresserIsMoving(InteractibleObject iObj, bool iIsMoving)
    {
        if (iIsMoving)
            RemovePresser(iObj);
    }

    public void PresserIsShattered(InteractibleObject iObj)
    {
        RemovePresser(iObj);
    }

    void OnCollisionEnter(Collision iCol)
    {
        InteractibleObject obj = iCol.gameObject.GetComponent<InteractibleObject>();
        if (obj != null)
        {
            if (!obj.isMovedByPlayer)
            {
                AddPresser(obj);
            }
        }
    }

    void OnCollisionStay(Collision iCol)
    {
        InteractibleObject obj = iCol.gameObject.GetComponent<InteractibleObject>();
        if (obj != null)
        {
            if (!obj.isMovedByPlayer)
            {
                AddPresser(obj);
            }
            else
            {
                RemovePresser(obj);
            }
        }
    }

    void OnCollisionExit(Collision iCol)
    {
        InteractibleObject obj = iCol.gameObject.GetComponent<InteractibleObject>();
        if (obj != null)
        {
            RemovePresser(obj);
        }
    }
}
