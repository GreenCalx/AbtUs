using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
    public class FeedbackVariable
    {
        public enum Type { GTL_glow, GTL_sanity, OTC_world, OTC_player, MTO_proximity, MTO_movements, MTO_livings };

        public OWCAxis axis;
        public Type type;
        public float baseValue;
        public float maxValue;  
        public float minValue;

        private float Value;
        public float _value
        {
            get { return Value; }
            set { Value = Mathf.Clamp(value, minValue, maxValue); }
        }
    }

public class FeedbackManager : MonoBehaviour
{


    private static FeedbackManager instance = null;
    public static FeedbackManager Instance => instance;


    [SerializeField]
    private OverWorldControl OWC;

    [SerializeField]
    public List<FeedbackVariable> feedbackVariables;

    public Dictionary<FeedbackVariable.Type, FeedbackVariable> feedbackVariablesDict;
    private List<FeedbackVariable> GTLfeedbackVariables = new List<FeedbackVariable>();
    private List<FeedbackVariable> OTCfeedbackVariables = new List<FeedbackVariable>();
    private List<FeedbackVariable> MTOfeedbackVariables = new List<FeedbackVariable>();

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

        float base_gtl = 0;
        float base_otc = 0;
        float base_mto = 0;
        feedbackVariablesDict = new Dictionary<FeedbackVariable.Type, FeedbackVariable>();

        foreach (FeedbackVariable fVar in feedbackVariables)
        {
            feedbackVariablesDict.Add(fVar.type, fVar);
            fVar._value = fVar.baseValue;
            if (fVar.axis == OWCAxis.GTL)
            {
                GTLfeedbackVariables.Add(fVar);
                base_gtl += fVar._value;
            }
            else if (fVar.axis == OWCAxis.OTC)
            {
                OTCfeedbackVariables.Add(fVar);
                base_mto += fVar._value;
            }
            else if (fVar.axis == OWCAxis.MTO)
            {
                MTOfeedbackVariables.Add(fVar);
                base_otc += fVar._value;
            }
        }
        OWC.setAxisValue(OWCAxis.GTL, base_gtl);
        OWC.setAxisValue(OWCAxis.OTC, base_otc);
        OWC.setAxisValue(OWCAxis.MTO, base_mto);

    }


    public void ChangeOWC(Feedback feedback) 
    {

        FeedbackVariable var = feedbackVariablesDict[feedback.feedback_type];
        float currentValue = var._value;

        float axisValue = OWC.getAxisValue(var.axis);
        feedback.applyFeedback(var, axisValue);
        float deltaValue = var._value - currentValue;
        if(deltaValue != 0)
        {
            OWC.setAxisValue(var.axis, axisValue + deltaValue);
        }
        
    }

}
