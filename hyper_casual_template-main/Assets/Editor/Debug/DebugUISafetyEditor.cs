using UnityEngine;
using UnityEditor;

namespace HyperCasualTemplate
{
    [CustomEditor(typeof(DebugUISafety))]
    public class DebugUISafetyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("If disableObjects is not set to anything, it will automatically set its own GameObject."
                + " もしもdisableObjectsに何も設定されていないなら、自動的に自分自身のGameObjectをセットする", MessageType.Info);
            DrawDefaultInspector();
        }
    }
}