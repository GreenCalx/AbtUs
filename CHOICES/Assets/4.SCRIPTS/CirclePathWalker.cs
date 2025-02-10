using UnityEngine;

public class CirclePathWalker : MonoBehaviour
{
    [Range(0f,360f)]
    public float angle = 0f;
    public CirclePath path;

    void Update()
    {
        transform.localPosition = path.GetCoord(angle);
    }
}
