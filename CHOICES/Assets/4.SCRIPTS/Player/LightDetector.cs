using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Collections;
using System.Collections.Generic;
using static EventLog;
public class LightDetector : MonoBehaviour, IFeedbackEval
{
    public GameCamera playerCam;
    public Texture2D tex;
    private RenderTexture LightCamRT;
    private Coroutine renderCo;
    public float Luminance { private set; get; }
    private float LuminancePrev, LuminanceNext;
    public float RefreshRateInSec = 2f;
    public GameFeedback LuminanceFeedback;
    private bool GPUReqDonne = false;
    private bool isFirstPass = true;
    public float WarmUpTime = 10f;

    public RenderTexture lumRT;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LuminanceFeedback.Init(this);

        isFirstPass = true;
        Luminance = 0f;
        LuminancePrev = 0f;
        LuminanceNext = 0f;

        if (renderCo == null)
            renderCo = StartCoroutine(RenderCo());
    }

    IEnumerator RenderCo()
    {
        while (playerCam==null)
        { yield return null; }

        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        float lerpStartTime = 0f;
        float frac = 0f;
        
        while (true)
        {
            yield return new WaitForEndOfFrame();

            while (OverWorldControl.Instance.IsGTLCrossfading()) { yield return null; }

            GPUReqDonne = false;
            UpdateLuminance();
            var req = AsyncGPUReadback.Request(
                lumRT,
                lumRT.mipmapCount - 1,
                TextureFormat.RGBA32, OnCompleteReadback);

            while (!GPUReqDonne) { yield return null; }

            lerpStartTime = Time.time;
            while (Time.time - lerpStartTime < RefreshRateInSec)
            {
                frac = (Time.time - lerpStartTime) / RefreshRateInSec;
                Luminance = Utils.Lerp(LuminancePrev, LuminanceNext, frac);

                LuminanceFeedback.Refresh();
                yield return null;
            }

            Luminance = LuminanceNext;
            LuminanceFeedback.Refresh();
            yield return null;
        }
    }

    void OnDestroy()
    {
        Destroy(tex);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (!request.done)
            return;
        if (request.hasError)
            return;

        tex.LoadRawTextureData(request.GetData<uint>());
        tex.Apply(false);

        UpdateLuminance();
    }

    void UpdateLuminance()
    {
        LuminancePrev = Luminance;
        Color c = tex.GetPixel(0,0);
        LuminanceNext = c[0];

        Debug.Log(LuminanceNext);
        if (isFirstPass)
        {
            LuminancePrev = LuminanceNext;
            isFirstPass = false;
        }
        GPUReqDonne = true;
    }

    public float feedbackEvaluator()
    {
        float lumVal = Utils.Remap(Luminance, 0f, 1f, -1f, 1f);
        UIGame.Instance.DbgLightDetecRefresh(this);
        return lumVal;
    }
}
