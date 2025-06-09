using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    [Header("Mand Refs")]
    public GameCamera playerCam;
    [Header("Internals")]
    public Transform initPlayerCamParent;
    public float initPlayerFOV;
    public Coroutine camLerpCo;

    public Quaternion registeredRotation;
    public Vector3 registeredPosition;

    void Start()
    {
        initPlayerCamParent = playerCam.transform.parent;
        initPlayerFOV = playerCam.cam.fieldOfView;
    }

    public void RegisterPlayerCamTransform()
    {
        registeredRotation = playerCam.transform.localRotation;
        registeredPosition = playerCam.transform.localPosition;
    }

    public void SetCamToRegisteredTransform(float iTime)
    {
        ResetParent();
        if (iTime <= 0)
        {
            playerCam.transform.localPosition = registeredPosition;
            playerCam.transform.localRotation = registeredRotation;
            playerCam.cam.fieldOfView = initPlayerFOV;
            return;
        }

        if (camLerpCo != null)
        {
            StopCoroutine(camLerpCo);
            camLerpCo = null;
        }
        StartCoroutine(LerpCamToCo(registeredPosition, registeredRotation, initPlayerFOV, iTime));

    }


    public void ResetParent()
    {
        playerCam.transform.parent = initPlayerCamParent;
    }

    public void LerpCamToRef(GameCamera iRefCam, float iTime)
    {
        if (camLerpCo != null)
        {
            StopCoroutine(camLerpCo);
            camLerpCo = null;
        }
        playerCam.transform.parent = iRefCam.transform.parent;

        //Vector3 cameraRelativePos = playerCam.transform.InverseTransformPoint(iRefCam.transform.position);
        StartCoroutine(LerpCamToCo(iRefCam.transform.localPosition, iRefCam.transform.localRotation, iRefCam.cam.fieldOfView, iTime));
    }

    IEnumerator LerpCamToCo(Vector3 iRefCamPos, Quaternion iRefCamRot, float iRefCamFOV, float iTime)
    {
        float elapsedTime = 0f;

        Vector3 startPos = playerCam.transform.localPosition;
        Quaternion startRot = playerCam.transform.localRotation;
        float startFOV = playerCam.cam.fieldOfView;
        while (elapsedTime <= iTime)
        {
            float journeyFrac = elapsedTime / iTime;
            playerCam.transform.localPosition = Vector3.Lerp(startPos, iRefCamPos, journeyFrac);
            playerCam.cam.fieldOfView = Utils.Lerp(startFOV, iRefCamFOV, journeyFrac);
            playerCam.transform.localRotation = Quaternion.Lerp(startRot, iRefCamRot, journeyFrac);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
