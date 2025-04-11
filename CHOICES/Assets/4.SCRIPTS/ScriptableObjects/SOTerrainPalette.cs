using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOTerrainPalette", menuName = "Scriptable Objects/SOTerrainPalette")]
    [Serializable]
public class SOTerrainPalette : ScriptableObject
{
    public List<TerrainLayer> PaletteLayers = new List<TerrainLayer>();
}
