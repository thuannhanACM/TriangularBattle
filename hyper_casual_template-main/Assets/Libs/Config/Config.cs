using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  HyperCasualTemplate
{
    public enum Config 
    {
        // int sample (int value, sort priority)
        [DefaultValue(15000, 200)]AdInterval,
        // bool sample (bool value, sort priority, display name)
        [DefaultValue(false, 210, "GDPR")]is_gdpr_countries,
        
        [DefaultValue(1, 300)]PushNotification_MessageId,
        [DefaultValue("19:00", 310)]PushNotification_Time,
        
        // string sample (string value, sort priority)
        //[DefaultValue("b1,b2", 220)]ChestBonusItemCategories,
    }

    public class ConfigAction
    {
        public static void AdInterval(object newValue)
        {
            Debug.Log($"AdInterval changed => {newValue}");
        }
        public static void PushNotification_MessageId(object newValue)
        {
            Debug.Log($"PushNotification_MessageId changed => {newValue}");
            ConfigData.LocalPushSDKInstance?.AddNotificationCalendar();
        }
        public static void PushNotification_Time(object newValue)
        {
            Debug.Log($"PushNotification_Time changed => {newValue}");
            ConfigData.LocalPushSDKInstance?.AddNotificationCalendar();
        }
    }
    
}
