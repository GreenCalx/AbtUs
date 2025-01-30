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
    public readonly List<SPREAD_SHAPE> orderShapes = new List<SPREAD_SHAPE>()
    {   SPREAD_SHAPE.ALIGN_X, SPREAD_SHAPE.ALIGN_Z, SPREAD_SHAPE.TRIANGLE, 
        SPREAD_SHAPE.SQR, SPREAD_SHAPE.CIRCLE, SPREAD_SHAPE.GRID, SPREAD_SHAPE.HEX 
    };
    public readonly List<SPREAD_SHAPE> chaosShapes = new List<SPREAD_SHAPE>()
    { SPREAD_SHAPE.NOISE };

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

    public bool ShapeIsOrder(SPREAD_SHAPE iShape) { return orderShapes.Contains(iShape); }
    public bool ShapeIsChaos(SPREAD_SHAPE iShape) { return chaosShapes.Contains(iShape); }

    public void ComputeSpreadShape(OTCCluster iCluster, float iOTC, SPREAD_SHAPE iForceShapeOrder = SPREAD_SHAPE.NONE, SPREAD_SHAPE iForceShapeChaos = SPREAD_SHAPE.NONE)
    {
        bool askForChaos = (iOTC > NeutralVal);
        bool askForOrder = (iOTC < NeutralVal);

        // Don't recompute shape if the computed one already fits
        if (askForOrder && ShapeIsOrder(iCluster.currSpreadShape))
        { return; }
        if (askForChaos && ShapeIsChaos(iCluster.currSpreadShape))
        { return; }

        List<SPREAD_SHAPE> eligibleShapes = new List<SPREAD_SHAPE>();
        if ((iForceShapeOrder!=SPREAD_SHAPE.NONE)&&(iOTC < NeutralVal))
        {
                eligibleShapes.Add(iForceShapeOrder);
        } else if((iForceShapeChaos!=SPREAD_SHAPE.NONE)&&(iOTC > NeutralVal))
        {
            eligibleShapes.Add(iForceShapeChaos);
        }
        else
        {
            foreach(OTCLookupUnit u in units)
            {
                if (askForChaos)
                { // CHAOS
                    if (iOTC > u.OtC_Factor)
                        eligibleShapes.Add(u.spreadShape);
                } else if (askForOrder) 
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

            m.targetPos.x = Mathf.Clamp(m.targetPos.x, iCluster.clusterBounds.min.x, iCluster.clusterBounds.max.x);
            m.targetPos.z = Mathf.Clamp(m.targetPos.z, iCluster.clusterBounds.min.z, iCluster.clusterBounds.max.z);
        }
    }

    public void AlignOnX(OTCCluster iCluster, float iOrderFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            Vector3 idealPos = new Vector3(iCluster.clusterBounds.center.x, m.transform.position.y, m.transform.position.z );
            m.targetPos = Vector3.Lerp( m.initPos, idealPos, iOrderFactor);
            
            m.targetPos.x = Mathf.Clamp(m.targetPos.x, iCluster.clusterBounds.min.x, iCluster.clusterBounds.max.x);
            m.targetPos.z = Mathf.Clamp(m.targetPos.z, iCluster.clusterBounds.min.z, iCluster.clusterBounds.max.z);
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
                m.initPos.x + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.x, 
                m.initPos.y + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.y, 
                m.initPos.z + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.z );
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
