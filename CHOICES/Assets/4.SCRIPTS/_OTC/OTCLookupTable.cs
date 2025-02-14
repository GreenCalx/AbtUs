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
        ALIGN_X, ALIGN_Z, TRIANGLE, SQR, CIRCLE, GRID, HEX, ARCH,
        // CHAOS
        NOISE
        }
    public readonly List<SPREAD_SHAPE> orderShapes = new List<SPREAD_SHAPE>()
    {   SPREAD_SHAPE.ALIGN_X, SPREAD_SHAPE.ALIGN_Z, SPREAD_SHAPE.TRIANGLE, 
        SPREAD_SHAPE.SQR, SPREAD_SHAPE.CIRCLE, SPREAD_SHAPE.GRID, SPREAD_SHAPE.HEX, SPREAD_SHAPE.ARCH 
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
                AlignOnX(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                AlignOnZ(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                Triangulate(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.SQR:
                Square(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.CIRCLE:
                Circle(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.GRID:
                Grid(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.HEX:
                break;
            case SPREAD_SHAPE.ARCH:
                Arch(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.NOISE:
                Noise(iCluster, chaosFactorCurve.Evaluate(iOTC));
                break;
            default:
                break;
        }
    }

    public void RefreshTargetRotations(OTCCluster iCluster, float iOTC)
    {
        if (iOTC==NeutralVal)
        {
            iCluster.ResetRotations();
            return;
        }

        switch (iCluster.currSpreadShape)
        {
            case SPREAD_SHAPE.ALIGN_X:
                AlignRotOnX(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                AlignRotOnZ(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                AlignRotWithCenter(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.SQR:
                AlignRotWithCenter(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.CIRCLE:
                AlignRotWithCenter(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.GRID:
                AlignRotWithCenter(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.HEX:
                AlignRotWithCenter(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.ARCH:
                AlignRotOnZ(iCluster, computeiOrderFactor(iCluster, iOTC));
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
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.ALIGN_Z:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.TRIANGLE:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.SQR:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.CIRCLE:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.GRID:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.HEX:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));            
                break;
            case SPREAD_SHAPE.ARCH:
                HarmonizeScales(iCluster, computeiOrderFactor(iCluster, iOTC));
                break;
            case SPREAD_SHAPE.NOISE:
                RandomizeScales(iCluster, orderFactorCurve.Evaluate(iOTC));    
                break;

            default:
                break;
        }
    }

    #region POS_SPREADS
    public void AlignOnX(OTCCluster iCluster, float iOrderFactor)
    {
        // Vector2 P0 = new Vector2(clusterPlan.minXZ.x, clusterPlan.center.y);
        // Vector2 P1 = new Vector2(clusterPlan.maxXZ.x, clusterPlan.center.y);
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        foreach (OTCModifier m in iCluster.mods)
        {
            Vector3 idealPos = new Vector3(relative_center.x, relative_center.y + m.transform.localPosition.y, relative_center.z);
            m.targetPos = Vector3.Lerp( m.initPos, idealPos, iOrderFactor);
            //m.targetPos.x = Mathf.Clamp(m.targetPos.x, - iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.x);
            //m.targetPos.z = Mathf.Clamp(m.targetPos.z, -iCluster.clusterBounds.extents.z, iCluster.clusterBounds.extents.z);
        }
    }

    public void AlignOnZ(OTCCluster iCluster, float iOrderFactor)
    {
        // Vector2 P0 = new Vector2(clusterPlan.minXZ.x, clusterPlan.center.y);
        // Vector2 P1 = new Vector2(clusterPlan.maxXZ.x, clusterPlan.center.y);
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        foreach (OTCModifier m in iCluster.mods)
        {
            Vector3 idealPos = new Vector3(relative_center.x + m.transform.localPosition.x, relative_center.y + m.transform.localPosition.y, relative_center.z);
            m.targetPos = Vector3.Lerp(m.initPos, idealPos, iOrderFactor);
            //m.targetPos.x = Mathf.Clamp(m.targetPos.x, -iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.x);
            //m.targetPos.z = Mathf.Clamp(m.targetPos.z, -iCluster.clusterBounds.extents.z, iCluster.clusterBounds.extents.z);
        }
    }


    public void Triangulate(OTCCluster iCluster, float iOrderFactor)
    {
        int order = 0;
        int count = iCluster.mods.Count;
        float minDim = Mathf.Min(iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.z);
        float sideLength = Mathf.Sqrt(2f) * minDim;
        int sideCount= Mathf.CeilToInt(count / 3f);
        float step = sideLength / sideCount;
        Vector3 TargetPosition = (2 / 3f) * sideLength * new Vector3(0,1,0);
        foreach (OTCModifier m in iCluster.mods)
        {
            m.targetPos = m.targetPos = Vector3.Lerp(m.initPos, TargetPosition, iOrderFactor); 
            if(order < sideCount)
            {
                TargetPosition.x += step * Mathf.Cos(5.23599f);
                TargetPosition.y += step * Mathf.Sin(5.23599f);
            }
            else if (order < 2 * sideCount)
            {
                TargetPosition.x -= step;
            }
            else
            {
                float angle = 4 * Mathf.PI / 3;
                TargetPosition.x -= step * Mathf.Cos(angle);
                TargetPosition.y -= step * Mathf.Sin(angle);
            }
            order++;

        }
    }

    public void Square(OTCCluster iCluster, float iOrderFactor)
    {
        int count = iCluster.mods.Count;
        int innerSquareSize =Mathf.CeilToInt((count - 4) / 4);
        int gridSize = innerSquareSize + 2;
        float minDim = Mathf.Min(iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.z);
        float spacing = minDim / (gridSize - 1);
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        int row = 0, col = 0;
        Vector3 idealTargetPos;
        foreach (OTCModifier m in iCluster.mods)
        {
            idealTargetPos = new Vector3(
                relative_center.x - minDim + col * spacing,
                relative_center.y,
                relative_center.z - minDim + row * spacing
            );
            m.targetPos = Vector3.Lerp(m.initPos, idealTargetPos, iOrderFactor);
            if (row != 0 && row != gridSize -1 && col != gridSize - 1)
            {
                col = gridSize - 1;
            }
            else
            {
                col++;
            }
            if (col >= gridSize) 
            {
                col = 0;
                row++;
            }

        }
    }

    public void Arch(OTCCluster iCluster, float iOrderFactor)
    {

        int order = 0;
        int count = iCluster.mods.Count;
        int pillarCount = Mathf.FloorToInt(count / 3f);
        float step = Mathf.PI / (count + 1 - 2 * pillarCount);
        float pillarSize = Mathf.Min(iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.z);
        float pillarStep = pillarSize / pillarCount;
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        Vector3 idealTargetPos;
        Debug.Log(iOrderFactor);
        foreach (OTCModifier m in iCluster.mods)
        {
            if(order < pillarCount)
            {
                idealTargetPos = new Vector3(pillarSize / 2, -pillarSize / 2 + order * pillarStep, 0);// + relative_center;
            }
            else if(order >= pillarCount && order < count - pillarCount)
            {
                idealTargetPos = new Vector3(Mathf.Cos(step * (1 + order - pillarCount)) * pillarSize / 2, pillarSize / 2 - pillarStep + Mathf.Sin(step * (1 + order - pillarCount)) * pillarSize / 3, 0);// + relative_center;
            }
            else
            {
                idealTargetPos = new Vector3(-pillarSize / 2, -(pillarSize / 2) + (order - count + pillarCount) * pillarStep, 0);// + relative_center;
            }

            m.targetPos = Vector3.Lerp(m.initPos, idealTargetPos, iOrderFactor);
            order += 1;

        }
    }
    public void Circle(OTCCluster iCluster, float iOrderFactor)
    {
        float step = 2 * Mathf.PI / iCluster.mods.Count;
        float order = 0;
        float radius = Mathf.Min(iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.z);
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        Vector3 idealTargetPos;
        foreach (OTCModifier m in iCluster.mods)
        {
            idealTargetPos = new Vector3(relative_center.x  + Mathf.Cos(step * order) * radius, relative_center.y, relative_center.z + + Mathf.Sin(step * order) * radius);
            m.targetPos = Vector3.Lerp(m.initPos, idealTargetPos, iOrderFactor);
            order += 1;
        }
    }

    public void Grid(OTCCluster iCluster, float iOrderFactor)
    {
        int count = iCluster.mods.Count;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count)); // Déterminer la taille de la grille (carré)
        float minDim = Mathf.Min(iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.z);
        float spacing = minDim / (gridSize - 1);
        Vector3 relative_center = iCluster.clusterBounds.center - iCluster.transform.position;
        int row = 0, col = 0;
        Vector3 idealTargetPos;
        foreach (OTCModifier m in iCluster.mods)
        {
            idealTargetPos = new Vector3(
                relative_center.x - minDim + col * spacing,
                relative_center.y,
                relative_center.z - minDim + row * spacing
            );
            m.targetPos = Vector3.Lerp(m.initPos, idealTargetPos, iOrderFactor);
            col++;
            if (col >= gridSize)  // Passer à la ligne suivante
            {
                col = 0;
                row++;
            }
        }

    }

    public void Noise(OTCCluster iCluster, float iChaosFactor)
    {
        foreach(OTCModifier m in iCluster.mods)
        {
            m.targetPos = new Vector3(
                m.initPos.x + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.x, 
                m.initPos.y + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.y, 
                m.initPos.z + UnityEngine.Random.Range(-iChaosFactor*iCluster.noiseShapeMultiplier, iChaosFactor*iCluster.noiseShapeMultiplier)*m.initScale.z );
            m.targetPos.x = Mathf.Clamp(m.targetPos.x, -iCluster.clusterBounds.extents.x, iCluster.clusterBounds.extents.x);
            m.targetPos.z = Mathf.Clamp(m.targetPos.z, -iCluster.clusterBounds.extents.z, iCluster.clusterBounds.extents.z);
            m.targetPos.y = Mathf.Clamp(m.targetPos.y, -iCluster.clusterBounds.extents.y, iCluster.clusterBounds.extents.y);

        }
    }

    public float computeiOrderFactor(OTCCluster iCluster, float iOTC)
    {
        if(iCluster.completeShapeThreshold == 0.5f) { return orderFactorCurve.Evaluate(iOTC); }
        if(iCluster.completeShapeThreshold >= iOTC) { return 1; }
        return orderFactorCurve.Evaluate((iOTC - iCluster.completeShapeThreshold)/ (0.5f - iCluster.completeShapeThreshold));

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
