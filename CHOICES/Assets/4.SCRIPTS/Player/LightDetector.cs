using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using static EventLog;
public class LightDetector : MonoBehaviour, IFeedbackEval
{
    public Camera lightCam;
    public Texture2D tex;
    private RenderTexture LightCamRT;
    private Coroutine renderCo;
    public float Luminance { private set; get; }
    private float LuminancePrev, LuminanceNext;
    public float RefreshRateInSec = 2f;
    [Range(0,8)]
    public int ResolutionFactor = 2;
    public GameFeedback LuminanceFeedback;
    private bool GPUReqDonne = false;
    private bool isFirstPass = true;
    public float WarmUpTime = 10f;

    public int RTWidth, RTHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LuminanceFeedback.Init(this);

        isFirstPass = true;
        Luminance = 0f;
        LuminancePrev = 0f;
        LuminanceNext = 0f;
        INFO(" LightDetector RT dimensions : " + RTWidth + "x" + RTHeight);

        if (renderCo == null)
            renderCo = StartCoroutine(RenderCo());
    }

    IEnumerator RenderCo()
    {
        float warmup = Time.time;
        while ((Time.time - warmup ) < WarmUpTime ) { yield return null; }

        LightCamRT = lightCam.activeTexture;
        RTWidth = LightCamRT.width;
        RTHeight = LightCamRT.height ;
        tex = new Texture2D(  RTWidth, RTHeight, TextureFormat.R8, false);
        
        float lerpStartTime = 0f;
        float frac = 0f;
        while (true)
        {
            GPUReqDonne = false;
            AsyncGPUReadback.Request(
                LightCamRT,
                0, // mipmap
                TextureFormat.R8, OnCompleteReadback);

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

        Destroy(tex);
    }

    void OnCompleteReadback(AsyncGPUReadbackRequest request)
    {
        if (request.hasError)
            return;

         tex.LoadRawTextureData(request.GetData<uint>());
         tex.Apply();
        //Graphics.Blit(LightCamRT, tex);
        Color32[] colors = tex.GetPixels32();

        LuminancePrev = Luminance;
        LuminanceNext = 0f;
        for (int i = 0; i < colors.Length; i++)
        {
            // https://en.wikipedia.org/wiki/Relative_luminance
            //LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f * colors[i].b);
            LuminanceNext += colors[i].r;
        }
        LuminanceNext /= (RTWidth * RTHeight);

        LuminanceNext = Utils.Remap(LuminanceNext, 0f, 100f, 0f, 1f);
        if (isFirstPass)
        {
            LuminancePrev = LuminanceNext;
            isFirstPass = false;
        }
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
