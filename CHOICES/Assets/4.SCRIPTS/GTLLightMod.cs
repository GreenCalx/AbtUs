using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GTLLightMod : GTLModifier<Light, Light>
{
    //public Light light;
    public Quaternion initRot = Quaternion.identity;

    void Start()
    {
        initRot = transform.rotation;
        modifierTarget.enabled = isActive;
        init();
    }
    public override void ChangeTarget(Light iLight)
    {
        modifierTarget = iLight;
        weight = 0f;
    }

    public override void ChangeTargetWeight(float iValue)
    {
        // TODO : intensity lerp from init value to 0 & vice versa ?
        transform.rotation = Quaternion.Lerp(initRot, modifierTarget.transform.rotation, iValue);
    }
}