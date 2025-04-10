using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform player;
    public Transform portalCamera;
    public Transform reciever;

    public bool negative;

 
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlayerController>())
        {
            player.position = portalCamera.position - player.gameObject.GetComponentInChildren<Camera>().transform.localPosition;
            float targetYaw = portalCamera.eulerAngles.y;
            player.transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }
    }


}
