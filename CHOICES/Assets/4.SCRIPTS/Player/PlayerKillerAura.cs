using UnityEngine;

public class PlayerKillerAura : MonoBehaviour, IFeedbackEval
{
    
    public float decayStartTime = 10f;
    public float decayPeriodicityInSec = 5f;
    public float decay = 0f;
    public float accumulated = 0f;
    public float max_accumulation = 10f;
    private double lastKillTime;
    private float elapsedSinceLastDecay = 0f;
    public GameFeedback killFeedback;
    void Start()
    {
        accumulated = 0f;
        elapsedSinceLastDecay = 0f;
        killFeedback.Init(this);

        lastKillTime = Time.time;
    }

    void Update()
    {
        if (accumulated <= 0)
            return;
        if ((Time.time - lastKillTime) < decayStartTime)
            return;
            
        if (elapsedSinceLastDecay >= decayPeriodicityInSec)
        {
            Decay();
            elapsedSinceLastDecay = 0f;
        }
        else
        {
            elapsedSinceLastDecay += Time.deltaTime;
        }
    }

    void Decay()
    {
        accumulated -= (accumulated * decay);
        killFeedback.Refresh();
    }

    public void NotifyKill()
    {
        accumulated++;
        lastKillTime = Time.time;
        killFeedback.Refresh();
    }

    public float feedbackEvaluator()
    {
        return accumulated / max_accumulation;
    }
}
