using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTOLookupTable : MonoBehaviour
{
    [Serializable]
    public class MTOLookupUnit
    {
        public WORLD_AXIS axisConstraint;
        public Material mat;
        [Range(0f,1f)]
        public float MtO_Factor;
        //public List<Material> replacingThose;
    }

    [Serializable]
    public class MTOTerrainLookupUnit
    {
        public WORLD_AXIS axisConstraint;
        public SOTerrainPalette palette;
        [Range(0f,1f)]
        public float MtO_Factor;
    }
    
    public List<MTOLookupUnit> units;
    public List<MTOTerrainLookupUnit> terrainUnits;

    public Material ScoutForMatChange(Material iMat, float iMTOVal)
    {
        List<Material> eligibleMats = new List<Material>();
        bool isMineral = OverWorldControl.Instance.MineralMagnitude > 0f;
        bool isOrganic = OverWorldControl.Instance.OrganicMagnitude > 0f;
        foreach(MTOLookupUnit u in units)
        {
            if (!isOrganic && !isMineral)
            {
                if (u.axisConstraint == WORLD_AXIS.ZERO)
                {
                    if (!eligibleMats.Contains(u.mat))
                    {
                        eligibleMats.Add(u.mat);
                    }
                }
            }
            if (isOrganic && (u.axisConstraint == WORLD_AXIS.ORGANIC))
            {
                if (iMTOVal >= u.MtO_Factor)
                {
                    if (!eligibleMats.Contains(u.mat))
                    {
                        eligibleMats.Add(u.mat);
                    }
                }
            }
            else if (isMineral && (u.axisConstraint == WORLD_AXIS.MINERAL))
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

        int n_eligibles = eligibleMats.Count;
        if (n_eligibles==0)
            return null;

        int selected = UnityEngine.Random.Range(0, n_eligibles);

        return eligibleMats[selected];

    }

    public List<TerrainLayer> ScoutForTerrainLayersChange(MTOTerrain iTerrain, float iMTOVal)
    {
        List<SOTerrainPalette> eligibleLayers = new List<SOTerrainPalette>();

        bool isMineral = OverWorldControl.Instance.MineralMagnitude > 0f;
        bool isOrganic = OverWorldControl.Instance.OrganicMagnitude > 0f;

        foreach ( MTOTerrainLookupUnit u in terrainUnits)
        {
            if (!isOrganic && !isMineral)
            {
                // ZERO
            }
            else if (isOrganic && (u.axisConstraint == WORLD_AXIS.ORGANIC))
            {
                if (iMTOVal >= u.MtO_Factor)
                {
                    if (!eligibleLayers.Contains(u.palette))
                    {
                        eligibleLayers.Add(u.palette);
                    }
                }
            }
            else if (isMineral && (u.axisConstraint == WORLD_AXIS.MINERAL))
            {
                if (iMTOVal <= u.MtO_Factor)
                {
                    if (!eligibleLayers.Contains(u.palette))
                    {
                        eligibleLayers.Add(u.palette);
                    }
                }
            }
        }

        int n_eligibles = eligibleLayers.Count;
        if (n_eligibles==0)
            return null;

        int selected = UnityEngine.Random.Range(0, n_eligibles);

        return eligibleLayers[selected].PaletteLayers;
    }

}
