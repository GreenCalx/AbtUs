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
    public Material WrongAlignedMat;
    
    public GEM_SHAPE gemShape;   
    public bool GemIsActive = false;
    public bool GemIsAligned = false;
    public bool GemIsMisalgined = false;
    [Header("Internals")]
    [Range(-1,1)]
    private short slideDir = 0;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private Coroutine audioTransmissionCo;
    private bool requireMatReset = false;
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
                
                PowerPlantPuzzleGem othergem = hit.collider.gameObject.GetComponentInParent<PowerPlantPuzzleGem>();
                if (othergem.gemShape == gemShape)
                {
                    if (!GemIsAligned)
                    {
                        audioTransmissionCo = StartCoroutine(PlayGemsAudioCo(audioClipAligned, othergem.audioClipAligned, othergem.transform));
                        meshRenderer.material = AlignedMat;
                    }
                    GemIsAligned = true;
                    othergem.GemIsAligned = true;

                    GemIsMisalgined = false;
                    othergem.GemIsMisalgined = false;
                    
                } else if (othergem.gemShape != gemShape)
                {
                    if (!GemIsMisalgined)
                    {
                        audioTransmissionCo = StartCoroutine(PlayGemsAudioCo(audioClipAligned, othergem.audioClipAligned, othergem.transform));
                        meshRenderer.material = WrongAlignedMat;
                    }

                    GemIsAligned = true;
                    othergem.GemIsAligned = true;

                    GemIsMisalgined = true;
                    othergem.GemIsMisalgined = true;
                }
                requireMatReset = true;

            } else {
                Debug.DrawRay(transform.position, -transform.up * 50f, Color.red);
                if (requireMatReset)
                {
                    meshRenderer.material = NotAlignedMat;
                    requireMatReset = false;
                }
                
                GemIsAligned = false;
                GemIsMisalgined = false;
            }
        }
    }

    IEnumerator PlayGemsAudioCo(AudioClip iFirstGemClip, AudioClip iSecondGemClip, Transform iOtherGem)
    {
        Managers.Instance.Sound.TryPlayFX(iFirstGemClip, transform);
        yield return new WaitForSeconds(audioAlignementTransmissionLatency);
        Managers.Instance.Sound.TryPlayFX(iSecondGemClip, iOtherGem);
    }


    public bool IsSlidingCW() { return slideDir > 0; }
    public bool IsSlidingCCW() { return slideDir < 0; }
    public void SetAsSlidingCW() { slideDir = 1; pathWalker.NotifyCWMotion(); }
    public void SetAsSlidingCCW() { slideDir = -1; pathWalker.NotifyCCWMotion(); }
    public void SetAsNotSliding() { slideDir = 0; }

}
