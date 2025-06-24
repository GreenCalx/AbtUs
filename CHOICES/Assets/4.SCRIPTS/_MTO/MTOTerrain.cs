using System;
using System.Linq;
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
    [Range(0f, 1f)]
    public float treeFullness;
    [Tooltip("X : [0,1] MTO Raw Axis, Y is treefullness value")]
    public AnimationCurve treeFullnessOverMTO;
    private bool refreshReq = false;

    public bool ForceTreeRefresh = false;
    private TreeInstance[] baseTreeInstances;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        baseTreeInstances = terrainData.treeInstances;

        currLayers = new List<TerrainLayer>(terrainData.terrainLayers);
        initLayers = currLayers;

        OverWorldControl.Instance.SubscribeMTOTerrain(this);
    }
    private void RefreshTreeFullness()
    {
        if (treeFullnessOverMTO == null)
        {
            treeFullness = 1f;
            return;
        }
        treeFullness = treeFullnessOverMTO.Evaluate(OverWorldControl.Instance.MineralToOrganic);
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

    public void RefreshTrees()
    {
        RefreshTreeFullness();

        TreeInstance[] trees = terrainData.treeInstances;
        TreePrototype[] availablePrototypes = terrainData.treePrototypes;

        float mineralMag = OverWorldControl.Instance.MineralMagnitude;
        float organicMag = OverWorldControl.Instance.OrganicMagnitude;

        string checkTag = mineralMag > 0f ? Constants.tag_Mineral : (organicMag > 0f ? Constants.tag_Organic : null);
        if (checkTag == null)
            return;

        // select prototypes matching checkTag
        List<int> selectedProtoIndexes = new List<int>();
        for (int i = 0; i < availablePrototypes.Length; i++)
        {
            TreePrototype tp = availablePrototypes[i];
            if (tp.prefab.tag == checkTag)
            {
                selectedProtoIndexes.Add(i);
            }
        }
        if (selectedProtoIndexes.Count == 0)
            return;

        // random proto selection
        // TODO  could be gpu if a looooooot of trees ?
        int n_hide = trees.Length - (int)(trees.Length * treeFullness);
        List<int[]> intervals = new List<int[]>();
        List<int> hide_indexes = new List<int>(0);

        int[] firstInterval = new int[trees.Length];
        for (int i = 0; i < firstInterval.Length; i++)
        {
            firstInterval[i] = i;
        }
        intervals.Add( firstInterval );

        int k, p, c = 0;
        while (hide_indexes.Count < n_hide)
        {
            List<int[]> nextIntervals = new List<int[]>();
            foreach (int[] interval in intervals)
            {
                if (interval.Length == 0)
                    continue;
                k = interval[0];
                p = interval[interval.Length - 1];
                c = (p - k) / 2;
                hide_indexes.Add(interval[c]);

                if (hide_indexes.Count >= n_hide)
                    break;

                int[] left_inter, right_inter;
                Utils.Split(interval, c, out left_inter, out right_inter);
                nextIntervals.Add(left_inter);
                nextIntervals.Add(right_inter);
            }
            intervals = new List<int[]>(nextIntervals);
        }

        int nextTreeCount = trees.Length - hide_indexes.Count;
        TreeInstance[] nextTrees = new TreeInstance[nextTreeCount];
        for (int i = 0; i < trees.Length; i++)
        {
            trees[i].prototypeIndex = selectedProtoIndexes[UnityEngine.Random.Range(0, selectedProtoIndexes.Count)];
        }
        trees = trees.Where(e => !hide_indexes.Contains(Array.IndexOf(trees,e))).ToArray();

        terrainData.SetTreeInstances(trees, false);
        terrain.Flush();

    }

    public void ResetLayers()
    {
        terrainData.terrainLayers = initLayers.ToArray();
    }

    void OnDestroy()
    {
        terrainData.treeInstances = baseTreeInstances;
        ResetLayers();
    }

    void Update()
    {
        if (refreshReq)
        {
            if (!CheckFrustrum(Managers.Instance.Camera.playerCam.cam))
            {
                RefreshLayers();
                refreshReq = false;
            }
        }

        if (ForceTreeRefresh)
        {
            RefreshTrees();
            ForceTreeRefresh = false;
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
