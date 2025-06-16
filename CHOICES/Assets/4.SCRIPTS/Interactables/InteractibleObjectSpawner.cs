using UnityEngine;

public class InteractibleObjectSpawner : MonoBehaviour
{
    public GameObject prefab_InteractibleObject;
    private GameObject inst_InteractibleObject;

    void Update()
    {
        if (inst_InteractibleObject == null)
        {
            inst_InteractibleObject = Instantiate(prefab_InteractibleObject, transform.parent);
            inst_InteractibleObject.transform.localPosition = transform.localPosition;
            inst_InteractibleObject.transform.localRotation = transform.localRotation;
        }
    }
}
