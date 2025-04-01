using UnityEngine;

#if UNITY_EDITOR
public class ExitPortalGizmo : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        	Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.5f, 0.5f, 0, 0.5f);
        Gizmos.DrawCube(
                Vector3.zero, new Vector3(1f,0.1f,1.5f)
             );
        Debug.DrawRay(transform.position, -transform.up * 2, Color.blue);
    }
}
#endif
