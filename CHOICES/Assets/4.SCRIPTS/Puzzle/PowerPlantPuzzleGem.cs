using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class PowerPlantPuzzleGem : MonoBehaviour
{
    public enum GEM_SHAPE { HEX, SQR, TRI}
    public CirclePathWalker pathWalker;
    public AudioClip audioClipAligned;
    public AudioClip audioClipMismatch;
    public float audioAlignementTransmissionLatency = 0.2f;
    public Material NotAlignedMat;
    public Material AlignedMat;
    
    public GEM_SHAPE gemShape;   
    public bool GemIsActive = false;
    public bool GemIsAligned = false;
    [Header("Internals")]
    [Range(-1,1)]
    private short slideDir = 0;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private Coroutine audioTransmissionCo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = NotAlignedMat;
    }

    // Update is called once per frame
    void Update()
    {
        if (GemIsActive)
        {
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("PuzzleItem");
            if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, mask))
            {
                Debug.DrawRay(transform.position, -transform.up * 50f, Color.green);
                
                PowerPlantPuzzleGem othergem = hit.collider.gameObject.GetComponent<PowerPlantPuzzleGem>();
                if (othergem.gemShape == gemShape)
                {
                    if (!GemIsAligned)
                    {
                        audioSource.clip = audioClipAligned;
                        othergem.audioSource.clip = othergem.audioClipAligned;
                        //audioSource.Play();
                        if (!audioSource.isPlaying)
                        { audioTransmissionCo = StartCoroutine(PlayGemsAudioCo(audioSource, othergem.audioSource)); }
                        meshRenderer.material = AlignedMat;
                    }
                    GemIsAligned = true;
                    othergem.GemIsAligned = true;
                    
                } else 
                {
                    
                    audioSource.clip = audioClipAligned;
                    othergem.audioSource.clip = othergem.audioClipAligned;

                    if (!audioSource.isPlaying)
                    { audioTransmissionCo = StartCoroutine(PlayGemsAudioCo(audioSource, othergem.audioSource)); }
                }
            } else {
                Debug.DrawRay(transform.position, -transform.up * 50f, Color.red);
                if (GemIsAligned)
                {
                    meshRenderer.material = NotAlignedMat;
                }

                GemIsAligned = false;
            }
        }
    }

    IEnumerator PlayGemsAudioCo(AudioSource iFirstGemAS, AudioSource iSecondGemAS)
    {
        iFirstGemAS.Play();
        yield return new WaitForSeconds(audioAlignementTransmissionLatency);
        iSecondGemAS.Play();
    }


    public bool IsSlidingCW() { return slideDir > 0; }
    public bool IsSlidingCCW() { return slideDir < 0; }
    public void SetAsSlidingCW() { slideDir = 1; pathWalker.NotifyCWMotion(); }
    public void SetAsSlidingCCW() { slideDir = -1; pathWalker.NotifyCCWMotion(); }
    public void SetAsNotSliding() { slideDir = 0; }

}
