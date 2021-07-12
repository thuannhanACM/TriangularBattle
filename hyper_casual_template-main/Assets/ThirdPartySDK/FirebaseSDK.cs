using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !NO_FIREBASE
using Firebase.Extensions;
#endif
using System.Threading.Tasks;
using System;
using System.Threading;

namespace HyperCasualTemplate
{
	public class FirebaseSDK : MonoBehaviour
	{
		public event Action OnFetchRemoteConfigComplete;
		[Tooltip("Set Start Scene Name")]
		public string StartScene = "Start";
		
		// Firebase
		Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
		protected bool isFirebaseInitialized = false;
		private string FireBaseInstanceId;

		// Output text to the debug log text field, as well as the console.
		public void DebugLog(string s)
		{
			Debug.Log("-------------------------------" + s);
		}
		void Awake()
		{
			// remote configで"1.1"などの数値を正しく扱うための設定
			System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-us");
		}

		// Start is called before the first frame update
		void Start()
		{
#if !NO_FIREBASE
			Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
			{
				dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available)
				{
					InitializeFirebase();
				}
				else
				{
					Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
				}
			});
#endif

		}

		// Firebase ****************************************************************
		private void InitializeFirebase()
		{
			System.Collections.Generic.Dictionary<string, object> defaults = ConfigData.GetDefaults();

			foreach (var key in defaults.Keys)
			{
				//Debug.Log($"defValue: {key} = {defaults[key]}");
			}
#if !NO_FIREBASE			
			Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
			
			DebugLog("RemoteConfig configured and ready!");
			DebugLog("Enabling data collection.");
			Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
			// Set default session duration values.
			Firebase.Analytics.FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
			isFirebaseInitialized = true;
			FetchRemoteData();
#endif
			var userSeed = PlayerPrefs.GetInt("user_property_seed");
			if (userSeed <= 0)
			{
				userSeed = UnityEngine.Random.Range(1, 13);
				AnalyticsSetUserSeed(userSeed);
			}
		}


#if !NO_FIREBASE
		public bool GetBooleanValue(string key)
		{
			return (bool) Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
		}

		public float GetDoubleValue(string key)
		{
			return (float) Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
		}

		public int GetLongValue(string key)
		{
			return (int) Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
		}

		public string GetStringValue(string key)
		{
			return (string) Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
		}
		
		private void FetchComplete(Task fetchTask)
		{
			if (fetchTask.IsCanceled)
			{
				DebugLog("Fetch canceled.");
			}
			else if (fetchTask.IsFaulted)
			{
				DebugLog("Fetch encountered an error.");
			}
			else if (fetchTask.IsCompleted)
			{
				DebugLog("Fetch completed successfully!");
			}

			var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
			switch (info.LastFetchStatus)
			{
				case Firebase.RemoteConfig.LastFetchStatus.Success:
					Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().
						ContinueWithOnMainThread(task =>
							{
								DebugLog(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));
							});
					break;
				case Firebase.RemoteConfig.LastFetchStatus.Failure:
					switch (info.LastFetchFailureReason)
					{
						case Firebase.RemoteConfig.FetchFailureReason.Error:
							DebugLog("Fetch failed for unknown reason");
							break;
						case Firebase.RemoteConfig.FetchFailureReason.Throttled:
							DebugLog("Fetch throttled until " + info.ThrottledEndTime);
							break;
					}

					break;
				case Firebase.RemoteConfig.LastFetchStatus.Pending:
					DebugLog("Latest Fetch call still pending.");
					break;
			}

			foreach (var conf in ConfigData.GetConfigs())
			{
				ConfigData.SetRemoteValue(conf, Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(conf.ToString()));
			}

			ConfigData.Mode = ConfigData.ConfigMode.Remote; // fetch成功したらremoteモードに
			DebugLog(GetRemoteData());
			
			OnFetchRemoteConfigComplete?.Invoke();

			UnityEngine.SceneManagement.SceneManager.LoadScene(StartScene);
		}

		// Start a fetch request.
		public Task FetchDataAsync()
		{
			DebugLog("Fetching data...");
			System.Threading.Tasks.Task
				fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
			return fetchTask.ContinueWithOnMainThread(FetchComplete);
		}

		// Start a fetch request.
		public void FetchRemoteData()
		{
			FetchDataAsync();
		}
#endif

		public string GetRemoteData()
		{
			var text = "";

			var confs = ConfigData.GetConfigs();
			foreach (var conf in confs)
			{
				text += $"\n {conf.ToString()}: {ConfigData.GetValue<string>(conf, ConfigData.ConfigMode.Remote)}";
			}

			text += "\n";
			
			
			//DebugLog(text);
			return text;
		}
		
		public void AnalyticsSetUserSeed(int seed)
		{
			if (!isFirebaseInitialized || seed <= 0) return;
			DebugLog("SetUserSeed: " + seed);
			Firebase.Analytics.FirebaseAnalytics.SetUserProperty("seed", seed.ToString());
			PlayerPrefs.SetInt("user_property_seed", seed);
		}

		public void AnalyticsLevelStart(int level)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - level_start: level_number = " + level);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"level_start",
				new Firebase.Analytics.Parameter[]
				{
					new Firebase.Analytics.Parameter("level_name", level.ToString()),
					new Firebase.Analytics.Parameter("level_number", level),
				}
			);
		}

		public void AnalyticsLevelEnd(int level, int success, int score)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - level_end: level_number = " + level + ", success = " + success + ", score = " +
			         score);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"level_end",
				new Firebase.Analytics.Parameter[]
				{
					new Firebase.Analytics.Parameter("level_name", level.ToString()),
					new Firebase.Analytics.Parameter("level_number", level),
					new Firebase.Analytics.Parameter("success", success.ToString()),
					new Firebase.Analytics.Parameter("score", score),
				}
			);
		}

		public void AnalyticsClearStage(int stage)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging a Clear Stage event: stage_number = " + stage);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"clear_stage",
				new Firebase.Analytics.Parameter[]
				{
					new Firebase.Analytics.Parameter("stage_number_string", stage.ToString()),
					new Firebase.Analytics.Parameter("stage_number", stage),
				}
			);
		}

		public void AnalyticsSpendVirtualCurrency(string item_name, string item_category, double value)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - spend_virtual_currency: item_name = " + item_name + ", category = " +
			         item_category +
			         ",value = " + value);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"spend_virtual_currency",
				new Firebase.Analytics.Parameter[]
				{
					new Firebase.Analytics.Parameter("virtual_currency_name", "coin"),
					new Firebase.Analytics.Parameter("value", value),
					new Firebase.Analytics.Parameter("item_name", item_name),
					new Firebase.Analytics.Parameter("item_category", item_category),
				}
			);
		}

		public void AnalyticsEarnVirtualCurrency(string earn_type, double value)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - earn_virtual_currency: earn_type = " + earn_type + ",value = " + value);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"earn_virtual_currency",
				new Firebase.Analytics.Parameter[]
				{
					new Firebase.Analytics.Parameter("virtual_currency_name", "coin"),
					new Firebase.Analytics.Parameter("value", value),
					new Firebase.Analytics.Parameter("earn_type", earn_type),
				}
			);
		}

		public void AnalyticsLogEventDadPlayReward(string ad_type)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - dad_play_reward: ad_type = " + ad_type);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"dad_play_reward",
				new Firebase.Analytics.Parameter("ad_type", ad_type)
			);
		}

		public void AnalyticsLogEventWithoutParameter(string event_name)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event: " + event_name);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(event_name);
		}

		public void AnalyticsLogEventPostStep(int value)
		{
			if (!isFirebaseInitialized) return;
			DebugLog("Logging event - post_step: now_step_string = " + value);
			Firebase.Analytics.FirebaseAnalytics.LogEvent(
				"post_step",
				new Firebase.Analytics.Parameter("now_step_string", value.ToString())
			);
		}
		// End Firebase ****************************************************************


	}
}
