using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;

public class MTOTerrain : MonoBehaviour
{
    public Terrain terrain;
    public List<TerrainLayer> currLayers;

    private List<TerrainLayer> initLayers;
    private TerrainData terrainData;
    private bool refreshReq = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (terrain==null)
            terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        OverWorldControl.Instance.SubscribeMTOTerrain(this);
        currLayers = new List<TerrainLayer>(terrainData.terrainLayers);
        initLayers = currLayers;
    }

    public void ChangeLayers(List<TerrainLayer> iNewPalette)
    {
        currLayers = iNewPalette;
        refreshReq = true;
        //RefreshLayers();
    }

    public void RefreshLayers()
    {
        terrainData.terrainLayers = currLayers.ToArray();
        // terrainData.SetBaseMapDirty();

        // terrain.Flush();
    }

    public void ResetLayers()
    {
        terrainData.terrainLayers = initLayers.ToArray();
    }

    void OnDestroy()
    {
        ResetLayers();
    }

    void Update()
    {
        if (refreshReq)
        {
            if (!CheckFrustrum(Managers.Instance.Camera.playerCam.cam))
            {
                RefreshLayers();
                refreshReq =false;
            }
        }
    }

    private bool CheckFrustrum(Camera cam)
    {
        Vector3 center = terrain.gameObject.transform.position + terrain.terrainData.size * 0.5f;
        Vector3 size = terrain.terrainData.size;

        Bounds bounds = new Bounds(center, size);

        Plane[] frustrum = GeometryUtility.CalculateFrustumPlanes(cam);

        return GeometryUtility.TestPlanesAABB(frustrum, bounds);
    }

}
