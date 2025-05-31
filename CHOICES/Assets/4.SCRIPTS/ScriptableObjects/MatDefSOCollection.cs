using UnityEngine;
using System.Collections.Generic;
using static Constants;

public class MatDefSOCollection : MonoBehaviour
{
    [Header("Debug")]
    public bool debug_dump = true;
    [Header("Internals")]
    public Dictionary<string, MatDefSO> mats;
    
    void Awake()
    {
        mats = new Dictionary<string, MatDefSO>();
    }
    public bool TryAddMat(Material iMat, string iMatName)
    {
        string matName = GetCleanedName(iMatName);
        if (mats.ContainsKey(matName))
            return false;
        
        MatDefSO matData = ScriptableObject.CreateInstance<MatDefSO>();
        matData.name = matName + "Def";
        matData.init(iMat);
        mats.Add(matName, matData);

        if (debug_dump)
        {
            Debug.Log("Mats in collections : ");
            foreach( string name in mats.Keys)
            {
                Debug.Log(name);
            }
            Debug.Log('\n');
        }
        return true;
    }

    public MatDefSO GetMatDefFromName(string iMatName)
    {
        string matName = GetCleanedName(iMatName);
        if (mats.ContainsKey(matName))
            return mats[matName];
        return null;
    }

    private string GetCleanedName(string iMatName)
    {
        string retval = iMatName;
        if (retval.Contains(suffix_Instance))
        {
            retval = retval.Replace(suffix_Instance, "");
            retval = retval.Replace(" ", "");
        }
        return retval;
    }
}