using UnityEngine;
using UnityEngine.Events;

public class BeamReceiver : MonoBehaviour
{
    public bool isReceiving = false;
    public UnityEvent callbackOnReceive;
    public UnityEvent callbackOnStopReceive;

    [Header("GFX Feedback")]
    public MeshRenderer RendererToUpdate;
    public Material OnReceiveMat;
    public Material OnStopReceiveMat;

    void Start()
    {
        RefreshMaterial();
    }
    public void Receive()
    {
        if (isReceiving)
            return;
        isReceiving = true;
        callbackOnReceive.Invoke();
        RefreshMaterial();
    }

    public void StopReceive()
    {
        if (!isReceiving)
            return;
        isReceiving = false;
        callbackOnStopReceive.Invoke();
        RefreshMaterial();
    }

    private void RefreshMaterial()
    {
        if (RendererToUpdate==null)
            return;
            
        RendererToUpdate.material = isReceiving ? OnReceiveMat : OnStopReceiveMat;
    }
}
