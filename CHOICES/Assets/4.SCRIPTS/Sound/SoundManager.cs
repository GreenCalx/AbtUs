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
    [Header("Debug")]
    public bool updatePitchShift = false;

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
    public BGMData bgmData;
    [Header("Tweaks")]
    public int fxChannels = 3;
    public float MaxTimeBeforeFXChannelClean = 10f;
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
    private const string mixParmBGMPitchShift       = "BGMPitchShiftMul";
    private float elapsedBeatStep = 0f;
    private ushort elapsedStepInMeasure = 0;
    private float beatstep = 0f;
    private float lastComputedPitch = 1f;
    private Coroutine lerpPitchCoroutine;

    #region UNITY
    void Awake()
    {
        fxAudioCanals = new List<AudioSource>( new AudioSource[fxChannels]);
        playQueue = new List<AudioSource>(0);

        beatstep = 60f/bgmData.BPM_SYNC;
        elapsedBeatStep = 0f;
        elapsedStepInMeasure = 0;
    }

    void Start()
    {
        //InitBGMSources();
    }
    void Update()
    {
        //UpdateBGM();
        elapsedBeatStep += Time.deltaTime;
        if (elapsedBeatStep > beatstep)
        { OnBeatStep(); }
        if (elapsedStepInMeasure >= bgmData.TIME_SIG_MEASURE_SIZE)
        { OnMeasureStep(); }

        if (updatePitchShift)
        {
            ComputePitchChange();
            updatePitchShift = false;
        }
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
        elapsedBeatStep = 0f; 
        elapsedStepInMeasure++; 
    }
    #endregion

    #region BGM
    public void InitBGMSources(Transform iAudioHost)
    {
        //Transform bgmHost = Managers.Instance.Camera.playerCam.transform;
        
        bgmOrderAudioCanal =  SpawnBGMAudioSource(bgmData.orderBGM,   iAudioHost,    OrderMixerGroup);
        bgmChaosAudioCanal = SpawnBGMAudioSource(bgmData.chaosBGM,   iAudioHost,    ChaosMixerGroup);
        bgmMineralAudioCanal =  SpawnBGMAudioSource(bgmData.mineralBGM, iAudioHost,    MineralMixerGroup);
        bgmOrganicAudioCanal = SpawnBGMAudioSource(bgmData.organicBGM, iAudioHost,    OrganicMixerGroup);
        bgmGloomyAudioCanal = SpawnBGMAudioSource(bgmData.gloomyBGM,  iAudioHost,    GloomyMixerGroup);
        bgmLushAudioCanal = SpawnBGMAudioSource(bgmData.lushBGM,    iAudioHost,    LushMixerGroup);   

        bgmAudioCanals = new List<AudioSource>();
        bgmAudioCanals.Add(bgmOrderAudioCanal);
        bgmAudioCanals.Add(bgmChaosAudioCanal);
        bgmAudioCanals.Add(bgmMineralAudioCanal);
        bgmAudioCanals.Add(bgmOrganicAudioCanal);
        bgmAudioCanals.Add(bgmGloomyAudioCanal);
        bgmAudioCanals.Add(bgmLushAudioCanal);

        MasterMixer.GetFloat(mixParmBGMPitchShift, out lastComputedPitch);

        UpdateBGM();
    }

    public void UpdateBGM()
    {
        OverWorldControl owc = OverWorldControl.Instance;

        float orderVol =    bgmData.bgmOrderedVolumeCurve.Evaluate(owc.OrderToChaos);
        float chaosVol =    bgmData.bgmChaosVolumeCurve.Evaluate(owc.OrderToChaos);
        float mineralVol =  bgmData.bgmMineralVolumeCurve.Evaluate(owc.MineralToOrganic);
        float organicVol =  bgmData.bgmOrganicVolumeCurve.Evaluate(owc.MineralToOrganic);
        float lushVol =     bgmData.bgmLushVolumeCurve.Evaluate(owc.GloomyToLush);
        float gloomyVol =   bgmData.bgmGloomyVolumeCurve.Evaluate(owc.GloomyToLush);

        // Change volumes accordingly
        MasterMixer.SetFloat(mixParmBGMChaosVolume, chaosVol);
        if (orderVol > bgmData.BgmTrackCutVolume)
        {SyncPlay(bgmOrderAudioCanal);}
        else if (bgmOrderAudioCanal.isPlaying) { bgmOrderAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMOrderVolume, orderVol);
        if (chaosVol > bgmData.BgmTrackCutVolume)
        {SyncPlay(bgmChaosAudioCanal);}
        else if (bgmChaosAudioCanal.isPlaying) { bgmChaosAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMMineralVolume, mineralVol);
        if (mineralVol > bgmData.BgmTrackCutVolume)
        {SyncPlay(bgmMineralAudioCanal);}
        else if (bgmMineralAudioCanal.isPlaying) { bgmMineralAudioCanal.Stop(); }
        
        MasterMixer.SetFloat(mixParmBGMOrganicVolume, organicVol);
        if (organicVol > bgmData.BgmTrackCutVolume)
        {SyncPlay(bgmOrganicAudioCanal);}
        else if (bgmOrganicAudioCanal.isPlaying) { bgmOrganicAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMLushVolume, lushVol);
        if (lushVol > bgmData.BgmTrackCutVolume)
        {SyncPlay(bgmLushAudioCanal);}
        else if (bgmLushAudioCanal.isPlaying) { bgmLushAudioCanal.Stop(); }

        MasterMixer.SetFloat(mixParmBGMGloomyVolume, gloomyVol);
        if (gloomyVol > bgmData.BgmTrackCutVolume)
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

    private void ComputePitchChange()
    {
        if (bgmData.BPM_PITCH == bgmData.BPM_SYNC)
            return;
        
        if (lerpPitchCoroutine!=null)
        {
            StopCoroutine(lerpPitchCoroutine);
            lerpPitchCoroutine = null;
        }
        lerpPitchCoroutine = StartCoroutine(LerpPitchCo());
    }

    IEnumerator LerpPitchCo()
    {
        float target = (float)bgmData.BPM_PITCH / (float)bgmData.BPM_SYNC;
        float elapsedTime = 0f;
        while ( elapsedTime < bgmData.pitchShiftTime )
        {
            elapsedTime += Time.deltaTime;
            float frac = Mathf.Clamp01(elapsedTime / bgmData.pitchShiftTime);
            float target_lerp = Utils.Lerp(lastComputedPitch, target, frac);
            MasterMixer.SetFloat(mixParmBGMPitchShift, target_lerp);
            yield return null;
        }
        MasterMixer.SetFloat(mixParmBGMPitchShift, target);
        lastComputedPitch = target;
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
            if (i>=fxAudioCanals.Count)
            {
                ioFreeIndex = i;
                return true;
            }
            else if (fxAudioCanals[i]==null)
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
