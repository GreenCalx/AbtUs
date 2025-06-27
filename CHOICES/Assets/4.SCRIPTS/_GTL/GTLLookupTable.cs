using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable]
public class GTLLookupUnit
{
    public float GtL_Factor;
    public WORLD_AXIS axisConstraint;
}
[Serializable]
public class GTLLookupVolumeUnit : GTLLookupUnit
{
    public VolumeProfile volumeProfile;
}
[Serializable]
public class GTLLookupLightUnit : GTLLookupUnit
{
    public Light light;
}
[Serializable]
public class GTLLookupWaterUnit : GTLLookupUnit
{
    public WaterSurface water;
}
    
public class GTLLookupTable : MonoBehaviour
{
    public float NeutralVal = 0.5f;
    public VolumeTableSO volumeUnits;
    public List<GTLLookupLightUnit> lightUnits;
    public List<GTLLookupWaterUnit> waterUnits;

    public bool TryUpdateProfile(GTLVolumeMod iMod, VolumeProfile iActiveProfile)
    {
        List<GTLLookupVolumeUnit> eligibleProfiles = new List<GTLLookupVolumeUnit>();

        float lushMag = OverWorldControl.Instance.LushMagnitude;
        float gloomMag = OverWorldControl.Instance.GloomyMagnitude;

        bool isLush = lushMag > 0f;
        bool isGloom = gloomMag > 0f;
        foreach (GTLLookupVolumeUnit u in volumeUnits.volumeUnits)
        {
            // if (u.volumeProfile == iActiveProfile)
            //     continue;
            if (!isLush && !isGloom)
            {
                if (u.axisConstraint == WORLD_AXIS.ZERO)
                    eligibleProfiles.Add(u);
            }
            else if (isLush && (u.axisConstraint == WORLD_AXIS.LUSH))
            {
                if (lushMag >= u.GtL_Factor)
                    eligibleProfiles.Add(u);
            }
            else if (isGloom && (u.axisConstraint == WORLD_AXIS.GLOOMY))
            {
                if (gloomMag >= u.GtL_Factor)
                    eligibleProfiles.Add(u);
            }
        }
        if (eligibleProfiles.Count == 0)
            return false;

        // pick highest value
        float highest = -1f;
        GTLLookupVolumeUnit selected = null;
        foreach (GTLLookupVolumeUnit u in eligibleProfiles)
        {
            if (u.GtL_Factor > highest)
            {
                highest = u.GtL_Factor;
                selected = u;
            }
        }

        if (selected.volumeProfile == iActiveProfile)
            return false;

        iMod.ChangeTarget(selected.volumeProfile);
        return true;
    }

    public bool TryUpdateSun(GTLLightMod iSunMod, Light iActiveSunLight, float iGTLFactor)
    {
        List<Light> eligibleSuns = new List<Light>();
        float lushMag = OverWorldControl.Instance.LushMagnitude;
        float gloomMag = OverWorldControl.Instance.GloomyMagnitude;

        bool isLush = lushMag > 0f;
        bool isGloom = gloomMag > 0f;

        foreach (GTLLookupLightUnit u in lightUnits)
        {
            if (u.light == iActiveSunLight)
                continue;

            if (!isLush && !isGloom)
            {
                if (u.axisConstraint == WORLD_AXIS.ZERO)
                    eligibleSuns.Add(u.light);
            }
            else if (isLush && (u.axisConstraint == WORLD_AXIS.LUSH))
            {
                if (lushMag >= u.GtL_Factor)
                    eligibleSuns.Add(u.light);
            }
            else if (isGloom && (u.axisConstraint == WORLD_AXIS.GLOOMY))
            {
                if (gloomMag >= u.GtL_Factor)
                    eligibleSuns.Add(u.light);
            }
        }

        if (eligibleSuns.Count == 0)
            return false;

        int selectedSun = UnityEngine.Random.Range(0, eligibleSuns.Count);
        iSunMod.ChangeTarget(eligibleSuns[selectedSun]);

        return true;
    }

    public void ModSelect()
    {

    }
}
