using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public bool isPlayerCam = false;
    public Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam==null)
            cam = GetComponent<Camera>();

        if (isPlayerCam)
        {
            Managers.Instance.Camera.initPlayerCamLocalPos = transform.localPosition;
            Managers.Instance.Camera.initPlayerCamLocalRot = transform.localRotation;
            Managers.Instance.Camera.initPlayerFOV = cam.fieldOfView;
            Managers.Instance.Camera.initPlayerCamParent = transform.parent;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TryRCFromScreenCenter(out RaycastHit oRayHit , float iDistance = Mathf.Infinity)
    {
        Ray ray = GetRayFromScreenCenter();
        return Physics.Raycast(ray, out oRayHit, iDistance );
    }

    public Ray GetRayFromScreenCenter()
    {
        return cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
    }
}
