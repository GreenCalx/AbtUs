using UnityEngine;

public class HitboxFeedback : Feedback
{
    public bool one_shot = false;
    public bool destroy_after_use = false;
    public bool delay_between_consecutive_feedbacks = false;
    public float time_inside_for_feedback;
    private float current_time_inside;
    private bool player_inside_hitbox = false;
    private void Awake()
    {
        current_time_inside = time_inside_for_feedback;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_inside_hitbox = true;
            current_time_inside = time_inside_for_feedback;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player_inside_hitbox = false;
        current_time_inside = time_inside_for_feedback;
    }


    private void FixedUpdate()
    {
        if (player_inside_hitbox)
        {
            current_time_inside -= Time.fixedDeltaTime;
            if (current_time_inside <= 0)
            {
                use();
                if (one_shot && destroy_after_use) { Destroy(this.gameObject); }
                else if (one_shot) { this.enabled = false; }
                else if (delay_between_consecutive_feedbacks) { current_time_inside = time_inside_for_feedback; }

            }
        }
    }
}