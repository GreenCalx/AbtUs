using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SignalLink
{
    public SignalSource source;
    public SignalTarget target;
    public SignalLink(SignalSource iSource, SignalTarget iTarget)
    {
        source = iSource;
        target = iTarget;
        mSignal = false;

        target.AddLink(this);
        source.AddLink(this);
    }
    //-------------------

    private bool mSignal;
    public bool signal
    {
        get
        {
            return mSignal;
        }
        set
        {
            mSignal = value;
            target.OnSignalUpdate();
        }
    }
}
