using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModelTools))]
public class ModelToolsEditor : Editor
{
    void OnSceneGUI()
    {
        ModelTools script = (ModelTools)target; 

        if (Event.current.type == EventType.Repaint)
        {
            script.OnModification();
        }
    }
}