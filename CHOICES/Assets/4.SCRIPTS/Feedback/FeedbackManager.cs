using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;

public enum FeedbackType
{
    glow,
    sanity,
    world,
    player,
    proximity,
    movements,
    birth,
    death,
    duplication,
    shattering
};

[System.Serializable]
public class FeedbackData
{

    //public int tag; // Tags to evaluate groups separatly
    public OWCAxis tag;
    public FeedbackType type; // enum to define to refer to created feedbacks
    public float baseValue;
    public float loopStrength = 0f;
    public bool isSynced = false;
}

public class FeedbackMatrix
{
    public float constCommonInput = 0.5f;
    public List<List<Feedback>> feedbacks;
    // tag to output dic
    public Dictionary<int, float> outputs;
    public FeedbackMatrix(int n_tags)
    {
        outputs = new Dictionary<int, float>();
        feedbacks = new List<List<Feedback>>();
        for (int i = 0; i < n_tags; i++)
        {
            feedbacks.Add(new List<Feedback>(0));
            outputs.Add(i, constCommonInput);
        }
    }
    public Feedback BuildFeedback(FeedbackData iFData)
    {
        Feedback new_fb = new Feedback();

        new_fb.baseValue = iFData.baseValue;
        new_fb.loopStrength = iFData.loopStrength;
        new_fb.tag = (int)iFData.tag;
        new_fb.isSync = iFData.isSynced;
        new_fb.fType = iFData.type;

        return new_fb;
    }

    public void AddFeedback(Feedback iF)
    { feedbacks[iF.tag].Add(iF); }

    public void RemoveFeedback(Feedback iF)
    { feedbacks[iF.tag].Remove(iF); }

    public void RefreshRow(List<Feedback> iRow)
    {
        float aggregate = 0f;
        foreach (Feedback fb in iRow)
        {
            fb.influence = fb.Exec();
            aggregate += fb.output;
        }
        outputs[feedbacks.IndexOf(iRow)] = constCommonInput + aggregate;
    }

    public void RefreshTag(int iTag)
    {
        RefreshRow(feedbacks[iTag]);
    }

    public void RefreshAll()
    {
        foreach (List<Feedback> row in feedbacks)
        {
            if (row.Count == 0)
                continue;
            RefreshRow(row);
        }
    }

}
public interface IFeedbackEval
{
    public float feedbackEvaluator();
}
public class Feedback
{
    public int tag { get; set; }
    public float output
    {
        get
        {
            loopValue = (baseValue + loopValue) * influence * loopStrength;
            return (baseValue + loopValue) * influence;
        }
    }
    private float __influence { get; set; }
    public float influence { get { return __influence; } set { __influence = Mathf.Clamp01(value); } } // 0f => disabled
    public float loopStrength { get; set; } // 0f => disabled
    public FeedbackType fType;
    public bool isSync;
    public float baseValue { get; set; }
    public evaluate evaluator { get; set; }
    public delegate float evaluate();
    protected float loopValue
    { get; set; }

    public void Assign(evaluate iEvaluator)
    { evaluator = iEvaluator; }

    public float Exec()
    {
        return evaluator();
    }
}

public class FeedbackManager : MonoBehaviour
{
    private static FeedbackManager instance = null;
    public static FeedbackManager Instance => instance;


    [SerializeField]
    private OverWorldControl OWC;

    public FeedbackMatrix fMatrix;

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
        fMatrix = new FeedbackMatrix(3);
    }

    public void Start()
    {
        OWC = OverWorldControl.Instance;
    }

    public void RegisterGameFeedback(GameFeedback iF, Feedback.evaluate iEvaluator)
    {
        Feedback new_fb = fMatrix.BuildFeedback(iF.fData);
        fMatrix.AddFeedback(new_fb);
        new_fb.Assign(iEvaluator);
    }

    public void ChangeOWC()
    {
        OWC.SetOrderToChaos(fMatrix.outputs[(int)OWCAxis.OTC]);
        OWC.SetMineralToOrganic(fMatrix.outputs[(int)OWCAxis.MTO]);
        OWC.SetGloomyToLush(fMatrix.outputs[(int)OWCAxis.GTL]);

        Debug.Log("MATRIX OUTPUT : ");
        Debug.Log("output OTC : " + fMatrix.outputs[(int)OWCAxis.OTC]);
        Debug.Log("output MTO : " + fMatrix.outputs[(int)OWCAxis.MTO]);
        Debug.Log("output GTL : " + fMatrix.outputs[(int)OWCAxis.GTL]);
    }

    public void AsyncNotif(GameFeedback iF)
    {
        fMatrix.RefreshTag((int)iF.fData.tag);
        ChangeOWC();
    }

    void Update()
    {
        // foreach (Feedback fVar in feedbackLoops)
        // {
        //     ChangeOWC(fVar);
        // }
    }

}
