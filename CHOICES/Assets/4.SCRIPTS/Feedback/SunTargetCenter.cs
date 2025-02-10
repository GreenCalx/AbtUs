using UnityEngine;

public class SunTargetCenter : MonoBehaviour
{
    public Transform player;
    public Transform sun;


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.position;
        transform.rotation = sun.rotation;
    }
}
