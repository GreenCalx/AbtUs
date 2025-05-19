using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CreatureSpawner<T> : MonoBehaviour, IFeedbackEval where T : Creature
{
    [Header("Debug")]
    public bool spawnOne = false;
    public bool DespawnOne = false;
    [Header("Mand Refs")]
    public List<GameObject> creaturesRefs;
    public Bounds spawnBounds;
    public BSTAgentPool agentPool;
    public OWCEnabler enabler;

    [Header("Tweaks")]
    public int MAX_SPAWNS = 20;
    public WORLD_AXIS axis;
    [Tooltip("X: Axis Magnitude [0,1] \nY: Population in Fraction of MAX_SPAWNS [0,1]")]
    public AnimationCurve spawnsByMagnitudeCurve;

    [Header("Optional")]
    public Terrain relatedTerrain;
    public GameFeedback gameFeedback;

    [Header("Internals")]
    protected List<T> spawnedCreatures = new List<T>();

    private float xmin;
    private float xmax;
    private float ymin;
    private float ymax;
    private float zmin;
    private float zmax;
    public int expectedPopulation = 0;

    protected float maxSpawnMulFactor = 1f;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0f, 0.2f, 0.5f);
        Gizmos.DrawCube(
                transform.position, spawnBounds.size
             );
    }

    void Start()
    {
        InitBounds();
        InitFeedback();
    }

    void InitFeedback()
    {
        if (gameFeedback == null)
            Debug.LogError("MUST HAVE A GAMEFEEDBACK DEFINED FOR CREATURE SPAWNER.");
        gameFeedback.Init(this);
    }

    void Update()
    {
        if (spawnOne)
        {
            spawnOne = false;
            Spawn(1);
        }
        if (DespawnOne)
        {
            DespawnOne = false;
            DeSpawn(1);
        }
        CheckForSpawns();
    }

    void OnDisable()
    {
        DeSpawnAll();
    }

    protected void CheckForSpawns()
    {
        expectedPopulation = (int)Mathf.Ceil(spawnsByMagnitudeCurve.Evaluate(OverWorldControl.Instance.GetAxisMagnitude(axis)) * MAX_SPAWNS * maxSpawnMulFactor );
        int popDelta = expectedPopulation - spawnedCreatures.Count;
        if (popDelta > 0)
        { Spawn(popDelta); }
        else if (popDelta < 0)
        { DeSpawn(Mathf.Abs(popDelta)); }
    }

    protected void InitBounds()
    {
        xmin = transform.position.x + spawnBounds.min.x * transform.lossyScale.x;
        xmax = transform.position.x + spawnBounds.max.x * transform.lossyScale.x;

        ymin = transform.position.y + spawnBounds.min.y * transform.lossyScale.y;
        ymax = transform.position.y + spawnBounds.max.y * transform.lossyScale.y;

        zmin = transform.position.z + spawnBounds.min.z * transform.lossyScale.z;
        zmax = transform.position.z + spawnBounds.max.z * transform.lossyScale.z;
    }

    protected virtual void GetFeedbackValue() {}
    protected virtual void UpdateOWCFeedback() {}
    
    public virtual float feedbackEvaluator(){ return 0f; }
    public virtual void InitFromFeedbackFunc(float iFeedbackInfluence) { }

    protected virtual void NotifyBSTPoolSpawn(T iSpawned) {}
    protected virtual void NotifyBSTPoolDeSpawn(T iSpawned) {}
    public void Spawn(int iNumber)
    {
        for (int i = 0; i < iNumber; i++)
        {
            int selected = Random.Range(0, creaturesRefs.Count);
            GameObject new_C = Instantiate(creaturesRefs[selected]);

            new_C.transform.parent = transform;
            new_C.transform.localPosition = Vector3.zero;


            T as_C = new_C.GetComponent<T>();
            if (relatedTerrain != null)
            {
                as_C.terrain = relatedTerrain;
            }
            // sample position within bounds
            new_C.transform.position = new Vector3(Random.Range(xmin, xmax),
                                                 Random.Range(ymin, ymax),
                                                 Random.Range(zmin, zmax));

            spawnedCreatures.Add(as_C);
            as_C.deathCallbacks += DeSpawnTarget;
            as_C.agentPool = agentPool;
            NotifyBSTPoolSpawn(as_C);
        }

        gameFeedback.Refresh();
    }

    public void DeSpawn(int iNumber)
    {
        List<T> ToDelete = new List<T>();
        for (int i = 0; i < iNumber; i++)
        { ToDelete.Add(spawnedCreatures[i]); }

        for (int i = 0; i < ToDelete.Count; i++)
        {
            spawnedCreatures.Remove(ToDelete[i]);
            NotifyBSTPoolDeSpawn(ToDelete[i]);
            Destroy(ToDelete[i].gameObject);
        }

        gameFeedback.Refresh();
    }

    public void DeSpawnTarget(Creature iTarget)
    {
        T as_T = iTarget as T;
        if (!spawnedCreatures.Contains(as_T))
            return;
        spawnedCreatures.Remove(as_T);
        NotifyBSTPoolDeSpawn(as_T);
        Destroy(as_T.gameObject);

        gameFeedback.Refresh();
    }

    public void DeSpawnAll()
    {
        DeSpawn(spawnedCreatures.Count);
    }

    public void UpdateSpawnMulFactor(int iVal)
    {
        maxSpawnMulFactor = 1f + (1f - (1f/((Mathf.Pow(iVal,2)/4) + 1f)));
        Debug.Log(maxSpawnMulFactor);
    }
}