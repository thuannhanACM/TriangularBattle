#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using System;

public static class LocalPushNotification
{
    /// <summary>
    /// Register the push notification channel used by Android.
    /// </summary>
    public static void RegisterChannel(string cannelId, string title, string description)
    {
#if UNITY_ANDROID
        // Subscribe Channel
        var c = new AndroidNotificationChannel()
        {
            Id = cannelId,
            Name = title,
            Importance = Importance.High,
            Description = description,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
        if(GinGNotificationsExtension.instance)
            GinGNotificationsExtension.instance.NotificationOptions(cannelId, 1, true, true);
#endif
    }

    /// <summary>
    /// Clear all notifications.
    /// </summary>
    public static void AllClear()
    {
#if UNITY_ANDROID
        // Remove all Android notifications.
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllNotifications();
#endif
#if UNITY_IOS
        // Remove all iOS notifications.
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        // Turn off the badge.
        iOSNotificationCenter.ApplicationBadge = 0;
#endif
    }

    /// <summary>
    /// Register for push notifications.
    /// </summary>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="badgeCount">Number of badges to display</param>
    /// <param name="elapsedTime">How many seconds will it be displayed?</param>
    /// <param name="cannelId">Channels used on Android</param>
    public static void AddSchedule(string title, string message, int badgeCount, int elapsedTime, string cannelId)
    {
#if UNITY_ANDROID
        SetAndroidNotification(title, message, badgeCount, elapsedTime, cannelId);
#endif
#if UNITY_IOS
        SetIOSNotification(title, message, badgeCount, elapsedTime);
#endif
    }

#if UNITY_IOS
    /// <summary>
    /// Register for notifications. (iOS)
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="badgeCount"></param>
    /// <param name="elapsedTime"></param>
    static private void SetIOSNotification(string title, string message, int badgeCount, int elapsedTime)
    {
        // Create a notification.
        iOSNotificationCenter.ScheduleNotification(new iOSNotification()
        {
            // * If you want to cancel push notifications individually, use this Identifier.
            Identifier = $"_notification_{badgeCount}",
            Title = title,
            Body = message,
            ShowInForeground = false,
            Badge = badgeCount,
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, 0, elapsedTime),
                Repeats = false
            }
        });
    }
#endif

#if UNITY_ANDROID
    /// <summary>
    /// Register for notifications. (Android)
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="badgeCount"></param>
    /// <param name="elapsedTime"></param>
    /// <param name="cannelId"></param>
    static private void SetAndroidNotification(string title, string message, int badgeCount, int elapsedTime, string cannelId)
    {
        // Create a notification.
        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            Number = badgeCount,
            // *Here, set the Android icon.
            SmallIcon = "icon_small",
            LargeIcon = "icon_large",
            FireTime = DateTime.Now.AddSeconds(elapsedTime)
        };

        // Send a notification.
        AndroidNotificationCenter.SendNotification(notification, cannelId);
        // *You can control push notifications individually by using the following codes.
        //var identifier = AndroidNotificationCenter.SendNotification(notification, cannelId);
        //UnityEngine.Debug.Log($"TownSoftPush: Push notification registration completed -> {DateTime.Now.AddSeconds(elapsedTime)}");

        //if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
        //{
        //    // Replace the currently scheduled notification with a new notification.
        //    UnityEngine.Debug.Log("Push notification already registered");
        //}
        //else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
        //{
        //    //Remove the notification from the status bar
        //    //AndroidNotificationCenter.CancelNotification(identifier);
        //    UnityEngine.Debug.Log("Push notification already notified");
        //}
        //else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
        //{
        //    //AndroidNotificationCenter.SendNotification(newNotification, "channel_id");
        //    UnityEngine.Debug.Log("Push notification is unknown");
        //}
    }
#endif

    /// <summary>
    /// Register for push notifications.
    /// </summary>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="badgeCount">Number of badges to display</param>
    public static void AddCalendarTrigger(string title, string message, int badgeCount, int hour, int minute)
    {
#if UNITY_IOS
        // Create a notification.
        iOSNotificationCenter.ScheduleNotification(new iOSNotification()
        {
            // * If you want to cancel push notifications individually, use this Identifier.
            Identifier = $"_notification_{badgeCount}",
            Title = title,
            Body = message,
            ShowInForeground = false,
            Badge = badgeCount,
            Trigger = new iOSNotificationCalendarTrigger()
            {
                // Year = 2018,
                // Month = 8,
                // Day = 30,
                Hour = hour,
                Minute = minute,
                // Second = 0
                Repeats = true
            }
        });
#endif
#if UNITY_ANDROID
        GinGNotificationsExtension.instance.AndroidNotificationTimeIntervalTrigger(title, new string[] { message }, minute, hour);
#endif
    }

    public static void AndroidAddCalendarTriggerArr(string title, string[] messages, int badgeCount)
    {
#if UNITY_ANDROID
        GinGNotificationsExtension.instance.AndroidNotificationTimeIntervalTrigger(title, messages, 0, 19);
#endif
    }

    public static void RemoveAllDeliveredNotifications()
    {
#if UNITY_IOS
    iOSNotificationCenter.RemoveAllDeliveredNotifications();
#endif
#if UNITY_ANDROID
        //Remove and set Badge = 0
        if(GinGNotificationsExtension.instance) GinGNotificationsExtension.instance.CancelNotifications();
#endif
    }

    public static void SetApplicationBadge(int badge)
    {
#if UNITY_IOS
    iOSNotificationCenter.ApplicationBadge = badge;
#endif
    }
}
