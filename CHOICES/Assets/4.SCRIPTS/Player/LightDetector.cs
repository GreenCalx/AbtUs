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
    public GameFeedback LuminanceFeedback;
    private bool GPUReqDonne = false;
    private bool isFirstPass = true;
    public float WarmUpTime = 10f;
    public int mipLevel = 7;
    private Color32[] colors;

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


        int tex_w = playerCam.GetLumRT().width >> mipLevel;
        int tex_h = playerCam.GetLumRT().height >> mipLevel;
        tex = new Texture2D(tex_w, tex_h, TextureFormat.RGBA32, false);
        colors = new Color32[tex_w * tex_h];

        float lerpStartTime = 0f;
        float frac = 0f;
        
        while (true)
        {
            GPUReqDonne = false;
            AsyncGPUReadback.Request(
                playerCam.GetLumRT(),
                mipLevel,
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
        if (request.hasError)
            return;

        //Graphics.Blit(playerCam.GetLumRT(), tex);

        //tex.ReadPixels(new Rect(0, 0, playerCam.GetLumRT().width, playerCam.GetLumRT().height), 0, 0);
        //tex = new Texture2D( playerCam.GetLumRT().width, playerCam.GetLumRT().height, TextureFormat.RGBA32, false);
        tex.LoadRawTextureData(request.GetData<uint>());
        tex.Apply(true);
        //tex.desiredMipmapLevel = 5;
        // ReadPixelLuminance.SetTexture(kernelID, "inputTexture", tex);
        // ReadPixelLuminance.SetBuffer(kernelID, "outputBuffer", outputBuffer);
        // ReadPixelLuminance.SetFloat("w", tex.width);
        // ReadPixelLuminance.SetFloat("h", tex.height);

        // ReadPixelLuminance.Dispatch(kernelID, tex.width/8, tex.height/8, 1);

        // float[] outputArray = new float[tex.width*tex.height];
        // outputBuffer.GetData(outputArray);
        //Color color = new Color(outputArray[0], outputArray[1], 0, 255);


        //StartCoroutine(UpdateLuminance());
        UpdateLuminance();
    }

    void UpdateLuminance()
    {
        //while (!tex.IsRequestedMipmapLevelLoaded()) { yield return null; }
        
        colors = tex.GetPixels32();

        LuminancePrev = Luminance;
        LuminanceNext = 0f;
        for (int i = 0; i < colors.Length; i++)
        {
            // https://en.wikipedia.org/wiki/Relative_luminance
            //LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g);
            LuminanceNext += (0.2126f * colors[i].r) + (0.7152f * colors[i].g) + (0.0722f * colors[i].b);
            //LuminanceNext += colors[i].r;
        }
        LuminanceNext /= colors.Length;

        LuminanceNext = Utils.Remap(LuminanceNext, 0f, 80f, 0f, 1f);
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
        float lumVal = Utils.Remap(Luminance, 0f, 1f, -1f, 1f);
        UIGame.Instance.DbgLightDetecRefresh(this);
        //Debug.Log(Luminance + " : " + lumVal);
        // if (Luminance < 0.5f)
        //     return -(1f-Luminance);
        return lumVal;
    }
}
