using UnityEngine;

[CreateAssetMenu(fileName = "BGMData", menuName = "Scriptable Objects/BGMData")]
public class BGMData : ScriptableObject
{
    public float BgmTrackCutVolume = -30f;
    [Range(0,512)]
    public uint BPM_SYNC = 80;
    public uint TIME_SIG_MEASURE_SIZE = 4;
    public uint BPM_PITCH = 80;
    [Tooltip("Time it takes to go from current pitch to target pitch on BPM_PITCH modification")]
    public float pitchShiftTime = 0.5f;
    [Header("Order")]
    public AudioClip orderBGM;
    public AnimationCurve bgmOrderedVolumeCurve;
    [Header("Chaos")]
    public AudioClip chaosBGM;
    public AnimationCurve bgmChaosVolumeCurve;
    [Header("Mineral")]
    public AudioClip mineralBGM;
    public AnimationCurve bgmMineralVolumeCurve;
    [Header("Organic")]
    public AudioClip organicBGM;
    public AnimationCurve bgmOrganicVolumeCurve;
    [Header("Gloomy")]
    public AudioClip gloomyBGM;
    public AnimationCurve bgmGloomyVolumeCurve;
    [Header("Lush")]
    public AudioClip lushBGM;
    public AnimationCurve bgmLushVolumeCurve;

}
