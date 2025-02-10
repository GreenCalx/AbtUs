using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class PowerPlantPuzzleGem : MonoBehaviour
{
    public enum GEM_SHAPE { HEX, SQR, TRI}
    public CirclePathWalker pathWalker;
    public AudioClip audioClipAligned;
    public AudioClip audioClipMismatch;
    public Material NotAlignedMat;
    public Material AlignedMat;
    
    public GEM_SHAPE gemShape;   
    public bool GemIsActive = false;
    public bool GemIsAligned = false;

    private AudioSource audioSource;
    private MeshRenderer meshRenderer;

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
                        audioSource.Play();
                        meshRenderer.material = AlignedMat;
                    }
                    GemIsAligned = true;
                    othergem.GemIsAligned = true;
                    
                } else 
                {
                    audioSource.clip = audioClipMismatch;
                    audioSource.Play();
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
}
