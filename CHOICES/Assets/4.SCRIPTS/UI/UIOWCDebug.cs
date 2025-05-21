using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIOWCDebug : MonoBehaviour
{
    public Color ActiveColor = Color.yellow;
    public Color InactiveColor = Color.grey;

    public UIOWCStateLine MtOLine;
    public UIOWCStateLine GtLLine;
    public UIOWCStateLine OtCLine;
    [Header("Behaviour")]
    public bool ContinuousUpdate = true;

    private OverWorldControl owc;
    // Update is called once per frame
    void Update()
    {
        if (owc == null)
        { owc = OverWorldControl.Instance; }

        // MTO
        if (owc.MTOIsZero())
        {
            MtOLine.LeftBar.fillAmount = 0f;
            MtOLine.RightBar.fillAmount = 0f;
            MtOLine.center.color = ActiveColor;
        } else {
            MtOLine.center.color = InactiveColor;
            MtOLine.LeftBar.fillAmount = owc.MineralMagnitude;
            MtOLine.RightBar.fillAmount = owc.OrganicMagnitude;
        }

        // GTL
        if (owc.GTLIsZero())
        {
            GtLLine.LeftBar.fillAmount = 0f;
            GtLLine.RightBar.fillAmount = 0f;
            GtLLine.center.color = ActiveColor;
        } else {
            GtLLine.center.color = InactiveColor;
            GtLLine.LeftBar.fillAmount = owc.GloomyMagnitude;
            GtLLine.RightBar.fillAmount = owc.LushMagnitude;
        }

        // OTC
        if (owc.OTCIsZero())
        {
            OtCLine.LeftBar.fillAmount = 0f;
            OtCLine.RightBar.fillAmount = 0f;
            OtCLine.center.color = ActiveColor;
        } else {
            OtCLine.center.color = InactiveColor;
            OtCLine.LeftBar.fillAmount = owc.OrderMagnitude;
            OtCLine.RightBar.fillAmount = owc.ChaosMagnitude;
        }
    }
}
