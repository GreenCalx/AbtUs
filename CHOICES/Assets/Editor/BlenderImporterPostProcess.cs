using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

public class BlenderImporterPostProcess : AssetPostprocessor
{
    void OnPostprocessModel(GameObject importedGameObject)
    {
        // Vérifie si c'est un fichier FBX importé
        if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("FBX Detected");

            foreach(Transform child in importedGameObject.transform)
            {
                Debug.Log(child.name);
            }
            importedGameObject.AddComponent<Selectable>(); 
            importedGameObject.AddComponent<ModelTools>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(importedGameObject, "Assets/7.PREFABS/" + Path.GetFileNameWithoutExtension(assetPath) + ".prefab");
            if (prefab == null)
            {
                Debug.Log("Prefab not created");
                return;
            }
            Debug.Log("Prefab created and components added in :" + importedGameObject.name);
        }
    }
}