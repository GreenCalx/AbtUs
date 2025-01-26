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

    [Header("Modifier References")]
    public GTLModifier MainGTL_A;
    public GTLModifier MainGTL_B;
    public List<GTLModifier> extraGTLMods;
    public List<MTOModifier> mtoModifiers;
    public List<OTCModifier> otcModifiers;
    public List<OTCCluster> otcClusters;
    [Header("Modifier tweaks")]
    public float gtlCrossfadeTime = 10f;

    [Header("Internals")]
    private Coroutine gtlCrossfadeCo;
    private bool crossfadingDone = true;
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

        extraGTLMods = new List<GTLModifier>();
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

    #region GTL
    public void SetGloomyToLush(float iVal)
    {
        m_GloomyToLush = Mathf.Clamp(iVal, 0f, 1f);
        RefreshGTLMods();
    }
    public void SubscribeGTL(GTLModifier iGTLMod)
    {
        if (!extraGTLMods.Contains(iGTLMod))
        {
            extraGTLMods.Add(iGTLMod);
        }
    }
    public void RefreshGTLMods()
    {
        if (crossfadingDone)
        {
            if (gtlCrossfadeCo!=null)
            {
                StopCoroutine(gtlCrossfadeCo);
                gtlCrossfadeCo = null;
            }

            if (MainGTL_A.isActive)
            {
                if (gtlLookupTable.TryUpdateProfile(MainGTL_B, MainGTL_A.volume.sharedProfile, GloomyToLush))
                {
                    gtlCrossfadeCo = StartCoroutine(CrossfadeVolCo(MainGTL_A, MainGTL_B));
                }
            }
            else if (MainGTL_B.isActive)
            {
                if (gtlLookupTable.TryUpdateProfile(MainGTL_A, MainGTL_B.volume.sharedProfile, GloomyToLush))
                {
                    gtlCrossfadeCo = StartCoroutine(CrossfadeVolCo(MainGTL_B, MainGTL_A));
                }
            }
        }

        // extra volumes
        // TODO
        // foreach (GTLModifier mod in extraGTLMods)
        // {
        //     // Main volumes
        //     if (mod.volumeType == extraGTLMods.VOL_TYPE.MAIN)
        //     {
        //         if (mod.isActive)
        //             continue;
        //         if (gtlLookupTable.TryUpdateProfile(mod, GloomyToLush))
        //         {
        //             if (mod)
        //         }
        //     }
    }

    public IEnumerator CrossfadeVolCo(GTLModifier iFrom, GTLModifier iTo)
    {
        crossfadingDone = false;
        float elapsedTime = 0f;
        while ( elapsedTime < gtlCrossfadeTime )
        {
            elapsedTime += Time.deltaTime;
            float frac = elapsedTime / gtlCrossfadeTime;
            iFrom.weight = 1f - frac;
            iTo.weight = frac;
            yield return null;
        }
        iFrom.isActive = false;
        iTo.isActive = true;
        crossfadingDone = true;
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

    }
    #endregion
}
