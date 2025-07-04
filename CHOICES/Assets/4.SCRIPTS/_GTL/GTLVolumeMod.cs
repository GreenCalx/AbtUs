using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GTLVolumeMod : GTLModifier<Volume, VolumeProfile>
{
    public bool isReady = false;
    public VolumeProfile initProfile;
    List<VolumeComponent> lastSnapshot;
    void Start()
    {
        lastSnapshot = new List<VolumeComponent>();
        MakeSnapshot();
        ResetProfile();

        init();
        Activate();
    }

    void OnDestroy()
    {
        ResetProfile();
    }

    void ResetProfile()
    {
        foreach (var vc in initProfile.components)
        {
            var mutVC = GetComp(vc);
            if (mutVC == null)
                continue;
            vc.Override(mutVC, 1f);
        }
    }

    public VolumeComponent GetComp(VolumeComponent iVC)
    {
        List<VolumeComponent> l = lastSnapshot.Where(vc => vc.GetType() == iVC.GetType()).ToList();
        if (l.Count <= 0)
            return null;
        return l[0];
    }

    void MakeSnapshot()
    {
        lastSnapshot.Clear();
        foreach (var vc in modifierTarget.sharedProfile.components)
        {
            lastSnapshot.Add(vc);
        }
    }

    public override void ChangeTarget(VolumeProfile iVProfile)
    {
        MakeSnapshot();
        weight = 0f;
    }

    public override void ChangeTargetWeight(float iValue)
    {
        //modifierTarget.weight = iValue;
        // Override()
    }

    public override void Deactivate()
    {
        isActive = false;
        //modifierTarget.enabled = false;
        isReady = false;
    }
    public override void Activate()
    {
        //isActive = true;
        // modifierTarget.enabled = true;
        isActive = true;
        isReady = true;
    }
}
