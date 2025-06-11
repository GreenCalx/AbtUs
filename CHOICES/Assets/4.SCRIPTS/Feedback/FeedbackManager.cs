using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using static EventLog;

public enum FeedbackType
{
    luminance,
    darkness,
    obj_nature,
    player,
    proximity,
    movements,
    birth,
    death,
    duplication,
    shattering,
    kill
};

[System.Serializable]
public class FeedbackData
{

    //public int tag; // Tags to evaluate groups separatly
    public OWCAxis tag;
    public FeedbackType type; // enum to define to refer to created feedbacks
    public float baseValue;
    public float loopStrength = 0f;

    public bool useIntertia = false;
    [Tooltip("X: Time [0,1] \nY: Chance to call spawncheck")]
    public AnimationCurve intertiaCurve;
    public float timeToReachMaxInertia = 5f;
}

public class FeedbackMatrix
{
    public float constCommonInput = 0.5f;
    public List<List<Feedback>> feedbacks;
    // tag to output dic
    public Dictionary<int, float> outputs;
    public List<Feedback> syncedFeedbacks;
    public FeedbackMatrix(int n_tags)
    {
        outputs = new Dictionary<int, float>();
        feedbacks = new List<List<Feedback>>();
        syncedFeedbacks = new List<Feedback>();
        for (int i = 0; i < n_tags; i++)
        {
            feedbacks.Add(new List<Feedback>(0));
            outputs.Add(i, constCommonInput);
        }
    }
    public Feedback BuildFeedback(GameFeedback iGFB)
    {
        FeedbackData fData = iGFB.fData;
        Feedback new_fb = new Feedback(iGFB);

        new_fb.baseValue = fData.baseValue;
        new_fb.loopStrength = fData.loopStrength;
        new_fb.tag = (int)fData.tag;
        new_fb.fType = fData.type;
        new_fb.useIntertia = fData.useIntertia;
        new_fb.intertiaCurve = fData.intertiaCurve;
        new_fb.timeToReachMaxInertia = fData.timeToReachMaxInertia;

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
            if (fb.isDirty)
                fb.Exec();
            aggregate += fb.output;
        }
        outputs[feedbacks.IndexOf(iRow)] = constCommonInput + aggregate;
    }

    public void RefreshTag(int iTag)
    {
        RefreshRow(feedbacks[iTag]);
    }

    public void SetDirty(GameFeedback iF)
    {
        foreach (Feedback f in feedbacks[ (int) iF.fData.tag])
        {
            if (f.originator == iF)
            {
                f.isDirty = true;
                return;
            }
        }
        FAIL("Feedback Matrix : SetDirty on feedback because originators couldn't be match or found. Originator = " + iF.gameObject.name);

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

    public void OnSync()
    {
        RefreshAll();
    }

}
public interface IFeedbackEval
{
    public float feedbackEvaluator();
}
public class Feedback
{
    public int tag { get; set; }
    public GameFeedback originator;
    public bool isDirty = true;
    public float output
    {
        get
        {
            loopValue = (baseValue + loopValue) * influence * loopStrength;
            return (baseValue + loopValue) * influence;
        }
    }
    private float __influence { get; set; }
    public float influence { get { return __influence; } set { __influence = Mathf.Clamp(value, -2f, 2f); } } // 0f => disabled
    public float loopStrength { get; set; } // 0f => disabled
    public FeedbackType fType;
    public bool isSync;
    public float baseValue { get; set; }
    public evaluate evaluator { get; set; }
    public delegate float evaluate();
    protected float loopValue
    { get; set; }

    public bool useIntertia = false;
    [Tooltip("X: Time [0,1] \nY: Chance to call spawncheck")]
    public AnimationCurve intertiaCurve;
    public float timeToReachMaxInertia = 5f;
    private float lastEvaluationChangedTime;
    private float targetEval = 0f;
    private float currEval = 0f;
    private float prevEval = 0f;

    public Feedback(GameFeedback iOriginator)
    {
        originator = iOriginator;
    }

    public void Assign(evaluate iEvaluator)
    { evaluator = iEvaluator; }

    public void Exec()
    {
        float eval = evaluator();

        if (!useIntertia)
        {
            influence = eval;
        }
        else if (eval != targetEval)
        {
            float frac = Mathf.Clamp01((Time.time - lastEvaluationChangedTime) / timeToReachMaxInertia);
            float lerpFac = intertiaCurve.Evaluate(frac);
            currEval = Utils.Lerp(prevEval, targetEval, lerpFac);

            targetEval = eval;
            lastEvaluationChangedTime = Time.time;
            influence = currEval;
        }
        else
        {
            float frac = Mathf.Clamp01((Time.time - lastEvaluationChangedTime) / timeToReachMaxInertia);
            float lerpFac = intertiaCurve.Evaluate(frac);
            prevEval = Utils.Lerp(currEval, targetEval, lerpFac);
            influence = prevEval;
        }
        isDirty = false;
    }
}

public class FeedbackManager : MonoBehaviour
{
    private static FeedbackManager instance = null;
    public static FeedbackManager Instance => instance;
    [SerializeField]
    private OverWorldControl OWC;
    public FeedbackMatrix fMatrix;
    public float syncStep = 0.2f;
    private float lastSyncTime = 0f;
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
        lastSyncTime = Time.time;
    }

    public void RegisterGameFeedback(GameFeedback iF, Feedback.evaluate iEvaluator)
    {
        Feedback new_fb = fMatrix.BuildFeedback(iF);
        fMatrix.AddFeedback(new_fb);
        new_fb.Assign(iEvaluator);
    }

    public void ChangeOWC()
    {
        OWC.SetOrderToChaos(fMatrix.outputs[(int)OWCAxis.OTC]);
        OWC.SetMineralToOrganic(fMatrix.outputs[(int)OWCAxis.MTO]);
        OWC.SetGloomyToLush(fMatrix.outputs[(int)OWCAxis.GTL]);

        // Debug.Log("MATRIX OUTPUT : ");
        INFO("output OTC : " + fMatrix.outputs[(int)OWCAxis.OTC]);
        INFO("output MTO : " + fMatrix.outputs[(int)OWCAxis.MTO]);
        INFO("output GTL : " + fMatrix.outputs[(int)OWCAxis.GTL]);
    }

    public void AsyncNotif(GameFeedback iF)
    {
        fMatrix.SetDirty(iF);
    }

    void Update()
    {
        if ((Time.time - lastSyncTime) > syncStep)
        {
            fMatrix.OnSync();
            ChangeOWC();
            lastSyncTime = Time.time;
        }
        
    }

}
