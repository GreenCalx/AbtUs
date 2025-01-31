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

        feedback.applyFeedback(ref otc , ref mto, ref gtl);

        OWC.GloomyToLush = gtl;
        OWC.MineralToOrganic = mto;
        OWC.GloomyToLush = gtl;

        Debug.Log(gtl);
    }

}
