using UnityEngine;

public class CirclePathWalker : MonoBehaviour
{
    [Range(0f,360f)]
    public float initAngle = 0f;

    private float m_Angle = 0f;
    public float angle{
        get { return m_Angle; }
        set { m_Angle = Mathf.Clamp(value,0f,360f); }
    }
    public CirclePath path;
    [Header("Interaction with env")]
    public bool CanBeBlocked = true;
    public bool IsBlocked = false;
    private Vector3 freezedPos = Vector3.zero;
    public bool CWSlideMotion = false;
    public bool CCWSlideMotion = false;
    public bool CWPathMotion = false;
    public bool CCWPathMotion = false;
    public bool IsBlockedCW = false;
    public bool IsBlockedCCW = false;
    public bool isSlidingCW = false;
    public bool isSlidingCCW = false;
    public bool isManualSlidingCW = false;
    public bool isManualSlidingCCW = false;

    void Start()
    {
        angle = initAngle;
    }

    void Update()
    {
        if (!CanBeBlocked)
        {
            transform.localPosition = path.GetCoord(angle);
            return;
        }

        if ( IsBlockedCW && (CWPathMotion && isManualSlidingCCW) )
        {
            transform.position = freezedPos;
            angle = path.GetAngle(transform.localPosition);
        } 
        else if ( IsBlockedCCW && (CCWPathMotion && isManualSlidingCW))
        {
            transform.position = freezedPos;
            angle = path.GetAngle(transform.localPosition);
        }
        else if (!IsBlockedCW && isManualSlidingCW)
        {
            transform.localPosition = path.GetCoord(angle);
        }
        else if (!IsBlockedCCW && isManualSlidingCCW)
        {
            transform.localPosition = path.GetCoord(angle);
        }
        else if (IsBlocked && (isSlidingCCW || isSlidingCW))
        {
            transform.position = freezedPos;
            angle = path.GetAngle(transform.localPosition);
        }
        else if (!IsBlocked && !IsBlockedCCW)
        {
            transform.localPosition = path.GetCoord(angle);
        }
    }

    void OnCollisionEnter(Collision iCol)
    {
        IsBlocked = true;
        freezedPos = transform.position;
        if (CCWPathMotion || isManualSlidingCCW) { isSlidingCW = true; IsBlockedCCW = true; }
        if (CWPathMotion || isManualSlidingCW) { isSlidingCCW = true; IsBlockedCW = true; }
    }
    void OnCollisionStay(Collision iCol)
    {
        if (IsBlockedCCW && CWPathMotion) { IsBlocked = false;}
        if (IsBlockedCW && CCWPathMotion) { IsBlocked = false;}
    }
    void OnCollisionExit(Collision iCol)
    {
        IsBlocked = false;
        isSlidingCW = false; 
        isSlidingCCW = false;
        IsBlockedCCW = false; 
        IsBlockedCW = false;
    }

    public void NotifyCWMotion(){ CWSlideMotion = true; CCWSlideMotion = false;}
    public void NotifyCCWMotion(){ CWSlideMotion = false; CCWSlideMotion = true;}
}
