using UnityEngine;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance = null;
    public static ObjectPoolManager Instance => instance;

    private Dictionary<string, ObjectPool> poolsDict = new Dictionary<string, ObjectPool>();
    public List<ObjectPool> pools;
    public string activeIsland;



    public void Awake()
    {
        instance = this;
        foreach(ObjectPool pool in pools)
        {
            poolsDict.Add(pool.name, pool);
        }
    }


    public void AddObject(GameObject iObj, string islandName)
    {
        if (poolsDict.ContainsKey(islandName))
        {
            poolsDict[islandName].Add(iObj);
            return;
        }
        throw new System.Exception("island name not recognized : " + islandName);
    }

    public void RemoveObject(GameObject iObj, string islandName)
    {
        if (poolsDict.ContainsKey(islandName))
        {
            poolsDict[islandName].Remove(iObj);
            return;
        }
        throw new System.Exception("island name not recognized : " + islandName);
    }

    public void Enable(string islandName, bool bol)
    {
        if (poolsDict.ContainsKey(islandName))
        {
            poolsDict[islandName].Enable(bol);
            return;
        }
        throw new System.Exception("island name not recognized : " + islandName);
    }

    public void ChangeActiveIsland(string newIslandName)
    {
        if (poolsDict.ContainsKey(newIslandName) && poolsDict.ContainsKey(activeIsland))
        {
            Enable(activeIsland, false);
            Enable(newIslandName, true);
            activeIsland = newIslandName;
            return;
        }
        Debug.LogWarning("islands :" + newIslandName + " " + activeIsland + " not found in the ObjectPoolManager (change active island)");
    }
}
