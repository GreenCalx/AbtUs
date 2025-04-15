using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner<T> : MonoBehaviour where T : Creature
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

    private List<T> spawnedCreatures = new List<T>();

    private float xmin;
    private float xmax;
    private float ymin;
    private float ymax;
    private float zmin;
    private float zmax;
    private int expectedPopulation = 0;

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

    protected void CheckForSpawns()
    {
        expectedPopulation = (int)Mathf.Ceil(spawnsByMagnitudeCurve.Evaluate(OverWorldControl.Instance.GetAxisMagnitude(axis)) * MAX_SPAWNS );
        int popDelta = expectedPopulation - spawnedCreatures.Count;
        if (popDelta > 0)
        { Spawn(popDelta); }
        else if (popDelta < 0)
        { DeSpawn(popDelta); }
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

    protected virtual void NotifyBSTPoolSpawn(T iSpawned) {}
    protected virtual void NotifyBSTPoolDeSpawn(T iSpawned) {}
    public void Spawn(int iNumber)
    {
        for (int i=0; i < iNumber; i++)
        {
            int selected = Random.Range(0, creaturesRefs.Count);
            GameObject new_C = Instantiate(creaturesRefs[selected]);

            new_C.transform.parent = transform;
            new_C.transform.localPosition = Vector3.zero;
            
            // sample position within bounds
            new_C.transform.position = new Vector3(    Random.Range(xmin,xmax), 
                                                 Random.Range(ymin,ymax), 
                                                 Random.Range(zmin,zmax));

            T as_C = new_C.GetComponent<T>();
            spawnedCreatures.Add( as_C );
            as_C.deathCallbacks += DeSpawnTarget;
            NotifyBSTPoolSpawn(as_C);
        }
    }

    public void DeSpawn( int iNumber)
    {
        List<T> ToDelete = new List<T>();
        for (int i=0; i < iNumber; i++)
        { ToDelete.Add(spawnedCreatures[i]); }

        for (int i=0; i < ToDelete.Count; i++)
        {
            spawnedCreatures.Remove(ToDelete[i]);
            NotifyBSTPoolDeSpawn(ToDelete[i]);
            Destroy(ToDelete[i].gameObject);
        }
    }

    public void DeSpawnTarget(Creature iTarget)
    {
        T as_T = iTarget as T;
        if (!spawnedCreatures.Contains(as_T))
            return;
        spawnedCreatures.Remove(as_T);
        NotifyBSTPoolDeSpawn(as_T);
        Destroy(as_T.gameObject);
    }
}