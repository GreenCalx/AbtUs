using UnityEngine;

[RequireComponent(typeof(Feedback))]
public class GlowSpot : MonoBehaviour
{
    [Header("Mand Refs")]
    public Light lamp;
    public Collider lightTrigger;
    [Header("Tweaks")]
    [SerializeField]
    private float glow_strength; // + 0.01 GTL.s

    private bool player_in_spotlight = false;

    public float maxGtlValue;
    public float glowDelay = 0;
    private float glowTimer = 0;
    private Feedback glowFeedback;


    void Start()
    {
        glowFeedback = GetComponent<Feedback>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            player_in_spotlight = true;
            glowTimer = glowDelay;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
            player_in_spotlight = false;
        glowTimer = glowDelay;
    }

    private void FixedUpdate()
    {

        if (player_in_spotlight && glowTimer > 0)
            glowTimer -= Time.fixedDeltaTime;
        else if(glowTimer < 0)
        {
            // glowFeedback.Influence += glow_strength * 0.01f * Time.fixedDeltaTime;
            // glowFeedback.use();
        }
    }
}
