using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class ObjectChainLR : MonoBehaviour
{
    public LineRenderer LR;
    public Transform holderAnchor;
    public Transform connectedAnchor;
    private Vector3 lastHolderAnchor;
    private Vector3 lastConnectedAnchor;
    private BoxCollider box;

    public void Init(Transform iHolderAnchor, Transform iLastConnectedAnchor)
    {
        LR = GetComponent<LineRenderer>();
        LR.positionCount = 2;
        holderAnchor = iHolderAnchor;
        connectedAnchor = iLastConnectedAnchor;
        transform.position = iHolderAnchor.position;
        box = GetComponent<BoxCollider>();


        RefreshBox();
    }

    private void RefreshBox()
    {
        float dist = Vector3.Distance(holderAnchor.position, connectedAnchor.position);
        box.center = new Vector3(box.center.x, box.center.y, dist / 2f);
        box.size = new Vector3( box.size.x, box.size.y,dist);
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

        transform.position = holderAnchor.position;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, connectedAnchor.position - holderAnchor.position, 1f, 100f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        RefreshBox();

    }
}
