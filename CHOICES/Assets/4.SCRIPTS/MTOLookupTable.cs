using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTOLookupTable : MonoBehaviour
{
    [Serializable]
    public class MTOLookupUnit
    {
        public Material mat;
        [Range(0f,1f)]
        public float MtO_Factor;
        public List<Material> replacingThose;
    }
    
    public List<MTOLookupUnit> units;

    public Material ScoutForMatChange(Material iMat, float iMTOVal)
    {
        List<Material> eligibleMats = new List<Material>();

        foreach(MTOLookupUnit u in units)
        {
            foreach(Material m in u.replacingThose)
            {
                if (iMat.name.Contains(m.name))
                {
                    if (iMTOVal <= u.MtO_Factor)
                    {
                        if (!eligibleMats.Contains(u.mat))
                        {
                            eligibleMats.Add(u.mat);
                        }
                    }
                }
            }
        }

        int n_eligibles = eligibleMats.Count;
        if (n_eligibles==0)
            return null;

        int selected = UnityEngine.Random.Range(0, n_eligibles);

        return eligibleMats[selected];

    }

}
