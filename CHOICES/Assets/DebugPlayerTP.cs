using UnityEngine;

public class DebugPlayerTP : MonoBehaviour
{
    public PlayerController playerRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef.transform.position = transform.position;
        playerRef.transform.rotation = transform.rotation;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent red cube at the transforms position
        Gizmos.color = new Color(0.5f, 0.35f, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1f,2f,1f));
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.yellow);
    }
}
