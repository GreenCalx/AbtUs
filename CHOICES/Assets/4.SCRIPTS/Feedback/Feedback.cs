using UnityEngine;

public class Feedback : MonoBehaviour
{
    [Range(0,1)]
    public float value = 0f;

    [SerializeField, Range(0,1)]
    private float maxVarInfluence = 1f;

    [SerializeField, Range(0, 1)]
    private float minVarInfluence = 0f;

    public FeedbackVariable.Type feedback_type;

    [Header("Optional OWC range")]

    [SerializeField, Range(0, 1)]
    private float maxOWCInfluence = 1f;

    [SerializeField, Range(0, 1)]
    private float minOWCInfluence = 0f;



    private FeedbackManager fbm;

    private void Start()
    {
        fbm = FeedbackManager.Instance;

        
    }

    //override for different feedbacks, default is add
    public void applyFeedback(FeedbackVariable FeedbackVar, float OWCValue)
    {
        if(OWCValue < minOWCInfluence || OWCValue > maxOWCInfluence) { return; }

        var new_value = FeedbackVar._value + value;
        
        new_value = Mathf.Clamp(new_value, minVarInfluence, maxVarInfluence);

        FeedbackVar._value = new_value;
    }

    public void use()
    {
        fbm.ChangeOWC(this);
    }
}

