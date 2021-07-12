using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HyperCasualTemplate
{
    public class TouchEvent : MonoBehaviour
    {
        private bool isEditor = false;

        public event Action<Enums.TouchInfo> OnTouchInfo;

        private void Awake()
        {
#if UNITY_EDITOR
            isEditor = true;
#endif
        }

        private void Update()
        {
            if (isEditor)
            {
                OnTouchInfo?.Invoke(GetEditorInput());
            }
            else
            {
                OnTouchInfo?.Invoke(GetTouchInput());
            }
        }

        /// <summary>
        /// エディター上での入力を取得する
        /// </summary>
        /// <returns>入力イベント</returns>
        private Enums.TouchInfo GetEditorInput()
        {
            Enums.TouchInfo info = Enums.TouchInfo.None;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return info;
            }

            if (Input.GetMouseButtonDown(0))
            {
                info = Enums.TouchInfo.Began;
            }
            else if (Input.GetMouseButton(0))
            {
                info = Enums.TouchInfo.Moved;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                info = Enums.TouchInfo.Ended;
            }

            return info;
        }

        /// <summary>
        /// スマホ上での入力を取得する
        /// </summary>
        /// <returns>入力イベント</returns>
        private Enums.TouchInfo GetTouchInput()
        {
            Enums.TouchInfo info = Enums.TouchInfo.None;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return info;
            }

            if (Input.touchCount > 0)
            {
                info = (Enums.TouchInfo)((int)Input.GetTouch(0).phase);
            }

            return info;
        }

        /// <summary>
        /// タッチ位置を取得する
        /// </summary>
        /// <returns>タッチ位置</returns>
        public Vector2 GetTouchPos()
        {
            return (isEditor) ? Input.mousePosition : _GetTouchPos();
        }

        /// <summary>
        /// スマホ上でのタッチ位置を取得する
        /// </summary>
        /// <returns>タッチ位置</returns>
        private Vector3 _GetTouchPos()
        {
            Vector3 touchPos = Vector3.zero;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                touchPos.x = touch.position.x;
                touchPos.y = touch.position.y;
            }

            return touchPos;
        }
    }
}
