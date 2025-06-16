using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalSource : MonoBehaviour
{
    private List<SignalLink> links = new List<SignalLink>();

    public void AddLink(SignalLink iSL)
    {
        if (!links.Contains(iSL))
        {
            links.Add(iSL);
            iSL.signal = false;
         }
    }

    public void RemoveLink(SignalLink iSL)
    {
        if (links.Contains(iSL))
            links.Remove(iSL);
    }
    protected void Emit() { foreach (SignalLink sl in links) { sl.signal = true; } }
    protected void StopEmit() { foreach(SignalLink sl in links) { sl.signal = false; } }
}
