using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    private static FeedbackManager instance = null;
    public static FeedbackManager Instance => instance;

    [SerializeField]
    private OverWorldControl OWC;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

    }

    public void Start()
    {
        OWC = OverWorldControl.Instance;
    }

    public void ChangeOWC(Feedback feedback) 
    {
        float otc = OWC.OrderToChaos;
        float mto = OWC.MineralToOrganic;
        float gtl = OWC.GloomyToLush;
        
        if(feedback.feedback_type == Feedback.Type.GTL)
            feedback.applyFeedback(ref gtl);
            OWC.GloomyToLush = gtl;

        if (feedback.feedback_type == Feedback.Type.MTO)
             feedback.applyFeedback(ref mto);
            OWC.MineralToOrganic = mto;

        if (feedback.feedback_type == Feedback.Type.OTC)
            feedback.applyFeedback(ref otc);
            OWC.GloomyToLush = otc;

        Debug.Log("gtl = " + gtl + "mto = " + mto + "otc = " + otc);
    }

}
