using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test_GinGNotificationsExtension : MonoBehaviour
{
    public InputField hour, minute;

    public void setNotifition()
    {
        string[] messages = { "Hey guys! How about a grill when you have a spare time!", "Aren't you hungry?It's time to start grilling.", "What's tonight's dinner?" };
        GinGNotificationsExtension.instance.NotificationOptions("master_grill_test", 99, true, true);
        GinGNotificationsExtension.instance.AndroidNotificationTimeIntervalTrigger("MASTER GRILL", messages, int.Parse(minute.text), int.Parse(hour.text));
    }

    public void cancelNotifications()
    {
        GinGNotificationsExtension.instance.CancelNotifications();
    }
}
