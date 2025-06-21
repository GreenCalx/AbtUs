using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
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
    public List<FeedbackChain> feedbacks;
    // tag to output dic
    public Dictionary<int, float> outputs;
    public FeedbackMatrix(int n_tags)
    {
        outputs = new Dictionary<int, float>();
        feedbacks = new List<FeedbackChain>();
        for (int i = 0; i < n_tags; i++)
        {
            feedbacks.Add(new FeedbackChain());
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
    {
        if (iF.tag >= feedbacks.Count)
            return;
        if (feedbacks[iF.tag].Contains(iF))
            return;
        feedbacks[iF.tag].Add(iF);
    }

    public void RemoveFeedback(Feedback iF)
    {
        if (iF.tag >= feedbacks.Count)
            return;
        if (!feedbacks[iF.tag].Contains(iF))
            return;
        feedbacks[iF.tag].Remove(iF);
    }

    public void RefreshRow(FeedbackChain iRow)
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
        foreach (Feedback f in feedbacks[(int)iF.fData.tag])
        {
            if (f.originator == iF)
            {
                f.isDirty = true;
                return;
            }
        }
        FAIL("Feedback Matrix : SetDirty on feedback because originators couldn't be match or found. Originator = " + iF.gameObject.name);

    }

    public bool RefreshAll()
    {
        bool refreshed = false;
        foreach (FeedbackChain row in feedbacks)
        {
            if (row.Count == 0)
                continue;
            if (!row.isDirty)
                continue;

            RefreshRow(row);
            refreshed = true;
        }
        return refreshed;
    }

    public bool OnSync()
    {
        return RefreshAll();
    }

}
public interface IFeedbackEval
{
    public float feedbackEvaluator();
}

public class FeedbackChain : IEnumerable<Feedback>
{
    private List<Feedback> m_feedbacks = new List<Feedback>(0);
    public Feedback this[int i]
    {
        get => m_feedbacks[i];
        set => m_feedbacks[i] = value;
    }
    public bool isDirty = false;
    public int Count { get => m_feedbacks.Count; }
    public IEnumerator<Feedback> GetEnumerator() { foreach(Feedback f in m_feedbacks){ yield return f; }}
    public void Add(Feedback iF) { m_feedbacks.Add(iF); }
    public void Remove(Feedback iF) { m_feedbacks.Remove(iF); m_feedbacks = m_feedbacks.Where(e => e != null).ToList(); }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
public class Feedback
{
    public int tag { get; set; }
    public GameFeedback originator;
    private FeedbackChain matrixRow;
    private bool m_isDirty;
    public bool isDirty
    {
        set
        {
            if (value)
                matrixRow.isDirty = true;
            m_isDirty = value;
        }
        get { return m_isDirty; } 
    }
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
        float otc = fMatrix.outputs[(int)OWCAxis.OTC];
        float mto = fMatrix.outputs[(int)OWCAxis.MTO];
        float gtl = fMatrix.outputs[(int)OWCAxis.GTL];

        if (otc != OWC.OrderToChaos)
        {
            OWC.SetOrderToChaos(otc);
            INFO("output OTC : " + fMatrix.outputs[(int)OWCAxis.OTC]);
        }

        if (mto != OWC.MineralToOrganic)
        {
            OWC.SetMineralToOrganic(mto);
            INFO("output MTO : " + fMatrix.outputs[(int)OWCAxis.MTO]);
        }

        if (gtl != OWC.GloomyToLush)
        {
            OWC.SetGloomyToLush(gtl);
            INFO("output GTL : " + fMatrix.outputs[(int)OWCAxis.GTL]);
        }
    }

    public void AsyncNotif(GameFeedback iF)
    {
        fMatrix.SetDirty(iF);
    }

    void Update()
    {
        if ((Time.time - lastSyncTime) > syncStep)
        {
            if (fMatrix.OnSync())
                ChangeOWC();
            lastSyncTime = Time.time;
        }
        
    }

}
