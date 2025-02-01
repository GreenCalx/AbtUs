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
        foreach(GTLLookupVolumeUnit u in volumeUnits)
        {
            if (u.volumeProfile == iActiveProfile)
                continue;

            else if (iGTLFactor == NeutralVal)
            {
                if (NeutralVal==u.GtL_Factor)
                    eligibleProfiles.Add(u.volumeProfile);
            }
            else if (iGTLFactor > NeutralVal)
            {
                // LUSH
                if (iGTLFactor > u.GtL_Factor)
                    eligibleProfiles.Add(u.volumeProfile);
            } else if (iGTLFactor < NeutralVal) 
            {
                //GLOOMY
                if (iGTLFactor < u.GtL_Factor)
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
        foreach(GTLLookupLightUnit u in lightUnits)
        {
            if (u.light == iActiveSunLight)
                continue;
            
            if (iGTLFactor == NeutralVal)
            {
                if (u.GtL_Factor == NeutralVal)
                    eligibleSuns.Add(u.light);
            }
            else if (iGTLFactor > NeutralVal)
            {
                if (iGTLFactor > u.GtL_Factor)
                    eligibleSuns.Add(u.light);
            } 
            else if (iGTLFactor < NeutralVal)
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
