using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using HyperCasualTemplate;

public partial class SROptions
{
    public const string ConfigCategory = "Configrations";

    private void refresh()
    {
        Debug.Log("Refresh Option Panel");
        // 再読み込みの手段がpublicに開放されていないようなので、強制的に閉じてしまう
        // The means to reload doesn't seem to be open to the public, so I'm forced to close it.
        ConfigData.CloseSRDebuggerWindow();
    }
    
    [Sort(0)]
    [Category(ConfigCategory)]
    public ConfigData.ConfigMode configMode
    {
        get { return ConfigData.Mode; }
        set
        {
            ConfigData.Mode = value;
            if (ConfigData.Mode == ConfigData.ConfigMode.Local)
            {
                ConfigData.ReloadPlayerPrefs();
            }
            foreach (var key in ConfigData.GetConfigKeys())
            {
                OnPropertyChanged(key);
            }
            refresh();
        }
    }

    [Sort(5)]
    [Category(ConfigCategory)]
    public bool CaptureMode
    {
        get
        {
            return PlayerPrefsEx.GetBool("CaptureMode", false);
        }
        set
        {
            PlayerPrefsEx.SetBool("CaptureMode", value);
            PlayerPrefs.Save();
            DebugCaptureMode.SetCaptureMode(value);
        }
    }

    [Sort(10)]
    [Category(ConfigCategory)]
    public void FetchRemoteData()
    {
#if !NO_FIREBASE
        ConfigData.FirebaseSDKInstance.FetchRemoteData();
#endif
        refresh();
    }
    [Sort(11)]
    [Category(ConfigCategory)]
    public void CopyRemoteData()
    {
        var text = ConfigData.FirebaseSDKInstance.GetRemoteData();
		var delay = (long) ((Time.time - AppLovinMaxSDK.LastTimeShowAds) * 1000);
        var RConfigAd = ConfigData.GetValue<int>(Config.AdInterval);
		if (delay >= RConfigAd)
		{
			text += " LastTimeShowAds: " + delay + " > AdInterval: " + RConfigAd;
		}
		else
		{
			text += " LastTimeShowAds: " + delay + " < AdInterval: " + RConfigAd;
		}
        text += "\n";
        GUIUtility.systemCopyBuffer = text;
    }
    
    [Sort(13)]
    [Category(ConfigCategory)]
    [NumberRange(0, 13)]
    public int UserPropertySeed
    {
        get
        {
            return PlayerPrefs.GetInt("user_property_seed");
        }
        set
        {
            ConfigData.FirebaseSDKInstance.AnalyticsSetUserSeed(value);
            PlayerPrefs.Save();
        }
    }

    [Sort(14)]
    [Category(ConfigCategory)]
    public void ShowMediationDebugger()
    {
#if (!NO_UIDEBUG)
        MaxSdk.ShowMediationDebugger();
#endif
    }

    [Sort(15)]
    [Category(ConfigCategory)]
    public void LocalPushTest()
    {
        ConfigData.LocalPushSDKInstance.AddNotificationSchedule(10);
    }
    
    


}

