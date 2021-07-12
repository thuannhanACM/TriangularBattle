using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GinGNotificationsExtension : MonoBehaviour
{
    public static GinGNotificationsExtension instance;
#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject GigGPushNotifications;
    private AndroidJavaObject mContext;
#endif

    private void Awake()
    {
        instance = this;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var GigGPushNotificationsPlugin = new AndroidJavaClass("com.ging.gingnotifications.GigG_PushNotifications");
        GigGPushNotifications = GigGPushNotificationsPlugin.CallStatic<AndroidJavaObject>("getInstance", mContext);
#endif
    }

    public void NotificationOptions(string Channel, int badgeCount, bool enableVibration, bool enableLights)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GigGPushNotifications.Call("NotificationOptions", Channel, badgeCount, enableVibration, enableLights);
#endif
    }

    public void AndroidNotificationTimeIntervalTrigger(string title, string[] messages, int minute, int hour)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StopNotificationTimeIntervalTrigger();
        GigGPushNotifications.Call("AndroidNotificationTimeIntervalTrigger", title, messages, minute, hour);
#endif
    }

    public void StopNotificationTimeIntervalTrigger()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GigGPushNotifications.Call("StopNotificationTimeIntervalTrigger");
#endif
    }

    public void CancelNotifications()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        GigGPushNotifications.Call("CancelNotifications");
#endif
    }
}
