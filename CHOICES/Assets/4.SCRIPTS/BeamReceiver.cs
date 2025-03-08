using UnityEngine;
using UnityEngine.Events;

public class BeamReceiver : MonoBehaviour
{
    public bool isReceiving = false;
    public UnityEvent callbackOnReceive;
    public UnityEvent callbackOnStopReceive;

    public void Receive()
    {
        if (isReceiving)
            return;
        isReceiving = true;
        callbackOnReceive.Invoke();
    }

    public void StopReceive()
    {
        if (!isReceiving)
            return;
        isReceiving = false;
        callbackOnStopReceive.Invoke();
    }
}
