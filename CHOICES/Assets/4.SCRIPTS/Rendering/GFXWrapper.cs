using System.Collections.Generic;
using UnityEngine;
using static Constants;


public class GFXWrapper : MonoBehaviour
{
    [Header("Mand Refs")] 
    public Renderer targetRenderer;
    
    [Header("Internals")]
    public MatDefSOCollection matCollection;
    public List<MatDefSO> initMats;
    public List<Material> targetMats;
    public bool isMixed = false;
    private bool lerp_done = true;
    private float lerpTime = 10f;
    private float elapsedLerpTime = 0f;
    private bool asleep = false;

    public void InitShader()
    {
        matCollection = OverWorldControl.Instance.mtoLookupTable.matCollection;
        targetMats = new List<Material>(targetRenderer.materials);
        initMats.Clear();
        foreach (Material mat in targetMats)
        {
            matCollection.TryAddMat(mat, mat.name);
            initMats.Add(matCollection.GetMatDefFromName(mat.name));
        }

        lerp_done = true;

        refreshIsMixed();
        SetChaos(OverWorldControl.Instance.ChaosMagnitude);
        
        asleep = false;
    }

    public bool IsAvailable()
    {
        return lerp_done && !asleep;
    }
    
    public void Sleep(bool iState) { asleep = iState; }

    public bool ChangeMatText(List<MatDefSO> iNewMatDefs, float iCrossfadeTime)
    {
        if (!lerp_done)
            return false;

        refreshIsMixed();
        foreach (Material mat in targetMats)
        {
            if (iNewMatDefs[targetMats.IndexOf(mat)] == null)
            {
                // Unchanged material
                Debug.LogWarning("GFXWrapper: ChangeMatText called but there is null values in entry.");
                continue;
            }
            if (isMixed)
            {
                SetBaseMaterial(mat, iNewMatDefs[targetMats.IndexOf(mat)]);
            }
            else
            {
                SetMixedMaterial(mat, iNewMatDefs[targetMats.IndexOf(mat)]);
            }
        }


        CrossfadeToNewMatText(iCrossfadeTime);
        return true;
    }

    private void CrossfadeToNewMatText(float iCrossfadeTime)
    {
        float targetLerpVal = isMixed ? 0f : 1f;
        if (iCrossfadeTime < 0)
        {
            foreach(Material targetMat in targetMats)
                targetMat.SetFloat(shad_lerpParm, targetLerpVal);
            lerp_done = true;
            return;
        }
        lerp_done = false;
        lerpTime = iCrossfadeTime;
        elapsedLerpTime = 0f;
    }

    public void SetChaos(float iVal)
    {
        foreach(Material targetMat in targetMats)
            targetMat.SetFloat(shad_chaosParm, iVal);
    }

    public void SetShatter(float iVal)
    {
        foreach (Material targetMat in targetMats)
            targetMat.SetFloat(shad_shatterParm, iVal);
    }

    private void refreshIsMixed()
    {
        bool b = true;
        foreach (Material targetMat in targetMats)
            b &= targetMat.GetFloat(shad_lerpParm) >= 1f;
        isMixed = b;
    }

    private void SetBaseMaterial(Material iMat, MatDefSO iNewMatDef)
    {
        iMat.SetTexture(shad_baseAlbedo,iNewMatDef.albedo );
        iMat.SetTexture(shad_baseNormal,iNewMatDef.normals );
        iMat.SetTexture(shad_baseHeight,iNewMatDef.height );
        iMat.SetTexture(shad_baseMask  ,iNewMatDef.mask );
        
        iMat.SetFloat(shad_baseMinMetallicRemap    ,iNewMatDef.minMetallicRemap);
        iMat.SetFloat(shad_baseMaxMetallicRemap    ,iNewMatDef.maxMetallicRemap);
        iMat.SetFloat(shad_baseMinSmoothnessRemap  ,iNewMatDef.minSmoothnessRemap);
        iMat.SetFloat(shad_baseMaxSmoothnessRemap  ,iNewMatDef.maxSmoothnessRemap);
    }

    private void SetMixedMaterial(Material iMat,MatDefSO iNewMatDef)
    {
        iMat.SetTexture(shad_MixedAlbedo,iNewMatDef.albedo );
        iMat.SetTexture(shad_MixedNormal,iNewMatDef.normals );
        iMat.SetTexture(shad_MixedHeight,iNewMatDef.height );
        iMat.SetTexture(shad_MixedMask  ,iNewMatDef.mask );

        iMat.SetFloat(shad_MixedMinMetallicRemap    ,iNewMatDef.minMetallicRemap);
        iMat.SetFloat(shad_MixedMaxMetallicRemap    ,iNewMatDef.maxMetallicRemap);
        iMat.SetFloat(shad_MixedMinSmoothnessRemap  ,iNewMatDef.minSmoothnessRemap);
        iMat.SetFloat(shad_MixedMaxSmoothnessRemap  ,iNewMatDef.maxSmoothnessRemap);
    }

    void Update()
    {
        if (!lerp_done)        
        {
            elapsedLerpTime += Time.deltaTime;

            if (targetRenderer.isVisible)
            {
                float frac = isMixed ?
                    1f - (elapsedLerpTime / lerpTime) :
                    elapsedLerpTime / lerpTime;
                    frac = Mathf.Clamp01(frac);
                foreach(Material targetMat in targetMats)
                    targetMat.SetFloat(shad_lerpParm, frac);
            }
            lerp_done = elapsedLerpTime >= lerpTime ;  
        }
    }
}
