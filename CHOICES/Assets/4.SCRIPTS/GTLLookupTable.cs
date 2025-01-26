using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GTLLookupTable : MonoBehaviour
{
    public float NeutralVal = 0.5f;

    [Serializable]
    public class GTLLookupUnit
    {
        public VolumeProfile volumeProfile;
        public float GtL_Factor;
    }
    public List<GTLLookupUnit> units;

    public bool TryUpdateProfile(GTLModifier iMod, VolumeProfile iActiveProfile, float iGTLFactor)
    {
        List<VolumeProfile> eligibleProfiles = new List<VolumeProfile>();
        foreach(GTLLookupUnit u in units)
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
            } else {
                if (NeutralVal==u.GtL_Factor)
                    eligibleProfiles.Add(u.volumeProfile);
            }
        }
        if (eligibleProfiles.Count==0)
            return false;
        
        int selectedProfile = UnityEngine.Random.Range(0,eligibleProfiles.Count);
        iMod.ChangeVolume(eligibleProfiles[selectedProfile]);
        return true;
    }
}
