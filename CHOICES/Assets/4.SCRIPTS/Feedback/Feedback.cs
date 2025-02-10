using UnityEngine;

public class Feedback : MonoBehaviour
{
    [SerializeField]
    public float value = 0f;

    public float max_influence = 1f;
    public float min_influence = 0f;

    public Type feedback_type;
    public enum Type { GTL, OTC, MTO};

    private FeedbackManager fbm;

    private void Start()
    {
        fbm = FeedbackManager.Instance;
    }

    //override for different feedbacks, default is add
    public void applyFeedback(ref float OWC_value)
    {
        if(OWC_value < min_influence || OWC_value > max_influence) { return; }

        var new_value = OWC_value + value;
        
        new_value = Mathf.Clamp(new_value, min_influence, max_influence);

        OWC_value = new_value;
    }

    public void use()
    {
        fbm.ChangeOWC(this);
    }
}

