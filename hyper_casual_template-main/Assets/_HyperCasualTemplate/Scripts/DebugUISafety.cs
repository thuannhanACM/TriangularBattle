using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It is only visible in the Unity editor.
/// If NO_UIDEBUG is defined, it will also be hidden in the Unity editor.
/// Unityエディタのみ表示される。
/// NO_UIDEBUGが定義されていると、Unityエディタでも非表示になる。
/// </summary>
public class DebugUISafety : MonoBehaviour
{
    public GameObject[] disableObjects;

    void Awake()
    {
        // If disableObjects is not set to anything, it will automatically set its own GameObject.
        // もしもdisableObjectsに何も設定されていないなら、自動的に自分自身のGameObjectをセットする
        if (disableObjects == null || disableObjects.Length == 0)
        {
            disableObjects = new GameObject[1];
            disableObjects[0] = this.gameObject;
        }
#if NO_UIDEBUG || (!UNITY_EDITOR)
        if(disableObjects != null)
        {
            for(var i = 0; i < disableObjects.Length; i++)
            {
                disableObjects[i].SetActive(false);
            }
        }
#endif
    }

}
