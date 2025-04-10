using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class GTLLookupTable : MonoBehaviour
{
    public float NeutralVal = 0.5f;

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

    public List<GTLLookupVolumeUnit> volumeUnits;
    public List<GTLLookupLightUnit> lightUnits;
    public List<GTLLookupWaterUnit> waterUnits;

    public bool TryUpdateProfile(GTLVolumeMod iMod, VolumeProfile iActiveProfile, float iGTLFactor)
    {
        List<VolumeProfile> eligibleProfiles = new List<VolumeProfile>();
        bool isLush = OverWorldControl.Instance.LushMagnitude > 0f;
        bool isGloom = OverWorldControl.Instance.GloomyMagnitude > 0f;
        foreach(GTLLookupVolumeUnit u in volumeUnits)
        {
            if (u.volumeProfile == iActiveProfile)
                continue;
            else if (!isLush && !isGloom)
            {
                if (u.axisConstraint == WORLD_AXIS.ZERO)
                    eligibleProfiles.Add(u.volumeProfile);
            }
            else if (isLush && (u.axisConstraint == WORLD_AXIS.LUSH))
            {
                if (iGTLFactor >= u.GtL_Factor)
                    eligibleProfiles.Add(u.volumeProfile);
            } else if (isGloom && (u.axisConstraint == WORLD_AXIS.GLOOMY)) 
            {
                if (iGTLFactor <= u.GtL_Factor)
                    eligibleProfiles.Add(u.volumeProfile);
            }
        }
        if (eligibleProfiles.Count==0)
            return false;
        
        int selectedProfile = UnityEngine.Random.Range(0,eligibleProfiles.Count);
        iMod.ChangeTarget(eligibleProfiles[selectedProfile]);
        return true;
    }

    public bool TryUpdateSun(GTLLightMod iSunMod, Light iActiveSunLight, float iGTLFactor)
    {
        List<Light> eligibleSuns = new List<Light>();
        bool isLush = OverWorldControl.Instance.LushMagnitude > 0f;
        bool isGloom = OverWorldControl.Instance.GloomyMagnitude > 0f;

        foreach(GTLLookupLightUnit u in lightUnits)
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
                if (iGTLFactor > u.GtL_Factor)
                    eligibleSuns.Add(u.light);
            } 
            else if (isGloom && (u.axisConstraint == WORLD_AXIS.GLOOMY))
            {
                if (iGTLFactor < u.GtL_Factor)
                    eligibleSuns.Add(u.light);
            }
        }
        
        if (eligibleSuns.Count==0)
            return false;

        int selectedSun = UnityEngine.Random.Range(0,eligibleSuns.Count);
        iSunMod.ChangeTarget(eligibleSuns[selectedSun]);

        return true;
    }
}
