using UnityEngine;

public class ModelAutoRot : MonoBehaviour
{
    public bool ActivateRot = true;
    public float rotSpeedX = 1f;
    public float rotSpeedY = 1f;
    public float rotSpeedZ = 1f;

    // Update is called once per frame
    void Update()
    {
        if (ActivateRot)
        {
            transform.Rotate( new Vector3(rotSpeedX* Time.deltaTime,rotSpeedY* Time.deltaTime,rotSpeedZ * Time.deltaTime));
        }
    }
}
