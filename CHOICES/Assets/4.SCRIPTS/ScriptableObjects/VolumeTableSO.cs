using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VolumeTableSO", menuName = "Scriptable Objects/VolumeTableSO")]
public class VolumeTableSO : ScriptableObject
{
    public List<GTLLookupVolumeUnit> volumeUnits;
    public List<GTLLookupVolumeUnit> extraVolumeUnits;
    
}
