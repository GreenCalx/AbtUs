using UnityEngine;

public class ModelAutoRot : MonoBehaviour
{
    public bool RotAlongX = true;
    public float rotSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        if (RotAlongX)
        {
            transform.Rotate( new Vector3(0f,0f,rotSpeed * Time.deltaTime));
        }
    }
}
