using UnityEngine;

public class ChoiceHouse : MonoBehaviour
{
    public ChoicePortals portalsRoot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider iCollider)
    {
        if (iCollider.GetComponentInParent<PlayerController>())
        {
            portalsRoot.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider iCollider)
    {
        if (iCollider.GetComponentInParent<PlayerController>())
        {
            portalsRoot.gameObject.SetActive(false);
        }
    }
}
