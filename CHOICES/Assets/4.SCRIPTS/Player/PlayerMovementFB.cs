using UnityEngine;

public class PlayerMovementFB : MonoBehaviour, IFeedbackEval
{
    [Header("Mand")]
    public PlayerController PC;
    public GameFeedback feedback;
    [Header("Tweaks")]
    public float samplingStep = 0.2f;
    public float stopMovingTriggerTime = 10f;
    public float evalLerpTime = 10f;
    [Header("Internals")]
    public float lastMovementTime = 0f;
    private float lastSamplingTime;
    private float influence;
    private bool playerMoving = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        feedback.Init(this);
        influence = 0f;
        lastMovementTime = Time.time;
        playerMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - lastSamplingTime) < samplingStep)
            return;

        if (PC.playerInAction)
            return;

        if (!PC.isMoving)
        {
            if (Time.time - lastMovementTime >= stopMovingTriggerTime)
            {
                
                // ORDER
                influence = -1f;
            }
        }
        else if ((PC.hMove == 0f) || (PC.vMove == 0f))
        {
            // LESSE ORDER
            influence = -0.5f;
            lastMovementTime = Time.time;
        }
        else
        {
            if (PC.isRunning)
            {
                influence = 1f;
            }
            lastMovementTime = Time.time;
        }
        feedback.Refresh();
        
    }

    public float feedbackEvaluator()
    {
        return influence;
    }
}
