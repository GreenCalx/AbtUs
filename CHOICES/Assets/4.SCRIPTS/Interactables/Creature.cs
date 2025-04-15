using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using UnityEngine.Events;
public class Creature : BSTAgent
{
    [Header("Creature")]
    public Transform modelTransform;
    public OWCEnabler enabler;

    protected Terrain terrain;

    public Feedback killFeedback;

    public Rigidbody self_RB;
    
    [Header("Flags")]
    public bool isDead = false;
    public bool isFrozen = false;

    [Header("Internals")]
    public UnityAction<Creature> deathCallbacks;

    private void Awake()
    {
        modelTransform = GetComponentInChildren<MeshRenderer>().transform;
        self_RB = transform.GetComponentInChildren<Rigidbody>();
        navAgent = transform.GetComponentInChildren<NavMeshAgent>();
        enabler = GetComponentInParent<OWCEnabler>();
        terrain = transform.GetComponent<ModelTools>()?.GetTerrain();
    }
    /*
    static private void Spawn(Vector3 pos, Transform parent)
    {
        Creature newCreature = Instantiate(creaturePrefab, parent);
        newCreature.transform.position = pos;
        if(newCreature.enabler == null)
        {
            Debug.LogWarning("Creature " + newCreature.name + "has no pool");
        }
    }
    */
    public void Kill()
    {
        if (isFrozen) 
            return;

        if( killFeedback != null) 
        { killFeedback.use(); }

        if(enabler != null)
        { enabler.Remove(this.gameObject); }

        deathCallbacks.Invoke(this);
    }
}
