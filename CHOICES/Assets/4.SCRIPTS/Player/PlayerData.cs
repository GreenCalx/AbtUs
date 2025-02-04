using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Feedbacks variables")]

    [Range(-0.2f, 0.2f)]
    public float Init_Glow = 0f;
 
    public float Glow
    {
        get { return m_Glow; }
        set { m_Glow = Mathf.Clamp(value, -0.2f, 0.2f); }
    }
    private float m_Glow;
    private float delta_Glow = 0;

    private Feedback PlayerFeedback;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Glow = Init_Glow;
        PlayerFeedback = gameObject.GetComponent<Feedback>();
    }

    public void AddGlow(float value)
    {
        var vGlow = Glow;
        Glow = Glow + value;
        delta_Glow += Glow - vGlow;
    }

    public void FixedUpdate()
    {
        if (delta_Glow != 0)
        {
            PlayerFeedback.GTL = delta_Glow;
            PlayerFeedback.use();
            delta_Glow = 0;
        }

    }
}
