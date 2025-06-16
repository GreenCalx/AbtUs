using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class Door : SignalTarget
{
    public Vector3 openPosOffset;
    public float toggleTime = 1f;
    private Vector3 closedPos;
    private Vector3 openPos;
    public bool shouldOpen = false;
    public float openeningFrac = 0f;
    public Coroutine doorSlideCo;

    public bool IsOperating() { return ((openeningFrac >= 0.1f) && (openeningFrac <= 0.99f)); }
    public bool IsOpen() { return openeningFrac >= 1f; }
    public bool IsClose() { return openeningFrac <= 0f; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openPosOffset;
        openeningFrac = 0f;

        InitSignals();
    }
    public void OnOpen()
    {
        if (IsOpen())
            return;

        shouldOpen = true;
        if (doorSlideCo != null)
        {
            StopCoroutine(doorSlideCo);
            doorSlideCo = null;
        }
        doorSlideCo = StartCoroutine(SlideDoor());
    }

    public void OnClose()
    {
        if (IsClose())
            return;

        shouldOpen = false;
        if (doorSlideCo != null)
        {
            StopCoroutine(doorSlideCo);
            doorSlideCo = null;
        }
        doorSlideCo = StartCoroutine(SlideDoor());
    }

    IEnumerator SlideDoor()
    {
        float frac = shouldOpen ? openeningFrac : 1f - openeningFrac;
        Vector3 startPos    = transform.position;
        Vector3 targetPos   = shouldOpen ? openPos : closedPos;

        while (frac < 1f)
        {
            frac += Time.deltaTime / toggleTime;
            if (frac > 1f) { frac = 1f; }
            openeningFrac = shouldOpen ? frac : 1f - frac;

            transform.position = Vector3.Lerp(startPos, targetPos, frac);
            yield return null;
        }

        transform.position = targetPos;
        openeningFrac = shouldOpen ? 1f : 0f;
        
    }
}
