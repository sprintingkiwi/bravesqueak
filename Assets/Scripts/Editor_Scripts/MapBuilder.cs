using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldMap))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldMap myScript = (WorldMap)target;
        if (GUILayout.Button("Build Map"))
        {
            myScript.BuildMap();
        }
    }
}