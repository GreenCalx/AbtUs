using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OTCModifier : MonoBehaviour
{
    public OTCCluster cluster;
    public Vector3 initPos;
    public Vector3 initScale;
    public Quaternion initRot;

    [Header("Tweaks")]
    public bool FollowTerrainHeight = true;
    public float SeedRadius = 5;
    public float TimeToReachTarget = 5f; // in sec
    [Header("Internals")]
    public bool GoToTarget = false;
    public Vector3 targetPos;
    public Quaternion targetRot;
    public Vector3 targetScale;

    public Vector3 launchPos;
    public Quaternion launchRot;
    public Vector3 launchScale;
    private float elapsedTime = 0f;

    private float y_offset = 0f;
    public bool debug = false;
    public void Init_OTC(OTCCluster iCluster)
    {
        OverWorldControl.Instance.SubscribeOTC(this);
        cluster = iCluster;
        ModelTools mt = GetComponent<ModelTools>();
        if(mt != null && mt.clipToTerrain && FollowTerrainHeight)
        {
            y_offset = mt.terrainOffset;
        }
        else if (FollowTerrainHeight && mt == null)
        {
            Vector3 pos = transform.position;
            pos.y = cluster.relatedTerrain.SampleHeight(transform.position);
            transform.position =  pos;
        }

        initPos = transform.localPosition;
        initScale = transform.localScale;
        initRot = transform.localRotation;

        GoToTarget = false;

    }

    void Update()
    {
        if (GoToTarget)
        {
            elapsedTime += Time.deltaTime;

            bool changePos = ChangePosition();
            bool changeRot = ChangeRotation();
            bool changeScale = ChangeScale();
            if (    changePos &&
                    changeRot &&
                    changeScale
                )
            { GoToTarget = false; }
        }
    }

    public void Launch()
    {
        GoToTarget = true;
        elapsedTime = 0f;

        launchPos = transform.localPosition;
        launchRot = transform.localRotation;
        launchScale = transform.localScale;
    }

    public bool ChangePosition()
    {
        float journeyFrac = elapsedTime / TimeToReachTarget;
        if (journeyFrac>=1f)
        {
            if (FollowTerrainHeight)
            {
                Vector3 lastStep = transform.parent.transform.TransformPoint(targetPos);
                lastStep.y = cluster.relatedTerrain.SampleHeight(lastStep) + y_offset;
                transform.localPosition = transform.parent.transform.InverseTransformPoint(lastStep);
            }
            else
            {
                transform.localPosition = targetPos;
            }
            return true;
        }

        Vector3 nextStep = Vector3.Lerp(launchPos, targetPos, journeyFrac);
        if (FollowTerrainHeight)
        {
            Vector3 globalNextStep = transform.parent.transform.TransformPoint(nextStep);
            globalNextStep.y = cluster.relatedTerrain.SampleHeight(globalNextStep) + y_offset; 
            nextStep = transform.parent.transform.InverseTransformPoint(globalNextStep);
            if (debug)
                { Debug.DrawRay(globalNextStep, Vector3.up * 50, Color.red); }
        }
        transform.localPosition = nextStep;

        return false;
    }

    public bool ChangeRotation()
    {
        float journeyFrac = elapsedTime / TimeToReachTarget;
        if (journeyFrac>=1f)
        {
            transform.localRotation = targetRot;
            return true;
        }
        Quaternion nextStep = Quaternion.Lerp(launchRot, targetRot, journeyFrac);
        transform.localRotation = nextStep;

        return false;
    }

    public bool ChangeScale()
    {
        float journeyFrac = elapsedTime / TimeToReachTarget;
        if (journeyFrac==1f)
        {
            transform.localScale = targetScale;
            return true;
        }
        Vector3 nextStep = Vector3.Lerp(launchScale, targetScale, journeyFrac);
        transform.localScale = nextStep;

        return false;
    }
}
