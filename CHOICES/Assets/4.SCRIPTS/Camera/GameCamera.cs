using UnityEngine;
using UnityEngine.Rendering;

public class GameCamera : MonoBehaviour
{
    public bool isPlayerCam = false;
    public Camera cam;
    public Vector3 camRotAsEulers;
    public LayerMask InteractibleRCMaskLayer;
    public RenderTexture _lumRT;

    private CommandBuffer _commandBuffer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        camRotAsEulers = transform.eulerAngles;

        if (isPlayerCam)
        {
            // Camera.onPostRender += OnPostRenderCB;

            _commandBuffer = new CommandBuffer();
            var lookMatrix = Matrix4x4.LookAt(transform.position, transform.position + transform.forward, transform.up);
            var scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            var viewMatrix = scaleMatrix * lookMatrix.inverse;
            _commandBuffer.SetViewMatrix(viewMatrix);
            _commandBuffer.SetProjectionMatrix(cam.projectionMatrix);

            _commandBuffer.name = "LumBuff";
            int lumRT = Shader.PropertyToID("_lumRT");
            _commandBuffer.GetTemporaryRT(lumRT, 1920, 1080, 0, FilterMode.Point, RenderTextureFormat.RG16, RenderTextureReadWrite.Linear, 0, false);
            _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, lumRT);
            _commandBuffer.Blit(lumRT, _lumRT);
            cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, _commandBuffer);
            
            Graphics.ExecuteCommandBuffer(_commandBuffer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerCam)
            Managers.Instance.Camera.lastRender = cam.activeTexture;
        Graphics.ExecuteCommandBuffer(_commandBuffer);
    }

    public RenderTexture GetLumRT()
    {
        
        return _lumRT;
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
