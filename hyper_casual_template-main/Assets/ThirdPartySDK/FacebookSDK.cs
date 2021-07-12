using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !NO_FACEBOOK
using Facebook.Unity;
#endif

namespace HyperCasualTemplate
{
	/// <summary>
	/// 常駐する(DontDestroyOnLoadを設定する)GameObjectにアタッチすること
	/// </summary>
	public class FacebookSDK : MonoBehaviour
	{
		private void Awake()
		{
#if !NO_FACEBOOK
			if (!FB.IsInitialized)
			{
				// Initialize the Facebook SDK
				FB.Init(InitCallback, OnHideUnity);
			}
			else
			{
				// Already initialized, signal an app activation App Event
				FB.ActivateApp();
			}
#endif			
		}
		// Facebook ****************************************************************
		private void InitCallback()
		{
#if !NO_FACEBOOK
			if (FB.IsInitialized)
			{
				// Signal an app activation App Event
				FB.ActivateApp();
				// Continue with Facebook SDK

				//var tutParams = new Dictionary<string, object>();
				//tutParams[AppEventParameterName.ContentID] = "tutorial_step_1";
				//tutParams[AppEventParameterName.Description] = "First step in the tutorial, clicking the first button!";
				//tutParams[AppEventParameterName.Success] = "1";

				//FB.LogAppEvent (
				//	AppEventName.CompletedTutorial,
				//	parameters: tutParams
				//);
				// ...

				FB.Mobile.SetAdvertiserTrackingEnabled(true);
			}
			else
			{
				Debug.Log("Failed to Initialize the Facebook SDK");
			}
#endif
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (!isGameShown)
			{
				// Pause the game - we will need to hide
				Time.timeScale = 0;
			}
			else
			{
				// Resume the game - we're getting focus again
				Time.timeScale = 1;
			}
		}
		// End Facebook ****************************************************************
	}

}
