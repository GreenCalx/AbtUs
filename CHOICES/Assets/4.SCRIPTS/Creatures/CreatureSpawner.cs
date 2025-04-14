using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner<T> : MonoBehaviour where T : Creature
{
    [Header("Debug")]
    public bool spawnOne = false;
    [Header("Mand Refs")]
    public List<GameObject> creaturesRefs;
    public Bounds spawnBounds;
    public BSTAgentPool agentPool;
    public OWCEnabler enabler;
    private List<T> spawnedCreatures = new List<T>();

    private float xmin;
    private float xmax;
    private float ymin;
    private float ymax;
    private float zmin;
    private float zmax;

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

    protected virtual void NotifyBSTPool(T iSpawned) {}
    public void Spawn(int iNumber)
    {
        for (int i=0; i < iNumber; i++)
        {
            int selected = Random.Range(0, creaturesRefs.Count);
            GameObject new_C = Instantiate(creaturesRefs[selected]);

            new_C.transform.parent = transform;
            new_C.transform.localPosition = Vector3.zero;
            
            // sample position within bounds
            transform.position = new Vector3(    Random.Range(xmin,xmax), 
                                                 Random.Range(ymin,ymax), 
                                                 Random.Range(zmin,zmax));

            T as_C = new_C.GetComponent<T>();
            spawnedCreatures.Add( as_C );

            NotifyBSTPool(as_C);
        }
    }
}