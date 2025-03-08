using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChoicePortals : MonoBehaviour
{

    [Serializable]
    public class ChoicePortalBundle
    {
        [Header("Mand refs")]
        public Camera cam;
        public Material mat;
        public Transform slidingDoor;
        [Header("Internals")]
        public bool shouldOpen = false;
        public Vector3 openedLocalPos = Vector3.zero;
        public Vector3 closedLocalPos = Vector3.zero;
        public float openeningFrac = 0f;
        public Coroutine doorSlideCo;

        public bool IsOperating() { return ((openeningFrac >= 0.1f) && (openeningFrac <= 0.99f)); }

        public void Show() 
        {
            cam.gameObject.SetActive(true);
            if (cam.targetTexture != null)
                cam.targetTexture.Release();
            cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            mat.mainTexture = cam.targetTexture;
        }

        public void UnShow()
        {
            cam.targetTexture.Release();
            cam.gameObject.SetActive(false);
        }

        public bool IsOpen() { return openeningFrac >= 1f; }
        public bool IsClose() { return openeningFrac <= 0f; }
    }

    public Vector3 doorSlideAmount = new Vector3(0f,2f,0f);
    public float doorSlideTime = 1f;
    public List<ChoicePortalBundle> choiceBundles = new List<ChoicePortalBundle>();


    void Start()
    {
        foreach(ChoicePortalBundle b in choiceBundles)
        {
            b.cam.gameObject.SetActive(false);
            b.closedLocalPos = b.slidingDoor.localPosition;
            b.openedLocalPos = b.slidingDoor.localPosition + doorSlideAmount;
            b.openeningFrac = 0f;
        }
    }

    public void TryOpenLeft()
    {
        if (choiceBundles[0].shouldOpen)
            return ;

        if (choiceBundles[0].IsOperating())
        { 
            StopCoroutine(choiceBundles[0].doorSlideCo);
        }

        choiceBundles[0].shouldOpen = true;

        choiceBundles[0].Show();
        
        choiceBundles[0].doorSlideCo = StartCoroutine(SlideDoor(choiceBundles[0], () => {}));
    }

    public void TryCloseLeft()
    {
        if (!choiceBundles[0].shouldOpen)
            return ;
        if (choiceBundles[0].IsOperating())
        { StopCoroutine(choiceBundles[0].doorSlideCo); }

        choiceBundles[0].shouldOpen = false;
        choiceBundles[0].doorSlideCo = StartCoroutine(SlideDoor(choiceBundles[0], () => choiceBundles[0].UnShow()));
        
    }

    public void TryOpenRight()
    {
        if (choiceBundles[1].shouldOpen)
            return ;
        if (choiceBundles[1].IsOperating())
        { StopCoroutine(choiceBundles[1].doorSlideCo); }

        choiceBundles[1].shouldOpen = true;
        choiceBundles[1].Show();
        choiceBundles[1].doorSlideCo = StartCoroutine(SlideDoor(choiceBundles[1], () => {}));

    }

    public void TryCloseRight()
    {
        if (!choiceBundles[1].shouldOpen)
            return ;
        if (choiceBundles[1].IsOperating())
        { StopCoroutine(choiceBundles[1].doorSlideCo); }

        choiceBundles[1].shouldOpen = false;
        choiceBundles[1].doorSlideCo = StartCoroutine(SlideDoor(choiceBundles[1], () => choiceBundles[1].UnShow() ));
    }

    IEnumerator SlideDoor(ChoicePortalBundle iBundle, System.Action mutexCallback)
    {
        float frac = iBundle.shouldOpen ? iBundle.openeningFrac : 1f - iBundle.openeningFrac;
        Vector3 startPos    = iBundle.slidingDoor.localPosition;
        Vector3 targetPos   = iBundle.shouldOpen ? iBundle.openedLocalPos : iBundle.closedLocalPos ;

        while (frac < 1)
        {
            frac += Time.deltaTime / doorSlideTime;
            if (frac>1) { frac = 1; }
            iBundle.openeningFrac = iBundle.shouldOpen ? frac : 1f - frac;

            iBundle.slidingDoor.localPosition = Vector3.Lerp(startPos, targetPos, frac);
            yield return null;
        }

        iBundle.slidingDoor.localPosition = targetPos;
        iBundle.openeningFrac = iBundle.shouldOpen ? 1f : 0f;
        
        mutexCallback();
    }

    public bool OneDoorOpened()
    {
        return choiceBundles[0].IsOpen() ^ choiceBundles[1].IsOpen();
    }
}
