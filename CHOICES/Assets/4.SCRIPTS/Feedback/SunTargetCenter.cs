using UnityEngine;

public class SunTargetCenter : MonoBehaviour
{
    private Transform player;
    public Transform sun;


    private void Awake()
    {
        player = transform.root.transform.Find("Player");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.position;
        transform.rotation = sun.rotation;
    }
}
