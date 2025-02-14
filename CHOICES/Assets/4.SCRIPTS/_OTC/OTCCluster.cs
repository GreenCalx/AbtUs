using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class OTCCluster : MonoBehaviour
{
    [Header("unused")]
    public float clusterizationRange = 50f;
    [Header("TWEAKS")]
    public float margin = 20f;
    public float noiseShapeMultiplier = 2f;
    public bool retrieveModsInChildren = false;
    public readonly int MIN_CLUSTER_SIZE = 3;
    public List<OTCModifier> mods;
    public Terrain relatedTerrain;
    public OTCLookupTable.SPREAD_SHAPE orderForcedShape = OTCLookupTable.SPREAD_SHAPE.NONE;
    public OTCLookupTable.SPREAD_SHAPE chaosForcedShape = OTCLookupTable.SPREAD_SHAPE.NONE;
    [Header("Internals")]
    public OTCLookupTable.SPREAD_SHAPE currSpreadShape = OTCLookupTable.SPREAD_SHAPE.NONE;
    public Bounds clusterBounds;
    public BoundedObjectPool clusterPool;

    [Range(0,0.5f)]
    public float completeShapeThreshold = 0.5f;


    public bool CanBeAddedToCluster(OTCModifier iMod)
    {
        return Vector3.Distance(iMod.transform.position, clusterBounds.center) < clusterizationRange;
    }

    public void AddToCluster(OTCModifier iMod)
    {
        if (!mods.Contains(iMod))
        {
            mods.Add(iMod);
        }
    }

    void Awake()
    {
        if (retrieveModsInChildren)
        {
            mods = new List<OTCModifier>( GetComponentsInChildren<OTCModifier>().ToList());
        }
    }

    void Start()
    {
        foreach(OTCModifier m in mods) { m.Init_OTC(this); }
        OverWorldControl.Instance.SubscribeOTCCluster(this);

        Vector3 center = Vector3.zero;
        foreach(OTCModifier m in mods)
        {
            center += m.transform.localPosition;
        }
        center /= mods.Count;

        // set self to bound center
        GameObject newPool = new GameObject("ClusterBounds", typeof(BoundedObjectPool));
        clusterPool = newPool.GetComponent<BoundedObjectPool>();

        clusterPool.transform.position = center;
        clusterPool.transform.parent = transform;

        foreach(OTCModifier m in mods)
        {
            clusterPool.Encapsulate(m.gameObject);
        }
        clusterPool.ApplyMargin(margin);

        clusterBounds = clusterPool.bounds;
        
    }

    public void spread(OTCLookupTable iOTCTable, float iOTC_Factor)
    {
        // 0. No need to spread if inferior to cluster min size
        if (mods.Count < MIN_CLUSTER_SIZE)
            return;

        // 1. Get Spread shape & set destination for each mod
        if ((orderForcedShape!=OTCLookupTable.SPREAD_SHAPE.NONE)||(chaosForcedShape!=OTCLookupTable.SPREAD_SHAPE.NONE))
            iOTCTable.ComputeSpreadShape(this, iOTC_Factor, orderForcedShape, chaosForcedShape);
        else
            iOTCTable.ComputeSpreadShape(this, iOTC_Factor);

        // 2. Refresh Positions
        iOTCTable.RefreshTargetPositions(this,iOTC_Factor);

        // 3. Set Rotations
        iOTCTable.RefreshTargetRotations(this, iOTC_Factor);

        // 4. Set Scales
        iOTCTable.RefreshTargetScales(this, iOTC_Factor);

        // 5. Launch LERP !
        mods.ForEach(m => m.Launch());
    }
    
    public void ResetPositions()
    {
        foreach(OTCModifier m in mods)
        {
            m.targetPos = m.initPos;
        }
    }
    public void ResetRotations()
    {
        foreach(OTCModifier m in mods)
        {
            m.targetRot = m.initRot;
        }
    }
    public void ResetScales()
    {
        foreach(OTCModifier m in mods)
        {
            m.targetScale = m.initScale;
        }
    }


}
