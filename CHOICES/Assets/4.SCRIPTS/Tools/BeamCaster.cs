using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class BeamCaster : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public LayerMask beamLayerMaskHit;
    public LayerMask beamReceiverMaskHit;
    public float defaultBeamLength = 10f;

    private BeamReceiver currentReceiver;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.loop = false;
        lineRenderer.positionCount = 2;
        
        CastBeam();
    }
    void Update()
    {

    }

    public void CastBeam()
    {
        if (lineRenderer==null)
            return;
        Vector3[] points = new Vector3[2];
        points[0] = transform.position;
        
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up, out hit, Mathf.Infinity, beamLayerMaskHit ))
        {
            points[1] = hit.point;

            // Do a layer specific raycast to check if beamreceiver
            RaycastHit hitReceiver;
            if (Physics.Raycast(transform.position, transform.up, out hitReceiver, Mathf.Infinity, beamReceiverMaskHit))
            {
                BeamReceiver br = hitReceiver.collider.gameObject.GetComponent<BeamReceiver>();
                if (!!br && (currentReceiver==null))
                {
                    br.Receive(); 
                    currentReceiver = br; 
                } 
            } else {
                if ((currentReceiver!=null)&&(currentReceiver.isReceiving))
                {
                    currentReceiver.StopReceive();
                    currentReceiver = null;
                }
            }

        } else {
            points[1] = transform.forward * 10f;
        }
        lineRenderer.SetPositions(points);
    }
}
