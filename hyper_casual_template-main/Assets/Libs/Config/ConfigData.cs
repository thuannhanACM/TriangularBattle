using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if !NO_FIREBASE
using Firebase;
using Firebase.RemoteConfig;
#endif
using UnityEngine;

namespace HyperCasualTemplate
{
    public class ConfigData : MonoBehaviour
    {
        public class Data
        {
            private object defValue;
            private object _remoteValue;
            private object _localPrefValue;
            private string _name;
            private string _dispName;
            private Config _config;
            private Action<object> _onValueChanged;

            public Data(Config config)
            {
                defValue = DefaultValueAttribute.GetDefaultValue(config);
                var dispName = DefaultValueAttribute.GetDisplayName(config);
                if (defValue is bool 
                    || defValue is int 
                    || defValue is long 
                    || defValue is float
                    || defValue is double
                    || defValue is string
                    )
                {
                    this._config = config;
                    this._name = config.ToString();
                    this._dispName = string.IsNullOrEmpty(dispName) ? this._name : dispName;
                }
                else
                {
                    throw new Exception("The type of the default value is an unsupported type.");
                }
            }

            public T GetValue<T>(ConfigMode mode)
            {
                if (mode == ConfigMode.Remote)
                {
                    return (T) System.Convert.ChangeType(_remoteValue, typeof(T));
                }
                if (mode == ConfigMode.Local)
                {
                    return (T) System.Convert.ChangeType(_localPrefValue, typeof(T));
                }
                return (T) System.Convert.ChangeType(defValue, typeof(T));
            }

#if !NO_FIREBASE
            public void SetRemoteValue(ConfigValue configValue)
            {
                if (defValue is bool)
                {
                    _remoteValue = configValue.BooleanValue;
                }
                else if (defValue is int)
                {
                    _remoteValue = (int)configValue.LongValue;
                }
                else if (defValue is long)
                {
                    _remoteValue = configValue.LongValue;
                }
                else if (defValue is float || defValue is double)
                {
                    _remoteValue = configValue.DoubleValue;
                }
                else if (defValue is string)
                {
                    _remoteValue = configValue.StringValue;
                }
                else
                {
                    throw new Exception("The type of the default value is an unsupported type.");
                }
            }
#endif

            public void SetLocalValueFromPlayerPrefs()
            {
                if (defValue is bool)
                {
                    _localPrefValue = PlayerPrefs.GetInt(_name, ((bool) defValue) ? 1 : 0) != 0;
                }
                else if (defValue is int || defValue is long)
                {
                    _localPrefValue = PlayerPrefs.GetInt(_name, (int) defValue);
                }
                else if (defValue is float || defValue is double)
                {
                    _localPrefValue = PlayerPrefs.GetFloat(_name, (float) defValue);
                }
                else if (defValue is string)
                {
                    _localPrefValue = PlayerPrefs.GetString(_name, (string) defValue);
                }
                else
                {
                    throw new Exception("The type of the default value is an unsupported type.");
                }
                Debug.Log($"PlayerPrefs: {_name} = {_localPrefValue}");
            }

            public void SetLocalValue(object setValue)
            {
                _localPrefValue = setValue;
                if (defValue is bool)
                {
                    PlayerPrefs.SetInt(_name, ((bool) setValue) ? 1 : 0);
                    //Debug.Log($"bool:{_name} = {PlayerPrefs.GetInt(prefsKey)}");
                }
                else if (defValue is int || defValue is long)
                {
                    PlayerPrefs.SetInt(_name, (int) setValue);
                    //Debug.Log($"int:{_name} = {PlayerPrefs.GetInt(prefsKey)}");
                }
                else if (defValue is float || defValue is double)
                {
                    PlayerPrefs.SetFloat(_name, (float) setValue);
                    //Debug.Log($"float:{_name} = {PlayerPrefs.GetFloat(prefsKey)}");
                }
                else if (defValue is string)
                {
                    PlayerPrefs.SetString(_name, (string) setValue);
                    //Debug.Log($"string:{_name} = {PlayerPrefs.GetString(prefsKey)}");
                }
                else
                {
                    throw new Exception("The type of the default value is an unsupported type.");
                }
                PlayerPrefs.Save();
            }
            public void AddSRDebuggerOption()
            {
#if (!NO_UIDEBUG) && (!DISABLE_SRDEBUGGER)
                SRDebugger.OptionDefinition option;
                if (defValue is bool)
                {
                    option = SRDebugger.OptionDefinition.Create(_dispName,
                        () => ConfigData.GetValue<bool>(_config), 
                        (newValue) =>
                        {
                            if (ConfigData.Mode == ConfigData.ConfigMode.Local)
                            {
                                ConfigData.SetLocalValue(_config, newValue);
                            }

                            this._onValueChanged?.Invoke(newValue);
                        },
                        category:SROptions.ConfigCategory,
                        sortPriority:DefaultValueAttribute.GetSortPriority(_config));
                }
                else if (defValue is int || defValue is long)
                {
                    option = SRDebugger.OptionDefinition.Create(_dispName,
                        () => ConfigData.GetValue<int>(_config), 
                        (newValue) =>
                        {
                            if (ConfigData.Mode == ConfigData.ConfigMode.Local)
                            {
                                ConfigData.SetLocalValue(_config, newValue);
                            }
                            this._onValueChanged?.Invoke(newValue);
                        },
                        category:SROptions.ConfigCategory,
                        sortPriority:DefaultValueAttribute.GetSortPriority(_config));
                }
                else if (defValue is float || defValue is double)
                {
                    option = SRDebugger.OptionDefinition.Create(_dispName,
                        () => ConfigData.GetValue<float>(_config), 
                        (newValue) =>
                        {
                            if (ConfigData.Mode == ConfigData.ConfigMode.Local)
                            {
                                ConfigData.SetLocalValue(_config, newValue);
                            }
                            this._onValueChanged?.Invoke(newValue);
                        },
                        category:SROptions.ConfigCategory,
                        sortPriority:DefaultValueAttribute.GetSortPriority(_config));
                }
                else if (defValue is string)
                {
                    option = SRDebugger.OptionDefinition.Create(_dispName,
                        () => ConfigData.GetValue<string>(_config), 
                        (newValue) =>
                        {
                            if (ConfigData.Mode == ConfigData.ConfigMode.Local)
                            {
                                ConfigData.SetLocalValue(_config, newValue);
                            }
                            this._onValueChanged?.Invoke(newValue);
                        },
                        category:SROptions.ConfigCategory,
                        sortPriority:DefaultValueAttribute.GetSortPriority(_config));
                }
                else
                {
                    throw new Exception("The type of the default value is an unsupported type.");
                }
                SRDebug.Instance.AddOption(option);
#endif
            }

            public void AddValueChangedAction()
            {
                var configActionType = typeof(ConfigAction);
                var methodInfo = configActionType.GetMethod(this._name,BindingFlags.Public | BindingFlags.Static);
                //Debug.Log($"{_name} => {methodInfo}");
                if (methodInfo != null)
                {
                    _onValueChanged = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), methodInfo);
                }
            }
        }

        private Dictionary<Config, Data> _store;

        public enum ConfigMode
        {
            Remote,
            Local,
        }

        private ConfigMode _Mode = ConfigMode.Local;

        public static ConfigMode Mode
        {
            get
            {
                if (Instance != null)
                {
                    return Instance._Mode;
                }
                return ConfigMode.Local;
            }
            set
            {
                if (Instance != null)
                {
                    Instance._Mode = value;
                }
            }
        }

        public static ConfigData Instance;
        public static FirebaseSDK FirebaseSDKInstance;
        public static LocalPushSDK LocalPushSDKInstance;
        private void Awake()
        {
            Instance = this;
            FirebaseSDKInstance = GetComponent<FirebaseSDK>();
            LocalPushSDKInstance = GetComponent<LocalPushSDK>();
            DontDestroyOnLoad(this.gameObject);
            
#if (!NO_UIDEBUG) && (!DISABLE_SRDEBUGGER)
            SRDebug.Init();
#endif
            _store = new Dictionary<Config, Data>();
            foreach (var conf in GetConfigs())
            {
                var dat = new Data(conf);
                dat.SetLocalValueFromPlayerPrefs();
                dat.AddSRDebuggerOption();
                dat.AddValueChangedAction();
                _store.Add(conf, dat);
            }
        }

        private void Start()
        {
#if (!NO_UIDEBUG) && (!DISABLE_SRDEBUGGER) && (UNITY_EDITOR)
            SRDebug.Instance.PinAllOptions(SROptions.ActionCategory);
#endif
        }

        private void Update()
        {
#if (!UNITY_EDITOR) && (!NO_UIDEBUG) && (!DISABLE_SRDEBUGGER)
            if (Input.GetMouseButtonDown(0) && Input.touchCount >= 3)
            {
                ToggleSRDebuggerWindow();
            }
#endif
        }

        public static Dictionary<string, object> GetDefaults()
        {
            var ret = new Dictionary<string, object>();
            foreach (Config conf in Enum.GetValues(typeof(Config)))
            {
                ret.Add(conf.ToString(), DefaultValueAttribute.GetDefaultValue(conf));
            }
            return ret;
        }

        public static string[] GetConfigKeys()
        {
            return Enum.GetNames(typeof(Config));
        }
        public static Config[] GetConfigs()
        {
            var confs =  Enum.GetValues(typeof(Config));
            var ret = new Config[confs.Length];
            Array.Copy(confs, ret, confs.Length);
            return ret;
        }

        public static T GetValue<T>(Config config, ConfigMode? mode = null)
        {
            if (Instance == null)
            {
                Debug.LogWarning("Config Data not initialized.");
                return DefaultValueAttribute.GetDefaultValue<T>(config);
            }

            if (Instance._store.ContainsKey(config) == false)
            {
                Debug.LogWarning($"Config Data<{config}> not initialized.");
                return DefaultValueAttribute.GetDefaultValue<T>(config);
            }

            var store = Instance._store[config];
            return store.GetValue<T>(mode.HasValue ? mode.Value : Instance._Mode);
        }

#if !NO_FIREBASE
        public static void SetRemoteValue(Config config, ConfigValue value)
        {
            if (Instance != null && Instance._store.ContainsKey(config))
            {
                Instance._store[config].SetRemoteValue(value);
            }
        }
#endif
        public static void SetLocalValue(Config config, object value)
        {
            if (Instance != null && Instance._store.ContainsKey(config))
            {
                Instance._store[config].SetLocalValue(value);
            }
        }

        public static void ReloadPlayerPrefs()
        {
            if (Instance != null)
            {
                foreach (var keyval in Instance._store)
                {
                    keyval.Value.SetLocalValueFromPlayerPrefs();
                }
            }
        }
        
#if false
        public static int _SetSystemLanguage
        {
            get { return PlayerPrefs.GetInt("SetSystemLanguage", 0); }
            set
            {
                PlayerPrefs.SetInt("SetSystemLanguage", value);
                PlayerPrefs.Save();
#if false
                if (LocalizeUI.instance != null)
                {
                    LocalizeUI.instance.UpdateCurrentData();
                }
#endif
            }
        }

        public static SystemLanguage SetSystemLanguage
        {
            get
            {
                switch (_SetSystemLanguage)
                {
                    case 0:
                        return Application.systemLanguage;
                    case 1:
                        return SystemLanguage.English;
                    case 2:
                        return SystemLanguage.Japanese;
                    case 3:
                        return SystemLanguage.ChineseSimplified;
                    case 4:
                        return SystemLanguage.ChineseTraditional;
                    default:
                        return Application.systemLanguage;
                }
            }
        }
#endif

#if DISABLE_SRDEBUGGER
        [System.Diagnostics.Conditional("DUMMYSYMBOL_DISABLE_SRDEBUGGER")]
#endif
        public static void OpenSRDebuggerWindow()
        {
#if (!NO_UIDEBUG) && (!DISABLE_SRDEBUGGER) 
            SRDebug.Instance.ShowDebugPanel(SRDebugger.DefaultTabs.Options);
#endif
        }
        
#if DISABLE_SRDEBUGGER
        [System.Diagnostics.Conditional("DUMMYSYMBOL_DISABLE_SRDEBUGGER")]
#endif
        public static void CloseSRDebuggerWindow()
        {
#if (!DISABLE_SRDEBUGGER) 
            if(SRDebug.Instance.IsDebugPanelVisible)
                SRDebug.Instance.HideDebugPanel();
#endif
        }
#if DISABLE_SRDEBUGGER
        [System.Diagnostics.Conditional("DUMMYSYMBOL_DISABLE_SRDEBUGGER")]
#endif
        public static void ToggleSRDebuggerWindow()
        {
#if (!DISABLE_SRDEBUGGER)
            if (SRDebug.Instance.IsDebugPanelVisible)
            {
                CloseSRDebuggerWindow();
            }
            else
            {
                OpenSRDebuggerWindow();
            }
#endif
        }
    }
}
