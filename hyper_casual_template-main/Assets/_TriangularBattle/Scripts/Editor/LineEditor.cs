using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(Line))]
public class LineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.BeginVertical();
        GUILayout.Space(20);
        if(GUILayout.Button("UpdateLine"))
        {
            ((Line)target).UpdateLine();
        }
        GUILayout.EndVertical();
    }
}
#endif