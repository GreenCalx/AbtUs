using UnityEngine;

public class GlowSpot : OWCListener
{
    [SerializeField]
    private float glow_strength; // + 0.01 GTL.s

    private bool player_in_spotlight = false;

    public float maxGtlValue;
    public float glowDelay = 0;
    private float glowTimer = 0;

    private Feedback glowFeedback;

    private Light lamp;
    private Collider lightAura;
    override protected void Init(float axis_value)
    {
        lamp = transform.GetComponent<Light>();
        lightAura = transform.GetComponent<CapsuleCollider>();
        if (axis_value < maxGtlValue) { lamp.enabled = lightAura.enabled = false; }
        glowFeedback = GetComponent<Feedback>();
    }

    public override void Call(float gtl)
    {
        lamp.enabled = lightAura.enabled = gtl <= maxGtlValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_in_spotlight = true;
            glowTimer = glowDelay;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player_in_spotlight = false;
        glowTimer = glowDelay;
    }

    private void FixedUpdate()
    {

        if (player_in_spotlight && glowTimer > 0)
            glowTimer -= Time.fixedDeltaTime;
        else if(glowTimer < 0)
        {
            glowFeedback.value += glow_strength * 0.01f * Time.fixedDeltaTime;
            glowFeedback.use();
        }
    }
}
