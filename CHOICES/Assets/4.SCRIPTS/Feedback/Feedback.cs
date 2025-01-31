using UnityEngine;

public class Feedback : MonoBehaviour
{
    [SerializeField]
    public float OTC = 0f;

    [SerializeField]
    public float MTO = 0f;

    [SerializeField]
    public float GTL = 0f;

    private FeedbackManager fbm;

    private void Start()
    {
        fbm = FeedbackManager.Instance;
    }
    
    //override for different feedbacks, default is add
    public void applyFeedback(ref float p_otc, ref float p_mto, ref float p_gtl)
    {
        p_otc += OTC;
        p_mto += MTO;
        p_gtl += GTL;
    }

    public void try_use()
    {
        fbm.ChangeOWC(this);
    }
}

