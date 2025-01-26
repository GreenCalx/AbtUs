using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OTCLookupTable : MonoBehaviour
{
    [Tooltip("Checks for order if below, chaos if upper, none if equal")]
    public float NeutralVal = 0.5f;
    public enum SPREAD_SHAPE { 
        NONE,
        // ORDER
        ALIGN_X, ALIGN_Z, TRIANGLE, SQR, CIRCLE, GRID, HEX,
        // CHAOS
        NOISE
        }
    [Serializable]
    public class OTCLookupUnit
    {
        public SPREAD_SHAPE spreadShape;
        public float OtC_Factor;
    }

    public List<OTCLookupUnit> units;

    [Tooltip("X : ]0.5,1] , Y is displacement units for noise ")]
    public AnimationCurve chaosFactorCurve;

    [Tooltip("X :  [0,0.5[ , Y is lerp factor between init pos and target pos ")]
    public AnimationCurve orderFactorCurve;

    public void ComputeSpreadShape(OTCCluster iCluster, float iOTC, SPREAD_SHAPE iForceShape = SPREAD_SHAPE.NONE)
    {
        List<SPREAD_SHAPE> eligibleShapes = new List<SPREAD_SHAPE>();
        if (iForceShape!=SPREAD_SHAPE.NONE)
        {
            eligibleShapes.Add(iForceShape);
        }
        else
        {
            foreach(OTCLookupUnit u in units)
            {
                if (iOTC > NeutralVal)
                { // CHAOS
                    if (iOTC > u.OtC_Factor)
                        eligibleShapes.Add(u.spreadShape);
                } else if (iOTC < NeutralVal) 
                {
                    // ORDER
                    if (iOTC < u.OtC_Factor)
                        eligibleShapes.Add(u.spreadShape);
                }
            }
        }

        if (eligibleShapes.Count==0)
            eligibleShapes.Add(SPREAD_SHAPE.NONE);
        SPREAD_SHAPE selectedShape = eligibleShapes[UnityEngine.Random.Range(0,eligibleShapes.Count)];
        iCluster.currSpreadShape = selectedShape;
    }

    public void RefreshTargetPositions(OTCCluster iCluster, float iOTC)
    {
        if (iOTC==NeutralVal)
        {
            iCluster.ResetPositions();
            return;
        }

        switch (iCluster.currSpreadShape)
        {
            case SPREAD_SHAPE.ALIGN_X:
                AlignOnX(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                AlignOnZ(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                break;
            case SPREAD_SHAPE.SQR:
                break;
            case SPREAD_SHAPE.CIRCLE:
                break;
            case SPREAD_SHAPE.GRID:
                break;
            case SPREAD_SHAPE.HEX:
                break;
            case SPREAD_SHAPE.NOISE:
                Noise(iCluster, chaosFactorCurve.Evaluate(iOTC));
                break;
            default:
                break;
        }
    }

    public void RefreshTargetRotations(OTCCluster iCluster, float iOTC )
    {
        if (iOTC==NeutralVal)
        {
            iCluster.ResetRotations();
            return;
        }

        switch (iCluster.currSpreadShape)
        {
            case SPREAD_SHAPE.ALIGN_X:
                AlignRotOnX(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                AlignRotOnZ(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                AlignRotWithCenter(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.SQR:
                AlignRotWithCenter(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.CIRCLE:
                AlignRotWithCenter(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.GRID:
                break;
            case SPREAD_SHAPE.HEX:
                AlignRotWithCenter(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.NOISE:
                
                break;
            default:
                break;
        }
    }

    public void RefreshTargetScales(OTCCluster iCluster, float iOTC)
    {
        if (iOTC==NeutralVal)
        {
            iCluster.ResetScales();
            return;
        }

        switch (iCluster.currSpreadShape)
        {
            case SPREAD_SHAPE.ALIGN_X:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.SQR:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.CIRCLE:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.GRID:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));
                break;
            case SPREAD_SHAPE.HEX:
                HarmonizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));            
                break;
            case SPREAD_SHAPE.NOISE:
                RandomizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));    
                break;
            default:
                break;
        }
    }

    #region POS_SPREADS
    public void AlignOnZ(OTCCluster iCluster, float iOrderFactor)
    {
        // Vector2 P0 = new Vector2(clusterPlan.minXZ.x, clusterPlan.center.y);
        // Vector2 P1 = new Vector2(clusterPlan.maxXZ.x, clusterPlan.center.y);
        foreach(OTCModifier m in iCluster.mods)
        {
            Vector3 idealPos = new Vector3(m.transform.position.x, m.transform.position.y, iCluster.clusterBounds.center.z );
            m.targetPos = Vector3.Lerp( m.initPos, idealPos, iOrderFactor);
        }
    }

    public void AlignOnX(OTCCluster iCluster, float iOrderFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            Vector3 idealPos = new Vector3(iCluster.clusterBounds.center.x, m.transform.position.y, m.transform.position.z );
            m.targetPos = Vector3.Lerp( m.initPos, idealPos, iOrderFactor);
        }
    }

    public void Triangulate()
    {

    }

    public void Square()
    {

    }

    public void Circle()
    {

    }

    public void Grid()
    {

    }

    public void Noise(OTCCluster iCluster, float iChaosFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            m.targetPos = new Vector3(
                m.initPos.x + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor), 
                m.initPos.y + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor), 
                m.initPos.z + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor) );
        }
    }
    #endregion
    
    #region ROT_SPREADS
    public void AlignRotOnX(OTCCluster iCluster, float iOrderFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            Quaternion idealRot = Quaternion.FromToRotation(transform.right, Vector3.right);
            m.targetRot = Quaternion.Lerp( m.initRot, idealRot, iOrderFactor);
        }
    }

    public void AlignRotOnZ(OTCCluster iCluster, float iOrderFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            Quaternion idealRot = Quaternion.FromToRotation(transform.forward, Vector3.forward);
            m.targetRot = Quaternion.Lerp( m.initRot, idealRot, iOrderFactor);
        }
    }

    public void AlignRotWithCenter(OTCCluster iCluster, float iOrderFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            Vector3 relativePosToCenter = iCluster.clusterBounds.center - transform.position;
            relativePosToCenter.y = transform.position.y;

            Quaternion idealRot = Quaternion.LookRotation( relativePosToCenter, Vector3.up);
            m.targetRot = Quaternion.Lerp( m.initRot, idealRot, iOrderFactor);
        }
    }
    #endregion

    #region SCALE_SPREADS
    public void HarmonizeScales(OTCCluster iCluster, float iOrderFactor)
    {
        Vector3 medScale = Vector3.zero;
        foreach(OTCModifier m in iCluster.mods)
        {
            medScale += m.initScale;
        }
        medScale /= iCluster.mods.Count;

        foreach(OTCModifier m in iCluster.mods)
        {
            m.targetScale = medScale;
        }
    }

    public void RandomizeScales(OTCCluster iCluster, float iChaosFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            m.targetScale = new Vector3(
                m.initScale.x + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor), 
                m.initScale.y + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor), 
                m.initScale.z + UnityEngine.Random.Range(-iChaosFactor, iChaosFactor) );
        }
    }
    #endregion
}
