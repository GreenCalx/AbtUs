using UnityEngine;

public class LuminanceCam : MonoBehaviour
{
    void OnEnable()
    {
            // _commandBuffer = new CommandBuffer();
            // _commandBuffer.name = "Test CommandBuffer";
            // int testRT = Shader.PropertyToID("_testRT");
            // _commandBuffer.GetTemporaryRT(testRT, 500, 500, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 0, false);
            // _commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, testRT);
            // Camera.main.AddCommandBuffer(CameraEvent.BeforeImageEffects, _commandBuffer);
    }
}
