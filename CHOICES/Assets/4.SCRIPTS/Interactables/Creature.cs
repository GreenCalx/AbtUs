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

    public Terrain terrain;

    public Feedback killFeedback;

    public Rigidbody self_RB;
    
    [Header("Flags")]
    public bool isDead = false;
    public bool isFrozen = false;

    [Header("Internals")]
    public UnityAction<Creature> deathCallbacks;

    private void Start()
    {
        if (modelTransform == null)
            modelTransform = GetComponentInChildren<MeshRenderer>().transform;
        if (self_RB==null)
            self_RB = transform.GetComponentInChildren<Rigidbody>();
        navAgent = transform.GetComponentInChildren<NavMeshAgent>();
        navAgent.enabled = false;
        enabler = GetComponentInParent<OWCEnabler>();
    }

    public void InitAgent()
    {
        if (navAgent.enabled)
        {return;}
        WarpAgentPos();
        navAgent.enabled = true;
    }

    public void WarpAgentPos()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        } else {
            Debug.LogWarning("Failed to Init Agent of : " + gameObject.name);
        }
    }

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
