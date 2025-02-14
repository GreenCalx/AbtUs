using UnityEngine;

[ExecuteInEditMode]
public class ModelTools : MonoBehaviour
{
    public bool clipToTerrain = false;
    public bool useModelForOffset = false;
    public bool alignModelToTerrain = false;
    public Vector2 terrainSize;

    public float terrainOffset = 0;
    public Terrain terrain;

    public bool addToIslandPool = false;
    public void OnModification()
    {
        if (clipToTerrain) { tapeToClosestTerrainHeigh();}
        if (addToIslandPool)
        {
            GetTerrain().transform.parent.GetComponent<ObjectPool>().Add(this.gameObject);
            addToIslandPool = false;
        }
    }


    #region TERRAIN DISPLACEMENT
    private void tapeToClosestTerrainHeigh()
    {
        if(useModelForOffset) {terrainOffset = this.GetComponentInChildren<MeshRenderer>().bounds.extents.y; }
        Vector3 RayOrigin = transform.position;
        RayOrigin.y += 100;
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("StaticTerrain");
        if (Physics.Raycast(RayOrigin, 300 * Vector3.down, out hit, 300, mask))
        {
            Terrain hitTerrain = hit.collider.gameObject.GetComponent<Terrain>();
            if (hitTerrain != null)
            {
                terrain = hitTerrain;
                terrainSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
                float new_y = hitTerrain.SampleHeight(transform.position);
                transform.position = new Vector3(transform.position.x, new_y + terrainOffset, transform.position.z);
                if (alignModelToTerrain)
                {
                    transform.up = hit.normal;
                }

            }
            else{ Debug.Log(hit.collider.gameObject.name + " mask = " + mask.value); }
        }
        else {
            Debug.DrawRay(RayOrigin, 300 * Vector3.down, Color.red); 
        }
    }

    public float getTerrainY()
    {
        if (terrain != null)
        {
            return( terrain.SampleHeight(transform.position) + terrainOffset);
        }
        else
        {
            findTerrain();
            return 0;
        }

    }

    public Terrain GetTerrain()
    {
        if(terrain == null) { findTerrain(); }
        if(terrain == null) { throw new System.Exception("Object " + this.name + " has no terrain but try to access it"); }
        return terrain;
    }

    private void findTerrain()
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
                terrain = hitTerrain;
                terrainSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
            }
            else { Debug.Log(hit.collider.gameObject.name + " mask = " + mask.value); }
        }
        else
        {
            Debug.DrawRay(RayOrigin, 300 * Vector3.down, Color.red);
        }
    }

    public Vector3 getNormal(Vector3 pos)
    {
        if(terrain == null) { return Vector3.up; }
        return (terrain.terrainData.GetInterpolatedNormal(pos.x / terrainSize.x, pos.z / terrainSize.y));
    }

    #endregion
}
