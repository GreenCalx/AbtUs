using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

public class SoundManager : MonoBehaviour
{
    [Header("Audio mixers Refs")]
    public AudioMixer MasterMixer;
    public AudioMixerGroup BGMMixerGroup;
    public AudioMixerGroup OrderMixerGroup;
    public AudioMixerGroup ChaosMixerGroup;
    public AudioMixerGroup MineralMixerGroup;
    public AudioMixerGroup OrganicMixerGroup;
    public AudioMixerGroup GloomyMixerGroup;
    public AudioMixerGroup LushMixerGroup;

    public AudioMixerGroup FXMixerGroup;
    [Header("Prefab References")]
    public GameObject prefabFXAudioSource;
    public GameObject prefabBGMAudioSource;

    [Header("BGM")]
    public AudioClip chaosBGM;
    public AudioClip orderBGM;
    public AudioClip organicBGM;
    public AudioClip mineralBGM;
    public AudioClip gloomyBGM;
    public AudioClip lushBGM;

    [Header("Tweaks")]
    public float BgmMinVolume = -80f;
    public float BgmMaxVolume = 0f;
    [Range(0,512)]
    public uint BPM_SYNC = 90;
    public uint TIME_SIG_MEASURE_SIZE = 4;
    [Tooltip("NOT IMPL ATM")]
    public uint TIME_SIG_NOTE_VAL = 4;
    public int fxChannels = 3;
    public float MaxTimeBeforeFXChannelClean = 10f;
    private float beatstep = 0f;

    [Header("Internal View")]
    public List<AudioSource> fxAudioCanals;
    public AudioSource bgmOrderAudioCanal;
    public AudioSource bgmChaosAudioCanal;
    public AudioSource bgmMineralAudioCanal;
    public AudioSource bgmOrganicAudioCanal;
    public AudioSource bgmGloomyAudioCanal;
    public AudioSource bgmLushAudioCanal;
    public List<AudioSource> playQueue;

    [Header("Internal")]
    private List<AudioSource> bgmAudioCanals;
    private const string mixParmBGMOrderVolume      = "BGMOrderVolume";
    private const string mixParmBGMChaosVolume      = "BGMChaosVolume";
    private const string mixParmBGMMineralVolume    = "BGMMineralVolume";
    private const string mixParmBGMOrganicVolume    = "BGMOrganicVolume";
    private const string mixParmBGMGloomyVolume     = "BGMGloomyVolume";
    private const string mixParmBGMLushVolume       = "BGMLushVolume";
    private float elapsedBeatStep = 0f;
    private ushort elapsedStepInMeasure = 0;
    
    

    #region UNITY
    void Awake()
    {
        fxAudioCanals = new List<AudioSource>(fxChannels);
        playQueue = new List<AudioSource>(0);

        beatstep = 60f/BPM_SYNC;
        elapsedBeatStep = 0f;
        elapsedStepInMeasure = 0;
    }

    void Start()
    {
        InitBGMSources();
    }
    void Update()
    {
        //UpdateBGM();
        elapsedBeatStep += Time.deltaTime;
        if (elapsedBeatStep > beatstep)
        { OnBeatStep(); }
        if (elapsedStepInMeasure >= TIME_SIG_MEASURE_SIZE)
        { OnMeasureStep(); }
    }

    public void OnMeasureStep()
    {
        if (playQueue.Count>0)
        {
            float time = 0f;
            foreach(AudioSource source in playQueue)
            {
                if (TryGetSampleTime(out time))
                {
                    source.time = time;
                }
                source.Play();
            }
            playQueue.Clear();
        }
        elapsedStepInMeasure = 0;
    }

    public void OnBeatStep()
    {
        Debug.Log(elapsedStepInMeasure);
        elapsedBeatStep = 0f; 
        elapsedStepInMeasure++; 
    }
    #endregion

    #region BGM
    private void InitBGMSources()
    {
        Transform bgmHost = Managers.Instance.Camera.playerCam.transform;
        
        bgmOrderAudioCanal =  SpawnBGMAudioSource(orderBGM,   bgmHost,    OrderMixerGroup);
        bgmChaosAudioCanal = SpawnBGMAudioSource(chaosBGM,   bgmHost,    ChaosMixerGroup);
        bgmMineralAudioCanal =  SpawnBGMAudioSource(mineralBGM, bgmHost,    MineralMixerGroup);
        bgmOrganicAudioCanal = SpawnBGMAudioSource(organicBGM, bgmHost,    OrganicMixerGroup);
        bgmGloomyAudioCanal = SpawnBGMAudioSource(gloomyBGM,  bgmHost,    GloomyMixerGroup);
        bgmLushAudioCanal = SpawnBGMAudioSource(lushBGM,    bgmHost,    LushMixerGroup);   

        bgmAudioCanals = new List<AudioSource>();
        bgmAudioCanals.Add(bgmOrderAudioCanal);
        bgmAudioCanals.Add(bgmChaosAudioCanal);
        bgmAudioCanals.Add(bgmMineralAudioCanal);
        bgmAudioCanals.Add(bgmOrganicAudioCanal);
        bgmAudioCanals.Add(bgmGloomyAudioCanal);
        bgmAudioCanals.Add(bgmLushAudioCanal);
    }

    public void UpdateBGM()
    {
        OverWorldControl owc = OverWorldControl.Instance;

        // update volumes from OWC
        float orderVol =    math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.OrderMagnitude);
        float chaosVol =    math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.ChaosMagnitude);
        float mineralVol =  math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.MineralMagnitude);
        float organicVol =  math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.OrganicMagnitude);
        float lushVol =     math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.LushMagnitude);
        float gloomyVol =   math.remap(0f, 1f, BgmMinVolume, BgmMaxVolume, owc.GloomyMagnitude);

        // Change volumes accordingly
        MasterMixer.SetFloat(mixParmBGMChaosVolume, chaosVol);
        if (orderVol > BgmMinVolume)
        {SyncPlay(bgmOrderAudioCanal);}
        else if (bgmOrderAudioCanal.isPlaying) { bgmOrderAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMOrderVolume, orderVol);
        if (chaosVol > BgmMinVolume)
        {SyncPlay(bgmChaosAudioCanal);}
        else if (bgmChaosAudioCanal.isPlaying) { bgmChaosAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMMineralVolume, mineralVol);
        if (mineralVol > BgmMinVolume)
        {SyncPlay(bgmMineralAudioCanal);}
        else if (bgmMineralAudioCanal.isPlaying) { bgmMineralAudioCanal.Stop(); }
        
        MasterMixer.SetFloat(mixParmBGMOrganicVolume, organicVol);
        if (organicVol > BgmMinVolume)
        {SyncPlay(bgmOrganicAudioCanal);}
        else if (bgmOrganicAudioCanal.isPlaying) { bgmOrganicAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMLushVolume, lushVol);
        if (lushVol > BgmMinVolume)
        {SyncPlay(bgmLushAudioCanal);}
        else if (bgmLushAudioCanal.isPlaying) { bgmLushAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMGloomyVolume, gloomyVol);
        if (gloomyVol > BgmMinVolume)
        {SyncPlay(bgmGloomyAudioCanal);}
        else if (bgmGloomyAudioCanal.isPlaying) { bgmGloomyAudioCanal.Stop(); }
    }

    private AudioSource SpawnBGMAudioSource(AudioClip iClip, Transform iFXSourceParent, AudioMixerGroup iMixerGroup)
    {
        GameObject newAS = Instantiate(prefabBGMAudioSource);
       
        newAS.transform.parent = iFXSourceParent;
        newAS.transform.localPosition = Vector3.zero;
        newAS.transform.name = "BGM " + iMixerGroup.ToString();

        AudioSource asSource = newAS.GetComponent<AudioSource>();
        asSource.clip = iClip;
        asSource.outputAudioMixerGroup = iMixerGroup;
        return asSource;
    }

    public void SyncPlay(AudioSource iSource)
    {
        if (iSource.isPlaying)
            return;
        if (playQueue.Contains(iSource))
            return;
        playQueue.Add(iSource);
    }

    private bool TryGetSampleTime(out float ioTime)
    {
        ioTime = 0f;
        foreach (AudioSource source in bgmAudioCanals)
        {
            if (source.isPlaying)
            {
                ioTime = source.time;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region FX
    public bool TryPlayFX(AudioClip iAudioClip, Transform iFXSourceParent)
    {
        if (prefabFXAudioSource==null)
            return false;
        return TryPlayFX(SpawnFXAudioSource(iAudioClip, iFXSourceParent));
    }

    public bool TryPlayFX(AudioSource iASource)
    {
        if (fxAudioCanals.Contains(iASource))
            return false; // already playing same audio source
        if (UsedFXChannels() >= fxChannels)
            return false; // no more space

        int freeChanIndex = 0;
        if (!TryGetFreeFXChannel(out freeChanIndex))
            return false;

        fxAudioCanals[freeChanIndex] = iASource;
        iASource.Play();
        StartCoroutine(PostCleanFXChannelCo(freeChanIndex));
        
        return true;
    }

    IEnumerator PostCleanFXChannelCo(int iChanIndex)
    {
        float elapsedTime;
        while (fxAudioCanals[iChanIndex].isPlaying)
        {
            elapsedTime =+ Time.deltaTime;
            if (elapsedTime >= MaxTimeBeforeFXChannelClean)
                break;
            yield return null;
        }
        fxAudioCanals[iChanIndex] = null;
    }

    private AudioSource SpawnFXAudioSource(AudioClip iClip, Transform iFXSourceParent)
    {
        GameObject newAS = Instantiate(prefabFXAudioSource);
       
        newAS.transform.parent = iFXSourceParent;
        newAS.transform.localPosition = Vector3.zero;

        newAS.AddComponent<AudioSource>();
        AudioSource asSource = newAS.GetComponent<AudioSource>();
        asSource.clip = iClip;

        return asSource;
    }

    private int UsedFXChannels() { return fxAudioCanals.Where(e => e!=null).ToList().Count; }

    private bool TryGetFreeFXChannel(out int ioFreeIndex) 
    {
        for (int i=0; i<fxChannels;i++)
        {
            if (fxAudioCanals[i]==null)
            {
                ioFreeIndex = i;
                return true;
            }
        }
        ioFreeIndex = -1;
        return false;
    }
    #endregion
}
