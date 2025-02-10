using UnityEngine;

public class TargetFeedback : Feedback
{
    public bool one_shot = false;
    public bool delay_between_consecutive_feedbacks = false;
    public float time_looking;
    private float current_time_looking;
    private bool looking = false;

    private void Awake()
    {
        current_time_looking = time_looking;
    }
    public void player_looking(bool bol)
    {
        looking = bol;
        Debug.Log(bol);
    }

    private void FixedUpdate()
    {
        if(looking)
        {
            current_time_looking -=  Time.fixedDeltaTime;
            if(current_time_looking <= 0)
            {
                use();
                if (one_shot) { Destroy(this.gameObject); }
                else if (delay_between_consecutive_feedbacks) { current_time_looking = time_looking; }
                
            }
        }
        else { current_time_looking = time_looking; }
    }
}
