using UnityEngine;

public class SunTargetCenter : MonoBehaviour
{
    public Transform player;
    public Transform sun;

    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.position;
        transform.rotation = sun.rotation;
    }
}
