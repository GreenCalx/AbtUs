using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class SignalTarget : MonoBehaviour
{
    public List<SignalCheck> targetSignals;
    private List<SignalLink> links;

    public UnityEvent OnTargetMatchCB;
    public UnityEvent OnTargetMisMatchCB;
    public virtual void InitSignals()
    {
        links = new List<SignalLink>();
        foreach (SignalCheck sc in targetSignals)
        {
            SignalLink sl = new SignalLink(sc.source, this);
        }
    }

    public virtual void OnSignalUpdate()
    {
        // try to match target
        if (targetSignals.Count != links.Count)
        {
            OnTargetMisMatch();
            return;
        }
        for (int i = 0; i < targetSignals.Count; i++)
        {
            if (targetSignals[i].signal != links[i].signal)
            {
                OnTargetMisMatch();
                return;
            }
        }

        OnTargetMatch();
    }

    public virtual void OnTargetMatch()
    {
        if (OnTargetMatchCB != null)
            OnTargetMatchCB.Invoke();
    }
    public virtual void OnTargetMisMatch()
    {
        if (OnTargetMisMatchCB != null)
            OnTargetMisMatchCB.Invoke();
    }

    public void AddLink(SignalLink iSL)
    {
        if (links.Contains(iSL))
            return;
        links.Add(iSL);
    }

    public void RemoveLink(SignalLink iSL)
    {
        if (!links.Contains(iSL))
            return;
        links.Remove(iSL);
    }
}
