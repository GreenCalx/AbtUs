using UnityEngine;

public class CirclePath : MonoBehaviour
{
    public Vector3 circleCenter;
    public float radius = 1f;
    public float circleRotationOffset = 90f;

    [Header("Render Path")]
    public bool renderOnStart = true;
    private bool m_DoRenderPath = false;
    public bool DoRenderPath {
        get { return m_DoRenderPath; }
        set { m_DoRenderPath = value; if (value) {DrawPath();} }
    }
    public LineRenderer pathRenderer;
    public int subdivFactor = 1;

    void Start()
    {
        if (renderOnStart)
        { DoRenderPath = true; }
    }

    public Vector3 GetCoord(float iAngleInDegree)
    {
        return new Vector3( circleCenter.x + radius * Mathf.Cos( (iAngleInDegree /*+ circleRotationOffset*/)  * Mathf.PI/180f), 
                            circleCenter.y + radius * Mathf.Sin( (iAngleInDegree /*+ circleRotationOffset*/) * Mathf.PI/180f ), 
                            circleCenter.z);
    }

    public float GetAngle(Vector3 iCoord)
    {
        //float angle =   Mathf.Atan2(iCoord.x - circleCenter.x, iCoord.y-circleCenter.y) * Mathf.Rad2Deg;
        //return angle <= 180? angle: angle - 360;
        float angle = Mathf.Atan2(iCoord.y-circleCenter.y, iCoord.x - circleCenter.x) * Mathf.Rad2Deg;
        angle = angle < 0 ? 360f+angle : angle;
        return angle;
    }

    public void DrawPath()
    {
        if (pathRenderer==null)
            return;
        
        int resolution = subdivFactor * 4;
        pathRenderer.positionCount = resolution;
        pathRenderer.loop = true;
        Vector3[] points = new Vector3[resolution];
        float resolutionStep = 360f / (float)resolution;
        for (int i=0; i < resolution; i++)
        {
            points[i] = GetCoord((float)i * resolutionStep);
        }
        pathRenderer.SetPositions(points);
    }
}
