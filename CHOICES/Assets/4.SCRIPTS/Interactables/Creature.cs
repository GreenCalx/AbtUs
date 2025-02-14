using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Creature : InteractibleObject
{
    [Header("Creature")]
    protected Transform modelTransform;

    protected NavMeshAgent agent;
    public OWCEnabler enabler;

    protected Terrain terrain;

    public bool respawn = false;
    public float timeRespawn = 60;

    public bool invincible = false;
    public Feedback killFeedback;

    protected Vector3 firstPos;

    private void Awake()
    {
        modelTransform = GetComponentInChildren<MeshRenderer>().transform;
        RB = transform.GetComponentInChildren<Rigidbody>();
        agent = transform.GetComponentInChildren<NavMeshAgent>();
        enabler = transform.parent.GetComponent<OWCEnabler>();
        terrain = transform.GetComponent<ModelTools>()?.GetTerrain();
        firstPos = transform.position;

        if (availableActions.Length >= 1)
        {
            ChangeSelectedAction(availableActions[0]);
        }
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
        if (invincible) { return; }
        if( killFeedback != null) { killFeedback.use(); }
        if (respawn)
        {
            this.gameObject.SetActive(false);
            Invoke("Respawn", timeRespawn);
            return;
        }
        if(enabler != null)
        {
            enabler.Remove(this.gameObject);
        }

        Destroy(this.gameObject);
         
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
    }

    public override void Move()
    {
        invincible = true;
        agent.enabled = false;
        modelTransform.position = transform.position;
        transform.up = Vector3.up;
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }

        UIGame.Instance.ForceCursorToCloseHand();
        ActionCo = StartCoroutine(MoveCo());
    }

    public override void StopMove()
    {
        agent.enabled = true;
        invincible = false;
        if (ActionCo != null)
        {
            StopCoroutine(ActionCo);
            ActionCo = null;
        }
        UIGame.Instance.ForceCursorToOpenHand();
    }
}
