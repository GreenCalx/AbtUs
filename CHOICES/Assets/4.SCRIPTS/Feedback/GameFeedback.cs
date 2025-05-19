using System;
using UnityEngine;
using UnityEngine.Events;

public class GameFeedback : MonoBehaviour
{
    [Header("Data Define")]
    public IFeedbackEval evaluatorTarget;
    public FeedbackData fData;
    [Header("Internal View")]
    
    [SerializeField, Range(0f, 2f)]
    public float Influence = 1f;
    public UnityEvent<float> InitFromFeedbackFunc;

    [Header("Optional OWC range")]

    [SerializeField, Range(0, 1)]
    private float maxOWCWindow = 1f;

    [SerializeField, Range(0, 1)]
    private float minOWCWindow = 0f;

    private FeedbackManager fbm;

    public AnimationCurve fInfluenceCurve;

    private void Start()
    {
        fbm = FeedbackManager.Instance;
    }

    public void Init(IFeedbackEval iEvaluator)
    {
        evaluatorTarget = iEvaluator;
        FeedbackManager.Instance.RegisterGameFeedback(this, evaluatorTarget.feedbackEvaluator);
       // Utils.CauchyToAnimCurve(ref fInfluenceCurve, 0.5f, 0.25f);
    }

    public void Refresh()
    {
        fbm.AsyncNotif(this);
        // fbm.ChangeOWC(this);
        // if (consumeOnUse)
        // {
        //     Destroy(this);
        // }
            
    }
}

