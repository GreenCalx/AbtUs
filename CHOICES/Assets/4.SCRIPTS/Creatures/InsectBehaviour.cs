using UnityEngine;
using System.Collections;

public class InsectBehaviour : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    public float rotationSpeed = 120f;
    public float minBreakTime = 1f;
    public float maxBreakTime = 3f;
    public float minRuningTime = 2f;
    public float maxRuningTime = 2f;


    private float speed;
    private Vector3 direction;
    private bool moving = true;

    private ModelTools mt;
    void Start()
    {
        mt = GetComponent<ModelTools>();
        StartCoroutine(DeplacementRoutine());
    }

    IEnumerator DeplacementRoutine()
    {
        while (true)
        {
            if (moving)
            {
                ChangerDirection();
                speed = Random.Range(minSpeed, maxSpeed);

                yield return new WaitForSeconds(Random.Range(minRuningTime,maxRuningTime));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(minBreakTime, maxBreakTime));
                moving = true;
            }
        }
    }

    void ChangerDirection()
    {
        float angle = Random.Range(-90f, 90f);
        transform.Rotate(Vector3.right * angle);
        direction = transform.up;
    }

    void Update()
    {
        if (moving)
        {
            Vector3 newPos = transform.position + direction * speed * Time.deltaTime;
            newPos.y = mt.getTerrainY();
            transform.position = newPos;
        }

        // Arrêt aléatoire
        if (Random.Range(0f, 1f) < 0.01f)
        {
            moving = false;
        }
    }
}