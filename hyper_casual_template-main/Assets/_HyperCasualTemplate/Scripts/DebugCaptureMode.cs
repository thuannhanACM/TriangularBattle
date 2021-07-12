using System;
using UnityEngine;

namespace HyperCasualTemplate
{
    public class DebugCaptureMode : MonoBehaviour
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
        }
        private static event Action<bool> CaptureModeSwitchAction;

        public static void SetCaptureMode(bool value)
        {
            CaptureModeSwitchAction?.Invoke(value);
        }
        private void OnEnable()
        {
            CaptureModeSwitchAction += SwitchProps;
        }

        private void OnDisable()
        {
            CaptureModeSwitchAction -= SwitchProps;
        }

        private void Start()
        {
            SwitchProps(PlayerPrefsEx.GetBool("CaptureMode", false));
        }

        private void SwitchProps(bool value)
        {
            for (var i = 0; i < disableObjects.Length; i++)
            {
                if (disableObjects[i] != null)
                {
                    var childrens = disableObjects[i].GetComponentsInChildren<MeshRenderer>();
                    foreach (var children in childrens)
                    {
                        children.enabled = !value;
                    }
                }
            }
        }

    }
}