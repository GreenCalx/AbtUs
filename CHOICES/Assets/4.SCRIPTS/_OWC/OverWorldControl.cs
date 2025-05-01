using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Mathematics;

using System.Linq;

public enum OWCAxis {GTL, OTC, MTO};

public enum WORLD_AXIS { ZERO=0, CHAOS=1, ORDER=2, MINERAL=3, ORGANIC=4, GLOOMY=5, LUSH=6}
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
    public float ChaosMagnitude
    {   // clamp inner val to slide window min
        get { return math.remap(0.5f,1f,0f,1f, Mathf.Clamp(m_OrderToChaos,0.5f,1f)); }
    }
    public float OrderMagnitude
    {   // just contain overflow with clamp for mins
        get { return math.remap(0.5f,0f,0f,1f, Mathf.Clamp(m_OrderToChaos,0f,0.5f)); }
    }
    public float OrganicMagnitude
    {   // clamp inner val to slide window min
        get { return math.remap(0.5f,1f,0f,1f, Mathf.Clamp(m_MineralToOrganic,0.5f,1f)); }
    }
    public float MineralMagnitude
    {   // just contain overflow with clamp for mins
        get { return math.remap(0.5f,0f,0f,1f, Mathf.Clamp(m_MineralToOrganic,0f,0.5f)); }
    }
    public float LushMagnitude
    {   // clamp inner val to slide window min
        get { return math.remap(0.5f,1f,0f,1f, Mathf.Clamp(m_GloomyToLush,0.5f,1f)); }
    }
    public float GloomyMagnitude
    {   // just contain overflow with clamp for mins
        get { return math.remap(0.5f,0f,0f,1f, Mathf.Clamp(m_GloomyToLush,0f,0.5f)); }
    }

    public float getAxisValue(OWCAxis axis)
    {
        if(axis == OWCAxis.GTL) { return GloomyToLush; }
        else if(axis == OWCAxis.MTO) { return MineralToOrganic; }
        return OrderToChaos;
    }
    public void setAxisValue(OWCAxis axis, float value)
    {
 
        if (axis == OWCAxis.GTL) { SetGloomyToLush(value); Debug.Log("GTL = " + value); }
        else if (axis == OWCAxis.MTO) { SetMineralToOrganic(value); Debug.Log("MTO = " + value); }
        else { SetOrderToChaos(value); Debug.Log("OTC = " + value); }
    }

    public float GetAxisMagnitude(WORLD_AXIS iAxis)
    {
        float retval = 0f;
        switch (iAxis)
        {
            case WORLD_AXIS.MINERAL:
                retval= MineralMagnitude;
                break;
            case WORLD_AXIS.ORGANIC:
                retval= OrganicMagnitude;
                break;
            case WORLD_AXIS.GLOOMY:
                retval = GloomyMagnitude;
                break;
            case WORLD_AXIS.LUSH:
                retval = LushMagnitude;
                break;
            case WORLD_AXIS.CHAOS:
                retval = ChaosMagnitude;
                break;
            case WORLD_AXIS.ORDER:
                retval = OrderMagnitude;
                break;
            default:
                break;
        }
        return retval;
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
    public List<MTOTerrain> mtoTerrains;
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
        
        /*
        SetGloomyToLush(Init_GloomyToLush);
        SetMineralToOrganic(Init_MineralToOrganic);
        SetOrderToChaos(Init_OrderToChaos);
        */

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

    public void SubscribeListener(OWCListener listener, WORLD_AXIS axis)
    {
        if ((axis == WORLD_AXIS.ORGANIC)||(axis == WORLD_AXIS.MINERAL)) { MTOListeners.Add(listener); }
        if ((axis == WORLD_AXIS.GLOOMY)||(axis == WORLD_AXIS.LUSH))  { GTLListeners.Add(listener); }
        if ((axis == WORLD_AXIS.CHAOS)||(axis == WORLD_AXIS.ORDER))  { OTCListeners.Add(listener); }
    }
    #endregion LISTENER

    #region GTL
    public void SetGloomyToLush(float iVal)
    {
        m_GloomyToLush = Mathf.Clamp(iVal, 0f, 1f);
        RefreshGTLMods();
        Managers.Instance.Sound?.UpdateBGM();
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
            listener.Call();
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
        iTo.Activate();

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

        iFrom.isActive = false;
        iTo.isActive = true;
        crossfadingVolDone = true;
    }

    public bool GTLIsZero()
    {
        return ( !(GloomyMagnitude>0f) && !(LushMagnitude>0f) );
    }
    #endregion

    #region MTO
    public void SetMineralToOrganic(float iVal)
    {
        m_MineralToOrganic = Mathf.Clamp(iVal, 0f, 1f);

        RefreshMTOMods();
        Managers.Instance.Sound?.UpdateBGM();
    }
    public void SubscribeMTO(MTOModifier iMTOMod)
    {
        if (!mtoModifiers.Contains(iMTOMod))
        {
            mtoModifiers.Add(iMTOMod);
        }
    }

    public void SubscribeMTOTerrain(MTOTerrain iTerrain)
    {
        if (!mtoTerrains.Contains(iTerrain))
        {
            mtoTerrains.Add(iTerrain);
        }        
    }

    public void RefreshMTOMods()
    {
        if (MTOIsZero())
        {
            foreach(MTOModifier mod in mtoModifiers) { mod.ResetMaterials(); }
            foreach(MTOTerrain t in mtoTerrains) { t.ResetLayers(); }
            return;
        }

        foreach(MTOModifier mod in mtoModifiers)
        {
            if (!mod.lerp_done)
                continue;

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

        foreach(MTOTerrain t in mtoTerrains)
        {
            List<TerrainLayer> newPalette = mtoLookupTable.ScoutForTerrainLayersChange(t, MineralToOrganic);
            if (newPalette==null)
                continue;
            t.ChangeLayers(newPalette);
        }

        foreach (OWCListener listener in MTOListeners)
        {
            listener.Call();
        }
    }
    private void ChangeModMaterial(MTOModifier iMod, Material iOldMat, Material iNewMat)
    {
        iMod.ChangeMaterial (iOldMat, iNewMat);
    }

    public bool MTOIsZero()
    {
        return ( !(OrganicMagnitude>0f) && !(MineralMagnitude>0f) );
    }
    #endregion

    #region OTC
    public void SetOrderToChaos(float iVal)
    {
        m_OrderToChaos = Mathf.Clamp(iVal, 0f, 1f);

        RefreshOTCMods();
        Managers.Instance.Sound?.UpdateBGM();
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
            listener.Call();
        }
    }

    public bool OTCIsZero()
    {
        return ( !(OrderMagnitude>0f) && !(ChaosMagnitude>0f) );
    }
    #endregion
}
