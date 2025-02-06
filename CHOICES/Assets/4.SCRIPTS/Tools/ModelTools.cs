using UnityEngine;

[ExecuteInEditMode]
public class ModelTools : MonoBehaviour
{
    public bool clipToTerrain = true;
    public float terrainOffset = 0;


    public void OnModification()
    {
        if (clipToTerrain) { tapeToClosestTerrainHeigh();}
    }

    public void tapeToClosestTerrainHeigh()
    {

        Vector3 RayOrigin = transform.position;
        RayOrigin.y += 100;
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("StaticTerrain");
        if (Physics.Raycast(RayOrigin, 300 * Vector3.down, out hit, 300, mask))
        {
            Terrain hitTerrain = hit.collider.gameObject.GetComponent<Terrain>();
            if (hitTerrain != null)
            {
               
                float new_y = hitTerrain.SampleHeight(transform.position);
                transform.position = new Vector3(transform.position.x, new_y + terrainOffset, transform.position.z);
            }
            else{ Debug.Log(hit.collider.gameObject.name + " mask = " + mask.value); }
        }
        else {
            Debug.DrawRay(RayOrigin, 300 * Vector3.down, Color.red); 
        }
    }
}
