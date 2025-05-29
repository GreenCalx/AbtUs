using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ObjectChainLR : MonoBehaviour
{
    public LineRenderer LR;
    public Transform holderAnchor;
    public Transform connectedAnchor;
    private Vector3 lastHolderAnchor;
    private Vector3 lastConnectedAnchor;

    public void Init(Transform iHolderAnchor, Transform iLastConnectedAnchor)
    {
        LR = GetComponent<LineRenderer>();
        LR.positionCount = 2;
        holderAnchor = iHolderAnchor;
        connectedAnchor = iLastConnectedAnchor;
    }

    void Update()
    {
        if (Vector3.Distance(lastHolderAnchor, holderAnchor.transform.position) > 0.1f)
        {
            LR.SetPosition(0, holderAnchor.transform.position);
            lastHolderAnchor = holderAnchor.transform.position;
        }
        
        if (Vector3.Distance(lastConnectedAnchor, connectedAnchor.transform.position) > 0.1f)
        {
            LR.SetPosition(1, connectedAnchor.transform.position);
            lastConnectedAnchor = connectedAnchor.transform.position;
        }
    }
}
