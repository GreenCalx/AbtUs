using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using System.Linq;

public class OverWorldControl : MonoBehaviour
{
    [Header("Debug")]
    public bool debug = true;
    public bool applyForcedControls = false;
    [Range(0f,1f)]
    public float ForceMineralToOrganic = 0f;
    [Range(0f,1f)]
    public float ForceGloomyToLush = 0f;
    [Range(0f,1f)]
    public float ForceOrderToChaos = 0f;
    [Header("Init")]
    [Range(0f,1f)]
    public float Init_GloomyToLush = 0.5f;
    [Range(0f,1f)] 
    public float Init_MineralToOrganic = 0.5f;
    [Range(0f,1f)]
    public float Init_OrderToChaos = 0.5f;
    
    [Header("Control")]
    private float m_GloomyToLush;
    [Range(0f,1f)]
    public float GloomyToLush
    {
        get { return m_GloomyToLush; }
        set {
            if (m_GloomyToLush==value)
                return;
            SetGloomyToLush(value);
        }
    }

    private float m_MineralToOrganic;
    [Range(0f,1f)] 
    public float MineralToOrganic
    {
        get { return m_MineralToOrganic; }
        set {
            if (m_MineralToOrganic==value)
                return;
            SetMineralToOrganic(value);
        }
    }

    private float m_OrderToChaos;
    [Range(0f,1f)]
    public float OrderToChaos
    {
        get { return m_OrderToChaos; }
        set {
            if (m_OrderToChaos==value)
                return;
            SetOrderToChaos(value);
        }
    }
    [Header("LookUpTables")]
    public GTLLookupTable gtlLookupTable;
    public MTOLookupTable mtoLookupTable;
    public OTCLookupTable otcLookupTable;

    [Header("Modifier Manual References")]
    public GTLVolumeMod MainGTL_A;
    public GTLVolumeMod MainGTL_B;
    public GTLLightMod MainSunGTL;
    [Header("Modifier Auto References")]
    public List<GTLVolumeMod> GTLExtraVolMods;
    public List<GTLLightMod> GTLLightMods;
    public List<MTOModifier> mtoModifiers;
    public List<OTCModifier> otcModifiers;
    public List<OTCCluster> otcClusters;

    public List<OWCListener> OTCListeners;
    public List<OWCListener> MTOListeners;
    public List<OWCListener> GTLListeners;

    [Header("Modifier tweaks")]
    public float gtlCrossfadeTime = 10f;

    [Header("Internals")]
    private Coroutine gtlCrossfadeVolCo;
    private Coroutine gtlCrossfadeSunsCo;
    private bool crossfadingVolDone = true;
    private bool crossfadingSunDone = true;
    private static OverWorldControl instance = null;
    public static OverWorldControl Instance => instance;

    #region UNITY
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        SetGloomyToLush(Init_GloomyToLush);
        SetMineralToOrganic(Init_MineralToOrganic);
        SetOrderToChaos(Init_OrderToChaos);

        GTLExtraVolMods = new List<GTLVolumeMod>();
        GTLLightMods = new List<GTLLightMod>();
        if (gtlLookupTable==null)
        { gtlLookupTable = GetComponentInChildren<GTLLookupTable>(); }

        mtoModifiers = new List<MTOModifier>();
        if (mtoLookupTable==null)
        { mtoLookupTable = GetComponentInChildren<MTOLookupTable>(); }

        otcModifiers = new List<OTCModifier>();
        otcClusters = new List<OTCCluster>();
        if (otcLookupTable==null)
        { otcLookupTable = GetComponentInChildren<OTCLookupTable>(); }

        if (debug)
        {
            ForceMineralToOrganic = Init_MineralToOrganic;
            ForceOrderToChaos = Init_OrderToChaos;
            ForceGloomyToLush = Init_GloomyToLush;
        }

    }

    void Update()
    {
        if (debug && applyForcedControls)
        {
            if (ForceMineralToOrganic!=MineralToOrganic)
            {
                MineralToOrganic = ForceMineralToOrganic;
            }

            if (OrderToChaos!=ForceOrderToChaos)
            {
                OrderToChaos = ForceOrderToChaos;
            }

            if (GloomyToLush!=ForceGloomyToLush)
            {
                GloomyToLush = ForceGloomyToLush;
            }

            applyForcedControls = false;
        }
    }

    #endregion

    #region LISTENER

    public void SubscribeListener(OWCListener listener, OWCListener.AXIS axis)
    {
        if (axis == OWCListener.AXIS.gtl) { GTLListeners.Add(listener); }
        if (axis == OWCListener.AXIS.otc) { OTCListeners.Add(listener); }
        if (axis == OWCListener.AXIS.mto) { MTOListeners.Add(listener); }
    }

    #endregion LISTENER
    #region GTL
    public void SetGloomyToLush(float iVal)
    {
        m_GloomyToLush = Mathf.Clamp(iVal, 0f, 1f);
        RefreshGTLMods();
    }
    public void SubscribeGTL<T,K>(GTLModifier<T,K> iGTLMod)
    {
        if (iGTLMod is GTLVolumeMod)
        {
            GTLVolumeMod asVol = iGTLMod as GTLVolumeMod;
            if (asVol.gtlType == GTL_TYPE.MAIN)
            {
                if      (MainGTL_A==null)   { MainGTL_A = asVol; }
                else if (MainGTL_B==null)   { MainGTL_B = asVol; }
            }
            else if (!GTLExtraVolMods.Contains(asVol))
            {
                GTLExtraVolMods.Add(asVol);
            }
        }
        else if (iGTLMod is GTLLightMod)
        {
            GTLLightMod asLight = iGTLMod as GTLLightMod;

            if (asLight.gtlType == GTL_TYPE.SUN)
            {
                if (MainSunGTL==null)
                    MainSunGTL = asLight;
            } 
            else if (asLight.gtlType == GTL_TYPE.LIGHT)
            {
                if (!GTLLightMods.Contains(asLight))
                {
                    GTLLightMods.Add(asLight);
                }
            }

        }

    }
    public void RefreshGTLMods()
    {
        if (crossfadingVolDone)
        {
            if (gtlCrossfadeVolCo!=null)
            {
                StopCoroutine(gtlCrossfadeVolCo);
                gtlCrossfadeVolCo = null;
            }

            if (MainGTL_A.isActive)
            {
                if (gtlLookupTable.TryUpdateProfile(MainGTL_B, MainGTL_A.modifierTarget.sharedProfile, GloomyToLush))
                {
                    gtlCrossfadeVolCo = StartCoroutine(CrossfadeVolCo(gtlCrossfadeTime, MainGTL_A, MainGTL_B));
                }
            }
            else if (MainGTL_B.isActive)
            {
                if (gtlLookupTable.TryUpdateProfile(MainGTL_A, MainGTL_B.modifierTarget.sharedProfile, GloomyToLush))
                {
                    gtlCrossfadeVolCo = StartCoroutine(CrossfadeVolCo(gtlCrossfadeTime, MainGTL_B, MainGTL_A));
                }
            }
        }

        // Sun
        if (crossfadingSunDone)
        {
            if (gtlLookupTable.TryUpdateSun(MainSunGTL, MainSunGTL.modifierTarget, GloomyToLush))
            {
                if (gtlCrossfadeSunsCo!=null)
                {
                    StopCoroutine(gtlCrossfadeSunsCo);
                    gtlCrossfadeSunsCo = null;
                }
                gtlCrossfadeSunsCo = StartCoroutine(CrossfadeSunsCo(gtlCrossfadeTime));
            }
                        
        }

        // Lights
        foreach (GTLLightMod mod in GTLLightMods)
        {

        }

        // Extra volumes
        foreach ( GTLVolumeMod mod in GTLExtraVolMods)
        {
            if (mod.gtlType == GTL_TYPE.MAIN)
                 continue;
            // Update extra volumes
        }

        foreach( OWCListener listener in GTLListeners)
        {
            listener.Call(GloomyToLush);
        }
    }

    public IEnumerator CrossfadeSunsCo(float iCrossfadeTime)
    {
        crossfadingSunDone = false;
        float elapsedTime = 0f;
        while ( elapsedTime < iCrossfadeTime )
        {
            elapsedTime += Time.deltaTime;
            float frac = elapsedTime / iCrossfadeTime;
            //iFrom.weight = 1f - frac;
            MainSunGTL.weight = frac;
            
            yield return null;
        }
        crossfadingSunDone = true;
    }

    public IEnumerator CrossfadeVolCo(float iCrossfadeTime, GTLVolumeMod iFrom, GTLVolumeMod iTo)
    {
        crossfadingVolDone = false;
        float elapsedTime = 0f;
        while ( elapsedTime < iCrossfadeTime )
        {
            elapsedTime += Time.deltaTime;
            float frac = elapsedTime / iCrossfadeTime;
            iFrom.weight = 1f - frac;
            iTo.weight = frac;
            yield return null;
        }
        iFrom.Deactivate();
        iTo.Activate();
        crossfadingVolDone = true;
    }
    #endregion

    #region MTO
    public void SetMineralToOrganic(float iVal)
    {
        m_MineralToOrganic = Mathf.Clamp(iVal, 0f, 1f);

        RefreshMTOMods();
    }
    public void SubscribeMTO(MTOModifier iMTOMod)
    {
        if (!mtoModifiers.Contains(iMTOMod))
        {
            mtoModifiers.Add(iMTOMod);
        }
    }

    public void RefreshMTOMods()
    {
        foreach(MTOModifier mod in mtoModifiers)
        {
            Dictionary<Material, Material> operations = new Dictionary<Material, Material>();

            foreach(Material mat in mod.currMats)
            {
                Material newMat = mtoLookupTable.ScoutForMatChange(mat, MineralToOrganic);
                if (newMat==null)
                    continue;
                operations.Add(mat, newMat);
            }
            foreach(Material m in operations.Keys)
            {
                ChangeModMaterial(mod, m, operations[m]);
            }

            mod.RefreshMaterials();
        }

        foreach (OWCListener listener in MTOListeners)
        {
            listener.Call(MineralToOrganic);
        }
    }
    private void ChangeModMaterial(MTOModifier iMod, Material iOldMat, Material iNewMat)
    {
        iMod.ChangeMaterial (iOldMat, iNewMat);
    }
    #endregion

    #region OTC
    public void SetOrderToChaos(float iVal)
    {
        m_OrderToChaos = Mathf.Clamp(iVal, 0f, 1f);

        RefreshOTCMods();
    }
    public void SubscribeOTC(OTCModifier iOTCMod)
    {
        if (!otcModifiers.Contains(iOTCMod))
        {
            otcModifiers.Add(iOTCMod);
        }
    }
    public void SubscribeOTCCluster(OTCCluster iCluster)
    {
        if (!otcClusters.Contains(iCluster))
        {
            otcClusters.Add(iCluster);
        }
    }
    public void RefreshOTCMods()
    {
        // Make clusters?
        foreach( OTCModifier mod in otcModifiers)
        {
            if (mod.cluster!=null)
                continue;
            
            // can be added to existing cluster ?
            bool addedToACluster = false;
            foreach(OTCCluster c in otcClusters)
            {
                if (c.CanBeAddedToCluster(mod))
                {
                    c.AddToCluster(mod);
                    addedToACluster = true;
                }
            }
            if (addedToACluster)
                continue;

            // create a new one otherwise
            OTCCluster newCluster = new OTCCluster();
            newCluster.AddToCluster(mod);
            otcClusters.Add(newCluster);
        }

        // Activate clusters
        otcClusters.ForEach( c => c.spread(otcLookupTable, m_OrderToChaos));


        foreach (OWCListener listener in OTCListeners)
        {
            listener.Call(OrderToChaos);
        }
    }
    #endregion
}
