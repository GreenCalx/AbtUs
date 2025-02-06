using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System.Collections.Generic;

public class BlenderImporterPostProcess : AssetPostprocessor
{

    void OnPostprocessModel(GameObject importedGameObject)
    {

        if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("FBX Detected");

            importedGameObject.AddComponent<ModelTools>();

            foreach(Transform child in importedGameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.AddComponent<Selectable>();
            }
            string prefabPath = "Assets/7.PREFABS/" + Path.GetFileNameWithoutExtension(assetPath) + ".prefab";

            return;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(importedGameObject, prefabPath);
            GameObject loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            Dictionary<string, MeshFilter> meshFiltersDict = new Dictionary<string, MeshFilter>();

            foreach (MeshFilter meshFilter in importedGameObject.GetComponentsInChildren<MeshFilter>())
            {
                meshFiltersDict[meshFilter.gameObject.name] = meshFilter;
            }
            Debug.Log(meshFiltersDict);
            foreach (MeshFilter meshFilter in loadedPrefab.GetComponentsInChildren<MeshFilter>())
            {
                if (meshFiltersDict.TryGetValue(meshFilter.gameObject.name, out MeshFilter sourceMeshFilter) &&
                    sourceMeshFilter.sharedMesh != null)
                {
                    meshFilter.sharedMesh = sourceMeshFilter.sharedMesh;
                }
                else
                {
                    Debug.LogWarning("Shared mesh not found" + meshFilter.gameObject.name);
                }
            }
            PrefabUtility.SavePrefabAsset(loadedPrefab);
            Debug.Log("Prefab created and components added in :" + importedGameObject.name);
        }
    }
}