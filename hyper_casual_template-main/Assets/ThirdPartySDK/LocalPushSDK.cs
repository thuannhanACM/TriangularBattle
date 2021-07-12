using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasualTemplate
{
	/// <summary>
	/// 常駐する(DontDestroyOnLoadを設定する)GameObjectにアタッチすること
	/// </summary>
	public class LocalPushSDK : MonoBehaviour
	{
		
        [SerializeField] private string title = "Hyper Casual Template";
        [SerializeField] private string cannelId = "HyperCasualTemplate";
        
       void OnApplicationFocus(bool hasFocus)
        {
            //if (!hasFocus) return;
            LocalPushNotification.RemoveAllDeliveredNotifications();
            LocalPushNotification.SetApplicationBadge(0);
        }
		private void Start()
		{
		    AddNotificationCalendar();
			ConfigData.FirebaseSDKInstance.OnFetchRemoteConfigComplete += AddNotificationCalendar;
		}

		public string GetNotificationString() {
			LocalPushNotification.RegisterChannel(cannelId, $"{title} Channel", "Generic notifications");
			string[][] messages = {
				new string[] { "You need more spinning!", "Next opponent awaits. Draw the strongest spinner!", "Time to flip off your opponent!"},
				new string[] { "Tournament is being held! Can you survive?", "Wanna spin today?", "Wanna draw something today?"},
				new string[] { "Draw a strongest spinenr!", "Flip off your opponent!", "Win through the tournament and become the best!"},
				new string[] { "Next opponent is waiting.", "Chance to learn a new skill.", "Here comes a new opponent."}
			};
			var id = ConfigData.GetValue<int>(Config.PushNotification_MessageId);
			id = id < 0 ? 0 : (id > messages.Length-1 ? messages.Length-1 : id);
			var mssId = new System.Random().Next(0, messages[id].Length);
			#if false // ローカライズ関係はいったん保留
			if (id == 0) return notification.GetLocalizeData(ConfigData.SetSystemLanguage, mssId).Message;
			#endif
			return messages[id][mssId];
		}

		public void AddNotificationCalendar() {
			var mss = GetNotificationString();
			var hour = 19;
			var minute = 00;

			string[] timeArr = ConfigData.GetValue<string>(Config.PushNotification_Time).Split(':');
			System.Int32.TryParse(timeArr[0], out hour);
			if (timeArr.Length >= 2) System.Int32.TryParse(timeArr[1], out minute);
			hour = hour < 0 ? 0 : (hour > 23 ? 23 : hour);
			minute = minute < 0 ? 0 : (minute > 59 ? 59 : minute);
			LocalPushNotification.AddCalendarTrigger(title, mss, 1, hour, minute);
			Debug.Log("===========AddNotificationCalendar: " + mss + ", time: "+hour+":"+minute);
		}

		public void AddNotificationSchedule(int seconds) {
			var mss = GetNotificationString();
			LocalPushNotification.AddSchedule(title, mss, 1, seconds, cannelId);
			Debug.Log("===========AddNotificationSchedule: " + mss + ", seconds: "+seconds);
		}
	}
}
