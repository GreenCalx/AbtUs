using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class InsectBehaviour : Creature
{
    public float moveDistance = 5f; 
    public float waitTime = 2f; 
    private bool waiting = false;

    public float killWalkingSpeed = 2f;

    private Vector3 terrainSize;
    private Vector3 terrainPos;
    [SerializeField]
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, moveDistance);
    }

    private void Start()
    {
        navAgent.updateUpAxis = false; 
        terrainSize = terrain.terrainData.size;
        terrainPos = terrain.transform.position;
    }

    public void Idle()
    {
        if (!navAgent.enabled) { return;}

        Vector2 terrainRelativePos = new Vector2((modelTransform.position.x - terrainPos.x) / terrainSize.x, (modelTransform.position.z - terrainPos.z) / terrainSize.z);
        transform.up = terrain.terrainData.GetInterpolatedNormal(terrainRelativePos.x, terrainRelativePos.y);

        Debug.DrawLine(modelTransform.position, modelTransform.position + 10 * terrain.terrainData.GetInterpolatedNormal(terrainRelativePos.x, terrainRelativePos.y));
        if (!waiting && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    public void ResetDestination()
    {
        if (navAgent.hasPath)
            navAgent.ResetPath();
        
    }

    IEnumerator WaitAndTurn()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        if (navAgent.enabled)
        {
            MoveToRandomPosition();
        }
        waiting = false;
    }

    void MoveToRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveDistance; 
        randomDirection += transform.position;
        randomDirection.y = terrain.SampleHeight(transform.position);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveDistance, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
        }
        else
        {
            MoveToRandomPosition();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<Rigidbody>().linearVelocity.magnitude > killWalkingSpeed)
            {
                isDead = true;
            }
        }
    }
}