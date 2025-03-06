using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class BeamCaster : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public LayerMask beamLayerMaskHit;
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
        if (Physics.Raycast(transform.position, transform.up, out hit, beamLayerMaskHit ))
        {
            points[1] = hit.point;

            BeamReceiver br = hit.collider.gameObject.GetComponent<BeamReceiver>();
            if (!!br && (currentReceiver==null))
            { 
                br.Receive(); 
                currentReceiver = br; 
            } else if (!!currentReceiver)
            {
                currentReceiver.StopReceive();
                currentReceiver = null;
            }

        } else {
            points[1] = transform.forward * 10f;
        }
        lineRenderer.SetPositions(points);
    }
}
