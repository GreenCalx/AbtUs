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
        RefreshLayers();
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
}
