using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightDetector : MonoBehaviour, IFeedbackEval
{
    private RenderTexture SampledScreenRT;
    private Texture2D tex;
    private Coroutine renderCo;
    public float Luminance { private set; get; }
    private float LuminancePrev, LuminanceNext;
    public float RefreshRateInSec = 2f;
    public GameFeedback LuminanceFeedback;
    private bool GPUReqDonne = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LuminanceFeedback.Init(this);
        if (renderCo == null)
            renderCo = StartCoroutine(RenderCo());
    }

    IEnumerator RenderCo()
    {
        tex = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        SampledScreenRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        float lerpStartTime = 0f;
        float frac = 0f;
        float LuminanceFrom = Luminance;
        float LuminanceTo   = Luminance;
        while (true)
        {
            ScreenCapture.CaptureScreenshotIntoRenderTexture(SampledScreenRT);
            GPUReqDonne = false;
            AsyncGPUReadback.Request(SampledScreenRT, 0, TextureFormat.ARGB32, OnCompleteReadback);

            while(!GPUReqDonne) { yield return null; }

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
        SampledScreenRT.Release();
        Destroy(tex);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
            return;

        tex.LoadRawTextureData(request.GetData<uint>());
        tex.Apply();

        Color32[] colors = tex.GetPixels32();
        LuminancePrev = Luminance;
        LuminanceNext = 0f;
        for (int i = 0; i < colors.Length; i++)
        {
            // https://en.wikipedia.org/wiki/Relative_luminance
            LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f + colors[i].b);
        }
        LuminanceNext /= (Screen.width * Screen.height);
        LuminanceNext = Utils.Remap(LuminanceNext, 0f, 150f, 0f, 1f);

        GPUReqDonne = true;
        //LuminanceFeedback.fData.baseValue = Luminance;

    }

    public float feedbackEvaluator()
    {
        //  chain root doesn't count towards chaos
        float lumVal = Utils.Remap( Luminance, 0f, 1f, -1f, 1f);
        UIGame.Instance.DbgLightDetecRefresh(this);
        //Debug.Log(Luminance + " : " + lumVal);
        // if (Luminance < 0.5f)
        //     return -(1f-Luminance);
        return lumVal;
    }
}
