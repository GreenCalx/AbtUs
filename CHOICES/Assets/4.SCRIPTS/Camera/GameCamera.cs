using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public bool isPlayerCam = false;
    public Camera cam;
    public Vector3 camRotAsEulers;
    public LayerMask InteractibleRCMaskLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        camRotAsEulers = transform.eulerAngles;

        if (isPlayerCam)
        {
            Camera.onPostRender += OnPostRenderCB;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerCam)
            Managers.Instance.Camera.lastRender = cam.activeTexture;   
    }

    public void VClampedRotation(Vector3 iDeltaRot, float iClampMin, float iClampMax)
    {
        camRotAsEulers += iDeltaRot * GameSettings.Instance.mouseSensivity;
        camRotAsEulers.x = Mathf.Clamp(camRotAsEulers.x, iClampMin, iClampMax);
        transform.eulerAngles = camRotAsEulers;
    }

    public bool TryRCFromScreenCenter(out RaycastHit oRayHit, float iDistance = Mathf.Infinity)
    {
        Ray ray = GetRayFromScreenCenter();
        return Physics.Raycast(ray, out oRayHit, iDistance, InteractibleRCMaskLayer);
    }

    public Ray GetRayFromScreenCenter()
    {
        return cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.forward * 5f, Color.blue);
        Debug.DrawRay(transform.position, transform.up * 5f, Color.green);
    }
    
    void OnPostRenderCB(Camera iCam)
    {
        Debug.Log("Post RENDEr");
        
    }
}
