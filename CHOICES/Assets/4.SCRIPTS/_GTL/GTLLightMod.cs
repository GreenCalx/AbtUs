using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GTLLightMod : GTLModifier<Light, Light>
{
    private class LightSnapshot
    {
        public Quaternion rotation;
        public float intensity;
        public Color color;
        public float colorTemperature;

        public LightSnapshot(Light iLight)
        {
            rotation = iLight.transform.rotation;
            intensity = iLight.intensity;
            color = iLight.color;
            colorTemperature = iLight.colorTemperature;
        }
    }
    public Light initLight;
    private LightSnapshot prevSnap;
    private LightSnapshot currSnap;
    void Start()
    {
        modifierTarget.enabled = isActive;
        init();
        modifierTarget = initLight;
    }
    public override void ChangeTarget(Light iLight)
    {
        prevSnap = new LightSnapshot(modifierTarget);
        currSnap = new LightSnapshot(iLight);

        modifierTarget = iLight; 
        weight = 0f;
    }

    public override void ChangeTargetWeight(float iValue)
    {
        transform.rotation = Quaternion.Lerp(prevSnap.rotation, currSnap.rotation, iValue);
        modifierTarget.intensity = Utils.Lerp(prevSnap.intensity, currSnap.intensity, iValue);
        modifierTarget.colorTemperature = Utils.Lerp(prevSnap.colorTemperature, currSnap.colorTemperature, iValue);
        modifierTarget.color = Color.Lerp(prevSnap.color, currSnap.color, iValue);
    }

    public override void Deactivate() 
    {
        weight = 0f;
        isActive = false;
        modifierTarget.enabled = false;
    }
    public override void Activate() 
    {
        weight = 1f;
        isActive = true;
        modifierTarget.enabled = true;
    }
}