using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GTLVolumeMod : GTLModifier<Volume, VolumeProfile>
{
    
    void Start()
    {
        init();
    }
    
    public override void ChangeTarget(VolumeProfile iVProfile)
    {
        modifierTarget.sharedProfile = iVProfile;
        weight = 0f;
    }

    public override void ChangeTargetWeight(float iValue)
    {
        modifierTarget.weight = iValue;
    }

    public override void Deactivate() 
    {
        //isActive = false;
        modifierTarget.enabled = false;
    }
    public override void Activate() 
    {
        //isActive = true;
        modifierTarget.enabled = true;
    }

}