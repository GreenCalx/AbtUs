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

    public Vector3 parentPos;

    public Vector3 launchPos;
    public Quaternion launchRot;
    public Vector3 launchScale;
    private float elapsedTime = 0f;

    void Start()
    {
        OverWorldControl.Instance.SubscribeOTC(this);

        initPos = transform.localPosition;
        initScale = transform.localScale;
        initRot = transform.localRotation;
        parentPos = GetComponentInParent<Transform>().position;
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
                Vector3 lastStep = targetPos - parentPos;
                lastStep.y = cluster.relatedTerrain.SampleHeight(lastStep) - parentPos.y;
                Debug.Log(lastStep.y);
                transform.localPosition = lastStep;
            }
            else
            {
                transform.localPosition = targetPos;
            }
            return true;
        }

        Vector3 nextStep = Vector3.Lerp(launchPos, targetPos, journeyFrac);
        Debug.Log(nextStep);
        if (FollowTerrainHeight)
        {
            nextStep.y = cluster.relatedTerrain.SampleHeight(nextStep) - parentPos.y;
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
