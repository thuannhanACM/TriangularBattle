using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasualTemplate
{
	public class AppLovinMaxSDK : MonoBehaviour
	{
		// AppLovin Max SDK Key
		private const string MAX_SdkKey =
			"hEFBlX0b4klkMBxd39g9f3fXFLLZsUHQzwTiKo3Txe73WRhxvDwLaLK4E88fbztKBoIEXwwfhRC1aFL_UGZHRn";
#if PRODUCTION
#if UNITY_IOS
		// prod ios
		private const string MAX_BannerAdUnitId = "6c359142ec1be9c6"; // Sample
		private const string MAX_InterstitialAdUnitId = "c4c2d1bacdf8f612"; // Sample
		private const string MAX_RewardedAdUnitId = "e84848d2c6955d6c"; // Sample
#else
		// prod android
		private const string MAX_BannerAdUnitId = "6c359142ec1be9c6"; // Sample
		private const string MAX_InterstitialAdUnitId = "c4c2d1bacdf8f612"; // Sample
		private const string MAX_RewardedAdUnitId = "e84848d2c6955d6c"; // Sample
#endif
#else
#if UNITY_IOS
		// dev ios
		private const string MAX_BannerAdUnitId = "6c359142ec1be9c6"; // Sample
		private const string MAX_InterstitialAdUnitId = "c4c2d1bacdf8f612"; // Sample
		private const string MAX_RewardedAdUnitId = "e84848d2c6955d6c"; // Sample
#else
		// dev android
		private const string MAX_BannerAdUnitId = "6c359142ec1be9c6"; // Sample
		private const string MAX_InterstitialAdUnitId = "c4c2d1bacdf8f612"; // Sample
		private const string MAX_RewardedAdUnitId = "e84848d2c6955d6c"; // Sample
#endif
#endif
		private bool MAX_Initialized;
		private bool MAX_isBannerShowing = false;
		public static float LastTimeShowAds { get; private set; }
		private int interstitialRetryAttempt;
		private int rewardedRetryAttempt;
		private Action<bool> RewardedAdCallback;
		private Action<bool> RewardedAdListenCallback;
		
		public static bool RewardAdTestMode;
		[HideInInspector] public bool MAX_RewardedAdLoaded;

		// Output text to the debug log text field, as well as the console.
		public void DebugLog(string s)
		{
			Debug.Log("-------------------------------" + s);
		}

		// Start is called before the first frame update
		void Start()
		{
			InitializeAppLovin();

		}

		public string GetAdUnitId()
		{
			var text = "Bundle ID: " + Application.identifier;
			text += "\n MAX_BannerAdUnitId: " + MAX_BannerAdUnitId;
			text += "\n MAX_InterstitialAdUnitId: " + MAX_InterstitialAdUnitId;
			text += "\n MAX_RewardedAdUnitId: " + MAX_RewardedAdUnitId;
			text += "\n systemLanguage: " + Application.systemLanguage;
			text += "\n CurrentRegion: " + System.Globalization.RegionInfo.CurrentRegion.Name;
			return text;
		}
		
		// 3rd party SDK AppLovin ****************************************************************
		void InitializeAppLovin()
		{
			MAX_Initialized = true;
			MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
			{
				DebugLog("AppLovin SDK is initialized.");
				MAX_InitializeInterstitialAds();
				MAX_InitializeRewardedAds();
				MAX_InitializeBannerAds();
				MAX_ShowBanner();
			};
			MaxSdk.SetSdkKey(MAX_SdkKey);
			MaxSdk.InitializeSdk();
		}

		#region Interstitial Ad Methods

		void MAX_InitializeInterstitialAds()
		{
			// Attach callbacks
			MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
			MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
			MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
			MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

			// Load the first interstitial
			MAX_LoadInterstitial();
		}

		void MAX_LoadInterstitial()
		{
			DebugLog("MAX Interstitial Ad Loading...");
			MaxSdk.LoadInterstitial(MAX_InterstitialAdUnitId);
		}

		void MAX_ShowInterstitial()
		{
			if (!MAX_Initialized) return;
			if (MaxSdk.IsInterstitialReady(MAX_InterstitialAdUnitId))
			{
				Time.timeScale = 0;
				DebugLog("MAX Interstitial Ad Showing");
				MaxSdk.ShowInterstitial(MAX_InterstitialAdUnitId);
				ConfigData.FirebaseSDKInstance.AnalyticsLogEventWithoutParameter("dad_play_interstitial");
			}
			else
			{
				DebugLog("MAX Interstitial Ad not ready");
				MAX_LoadInterstitial();
			}
		}

		private void OnInterstitialLoadedEvent(string adUnitId)
		{
			// Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
			DebugLog("MAX Interstitial Ad loaded");
			// Reset retry attempt
			interstitialRetryAttempt = 0;
		}

		private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
		{
			DebugLog("MAX Interstitial failed to load with error code: " + errorCode);
			// Interstitial ad failed to load. We recommend retrying with exponentially higher delays.
			interstitialRetryAttempt++;
			double retryDelay = Math.Pow(2, interstitialRetryAttempt);
			Invoke("MAX_LoadInterstitial", (float) retryDelay);
		}

		private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
		{
			// Interstitial ad failed to display. We recommend loading the next ad
			DebugLog("MAX Interstitial failed to display with error code: " + errorCode);
			MAX_LoadInterstitial();
		}

		private void OnInterstitialDismissedEvent(string adUnitId)
		{
			// Interstitial ad is hidden. Pre-load the next ad
			DebugLog("MAX Interstitial dismissed");
			LastTimeShowAds = Time.time;
			Time.timeScale = 1;
			MAX_LoadInterstitial();

		}

		#endregion


		#region Rewarded Ad Methods

		private void MAX_InitializeRewardedAds()
		{
			// Attach callbacks
			MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
			MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
			MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
			MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
			MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
			MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
			MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

			// Load the first RewardedAd
			LoadRewardedAd();
		}

		private void LoadRewardedAd()
		{
			DebugLog("MAX Rewarded Ad Loading...");
			MaxSdk.LoadRewardedAd(MAX_RewardedAdUnitId);
		}

		private void MAX_ShowRewardedAd()
		{
			MAX_RewardedAdLoaded = false;
			if (RewardedAdListenCallback != null) RewardedAdListenCallback(MAX_RewardedAdLoaded);
			if (MaxSdk.IsRewardedAdReady(MAX_RewardedAdUnitId))
			{
				Time.timeScale = 0;
				DebugLog("MAX Rewarded Ad Showing");
				MaxSdk.ShowRewardedAd(MAX_RewardedAdUnitId);
			}
			else
			{
				DebugLog("MAX Rewarded Ad not ready");
				LoadRewardedAd();
				if (RewardedAdCallback != null) RewardedAdCallback(false);
				RewardedAdCallback = null;
			}
		}

		private void OnRewardedAdLoadedEvent(string adUnitId)
		{
			// Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(MAX_RewardedAdUnitId) will now return 'true'
			DebugLog("MAX Rewarded Ad loaded");

			// Reset retry attempt
			rewardedRetryAttempt = 0;
			MAX_RewardedAdLoaded = true;
			if (RewardedAdListenCallback != null) RewardedAdListenCallback(MAX_RewardedAdLoaded);
		}

		private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
		{
			DebugLog("MAX Rewarded Ad failed to load with error code: " + errorCode);
			// Rewarded ad failed to load. We recommend retrying with exponentially higher delays.
			rewardedRetryAttempt++;
			double retryDelay = Math.Pow(2, rewardedRetryAttempt);
			Invoke("LoadRewardedAd", (float) retryDelay);
		}

		private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
		{
			// Rewarded ad failed to display. We recommend loading the next ad
			DebugLog("MAX Rewarded Ad failed to display with error code: " + errorCode);
			LoadRewardedAd();
			if (RewardedAdCallback != null) RewardedAdCallback(false);
			RewardedAdCallback = null;
		}

		private void OnRewardedAdDisplayedEvent(string adUnitId)
		{
			DebugLog("MAX Rewarded Ad displayed");
		}

		private void OnRewardedAdClickedEvent(string adUnitId)
		{
			DebugLog("MAX Rewarded Ad clicked");
		}

		private void OnRewardedAdDismissedEvent(string adUnitId)
		{
			// Rewarded ad is hidden. Pre-load the next ad
			DebugLog("MAX Rewarded Ad dismissed");
			Time.timeScale = 1;
			LoadRewardedAd();
		}

		private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
		{
			// Rewarded ad was displayed and user should receive the reward
			DebugLog("MAX Rewarded Ad received reward");
			if (RewardedAdCallback != null) RewardedAdCallback(true);
			RewardedAdCallback = null;
		}

		#endregion


		#region Banner Ad Methods

		private void MAX_InitializeBannerAds()
		{
			if (!MAX_Initialized) return;
			DebugLog("MAX Initialized banner ad.");
			// Banners are automatically sized to 320x50 on phones and 728x90 on tablets
			// You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments
			MaxSdk.CreateBanner(MAX_BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
			// Set background or background color for banners to be fully functional
			MaxSdk.SetBannerBackgroundColor(MAX_BannerAdUnitId, Color.black);
		}

		private void MAX_ToggleBannerVisibility()
		{
			if (!MAX_Initialized) return;
			if (!MAX_isBannerShowing)
			{
				MaxSdk.ShowBanner(MAX_BannerAdUnitId);
				MAX_isBannerShowing = true;
			}
			else
			{
				MaxSdk.HideBanner(MAX_BannerAdUnitId);
				MAX_isBannerShowing = false;
			}
		}

		// MAX_ShowBanner
		private void MAX_ShowBanner()
		{
			if (!MAX_Initialized) return;
			DebugLog("MAX Showing banner ad.");
			MaxSdk.ShowBanner(MAX_BannerAdUnitId);
			MAX_isBannerShowing = true;
		}

		#endregion

		// End AppLovin ****************************************************************

		/// <summary>
		/// Global.Delay
		/// </summary>
		/// <param name="seconds"></param>
		/// <param name="Callback"></param>
		/// <returns></returns>
		public IEnumerator Delay(float seconds, System.Action Callback)
		{
			yield return new WaitForSeconds(seconds);
			Callback();
		}



		// バナー関係
		public void ShowBannerDelay(float seconds)
		{
			StartCoroutine(Delay(seconds, () => { MAX_ShowBanner(); }));
		}

		public void ToggleBannerVisibility()
		{
			DebugLog("ToggleBannerVisibility");
			MAX_ToggleBannerVisibility();
		}

		public void ShowInterstitialDelay(float seconds)
		{
			DebugLog("ShowInterstitialDelay");
			StartCoroutine(Delay(seconds, () =>
			{
				var RConfigAd = ConfigData.GetValue<int>(Config.AdInterval);
				var delay = (int) ((Time.time - LastTimeShowAds) * 1000);
				if (delay >= RConfigAd)
				{
					DebugLog("LastTimeShowAds: " + delay + " >= AdInterval: " + RConfigAd);
					MAX_ShowInterstitial();
				}
				else
				{
					DebugLog("LastTimeShowAds: " + delay + " < AdInterval: " + RConfigAd);
				}
			}));
		}

		public void ShowRewardedAd(Action<bool> callback)
		{
			DebugLog("ShowRewardedAd");
			if (RewardAdTestMode)
			{
				callback(true);
				return;
			}

			RewardedAdCallback = callback;
			MAX_ShowRewardedAd();
		}

		public void RewardedAdListen(Action<bool> callback)
		{
			callback(RewardAdTestMode ? true : MAX_RewardedAdLoaded);
			RewardedAdListenCallback = callback;
		}




	}
}

