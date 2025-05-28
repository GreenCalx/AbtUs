using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform player_cam;
    public Transform portal;
    public Transform otherPortal;



    void Start()
    {
        
    }

  
    void LateUpdate()
    {
        Quaternion portalRotDiff = Quaternion.Euler(0,-90,0) * otherPortal.rotation * Quaternion.Inverse(portal.rotation);

        Vector3 newCamDir = portalRotDiff * player_cam.forward;

        Vector3 playerOffsetFromPrtal =  portal.position - player_cam.position;


        transform.rotation = Quaternion.LookRotation(newCamDir, Vector3.up);

        transform.position = otherPortal.position - portalRotDiff * playerOffsetFromPrtal;



     /*   Matrix4x4 m = portal.localToWorldMatrix * otherPortal.localToWorldMatrix * player_cam.localToWorldMatrix;

        portal_cam.SetPositionAndRotation(m.GetColumn(3), m.rotation);*/

    }
}
