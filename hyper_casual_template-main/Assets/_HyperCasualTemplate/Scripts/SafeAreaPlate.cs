using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SafeAreaPlate : MonoBehaviour
{
    private bool initialized;
    private Rect postSafeArea;
    private RectTransform recttrans;

    private void Start()
    {
        Adjust();
    }

    void Update()
    {
        if (postSafeArea.Equals(Screen.safeArea))
        {
            return;
        }
        
        Adjust();
    }

    void Init()
    {
        if (initialized) return;
        recttrans = GetComponent<RectTransform>();
        initialized = true;
    }

    void Adjust()
    {
        if (!initialized)
        {
            Init();
        }
        var area = Screen.safeArea;
        postSafeArea = Screen.safeArea;
        Debug.Log($"SafeArea:{area}");

        int width, height;
        if (isDeviceMode())
        {
            width = Screen.currentResolution.width;
            height = Screen.currentResolution.height;
        }
        else
        {
            width = Screen.width;
            height = Screen.height;
        }

        {
            //var resolition = Screen.currentResolution;
            //Debug.Log($"Resolution:{resolition.width}({Screen.width}),{resolition.height}({Screen.height})");
        }

        var cutouts = Screen.cutouts;
        for (var i = 0; i < cutouts.Length; i++)
        {
            Debug.Log($"Cutout[{i}]:{cutouts[i]}");
        }

        recttrans.sizeDelta = Vector2.zero;
        var anchorMin = area.position;
        var anchorMax = area.position + area.size;
        anchorMin.x /= width;
        anchorMin.y /= height;
        anchorMax.x /= width;
        anchorMax.y /= height;
        recttrans.anchorMin = anchorMin;
        recttrans.anchorMax = anchorMax;
    }

    /// <summary>
    /// Device(Simulator View) Checker
    /// </summary>
    /// <returns></returns>
    bool isDeviceMode()
    {
#if UNITY_EDITOR
        // https://forum.unity.com/threads/new-device-simulator-preview.751067/page-5#post-6694636
        //Debug.Log($"SystemInfo.deviceType:{SystemInfo.deviceType}");
        return (SystemInfo.deviceType == DeviceType.Handheld);
#else
        return true;
#endif
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(SafeAreaPlate))]
    public class SafeAreaPlateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var obj = target as SafeAreaPlate;
            if (GUILayout.Button("Adjust"))
            {
                obj.Adjust();
            }
        }
    }
#endif
    
}
