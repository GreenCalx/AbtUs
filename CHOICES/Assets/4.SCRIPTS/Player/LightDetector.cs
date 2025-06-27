using UnityEngine;
using UnityEngine.Rendering;
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
    [Range(0,8)]
    public int ResolutionFactor = 2;
    public GameFeedback LuminanceFeedback;
    private bool GPUReqDonne = false;
    private bool isFirstPass = true;
    public float WarmUpTime = 10f;

    public int RTWidth, RTHeight;

    // CS


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
        while (playerCam.GetLumRT() == null)
        { yield return null; }
        while (!playerCam.GetLumRT().IsCreated())
        { yield return null; }

        RenderTexture lRT = playerCam.GetLumRT();
        lRT.GenerateMips();
        Vector2 v = Utils.GetMipsSize(lRT, lRT.mipmapCount - 3, true);
        Debug.Log("V.x = " + v.x + " V.y = " + v.y);

        tex = new Texture2D( 8, 8, TextureFormat.RGFloat, false);
        
        float lerpStartTime = 0f;
        float frac = 0f;
        
        
        while (true)
        {
            GPUReqDonne = false;
            //LightCamRT = playerCam.GetLumRT();
            playerCam.refreshCmdLum = true;
            while (playerCam.refreshCmdLum){ yield return null; }
            lRT = playerCam.GetLumRT();
            lRT.GenerateMips();
            Debug.Log("mimap count : " + playerCam.GetLumRT().mipmapCount);
            AsyncGPUReadback.Request(
                lRT,
                lRT.mipmapCount - 6, // mipmap
                TextureFormat.RGFloat, OnCompleteReadback);

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
        if (request.hasError)
            return;

        tex.LoadRawTextureData(request.GetData<float>());
        tex.Apply();

        // ReadPixelLuminance.SetTexture(kernelID, "inputTexture", tex);
        // ReadPixelLuminance.SetBuffer(kernelID, "outputBuffer", outputBuffer);
        // ReadPixelLuminance.SetFloat("w", tex.width);
        // ReadPixelLuminance.SetFloat("h", tex.height);

        // ReadPixelLuminance.Dispatch(kernelID, tex.width/8, tex.height/8, 1);

        // float[] outputArray = new float[tex.width*tex.height];
        // outputBuffer.GetData(outputArray);
        //Color color = new Color(outputArray[0], outputArray[1], 0, 255);

        Color32[] colors = tex.GetPixels32();

        LuminancePrev = Luminance;
        LuminanceNext = 0f;
        for (int i = 0; i < colors.Length; i++)
        {
            // https://en.wikipedia.org/wiki/Relative_luminance
            LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g);
            //LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f * colors[i].b);
            //LuminanceNext += colors[i].r;
        }
        LuminanceNext /= colors.Length;

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
