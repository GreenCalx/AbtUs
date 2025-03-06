using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChoicePortals : MonoBehaviour
{

    [Serializable]
    public class ChoicePortalBundle
    {
        public Camera cam;
        public Material mat;
        public Transform slidingDoor;
    }

    public float doorSlideYAmount = 2f;
    public float doorSlideTime = 1f;
    public List<ChoicePortalBundle> choiceBundles = new List<ChoicePortalBundle>();

    private bool lockLeft = false;
    private bool lockRight = false;

    void Start()
    {
        choiceBundles[0].cam.gameObject.SetActive(false);
        choiceBundles[1].cam.gameObject.SetActive(false);
    }

    public void OpenLeft()
    {
        if (lockLeft)
            return;
        lockLeft = true;

        choiceBundles[0].cam.gameObject.SetActive(true);

        if (choiceBundles[0].cam.targetTexture != null)
            choiceBundles[0].cam.targetTexture.Release();

        choiceBundles[0].cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        choiceBundles[0].mat.mainTexture = choiceBundles[0].cam.targetTexture;
        
        StartCoroutine(SlideDoor(choiceBundles[0].slidingDoor, doorSlideYAmount, (result) => lockLeft = result));
    }

    public void CloseLeft()
    {
        if (lockLeft)
            return;
        lockLeft = true;

        choiceBundles[0].cam.targetTexture.Release();
        StartCoroutine(SlideDoor(choiceBundles[0].slidingDoor, -doorSlideYAmount, (result) => lockLeft = result));

        choiceBundles[0].cam.gameObject.SetActive(false);
    }

    public void OpenRight()
    {
        if (lockRight)
            return;
        lockRight = true;

        choiceBundles[1].cam.gameObject.SetActive(true);

        if (choiceBundles[1].cam.targetTexture != null)
            choiceBundles[1].cam.targetTexture.Release();

        choiceBundles[1].cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        choiceBundles[1].mat.mainTexture = choiceBundles[1].cam.targetTexture;

        StartCoroutine(SlideDoor(choiceBundles[1].slidingDoor,doorSlideYAmount, (result) => lockRight = result));
    }

    public void CloseRight()
    {
        if (lockRight)
            return;
        lockRight = true;
        
        choiceBundles[1].cam.targetTexture.Release();
        StartCoroutine(SlideDoor(choiceBundles[1].slidingDoor, -doorSlideYAmount, (result) => lockRight = result ));

        choiceBundles[0].cam.gameObject.SetActive(false);
    }

    IEnumerator SlideDoor(Transform iDoor, float iYOffset, System.Action<bool> mutexCallback)
    {
        float frac = 0f;
        Vector3 startPos = iDoor.localPosition;
        Vector3 targetPos = new Vector3(iDoor.localPosition.x, iDoor.localPosition.y + iYOffset, iDoor.localPosition.z );

        while (frac < 1)
        {
            frac += Time.deltaTime / doorSlideTime;
            if (frac>1) { frac = 1; }
            iDoor.localPosition = Vector3.Lerp(startPos, targetPos, frac);
            yield return null;
        }

        iDoor.localPosition = targetPos;
        
        mutexCallback(false);
    }
}
