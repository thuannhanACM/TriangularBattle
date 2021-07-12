using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#if !UNITY_CLOUD_BUILD
namespace UnityEngine.CloudBuild
{
    /// <summary>
    /// Dummy just to make coding easier.
    /// コーディングしやすくするためだけのダミー
    /// https://docs.unity3d.com/2019.4/Documentation/Manual/UnityCloudBuildManifestAsScriptableObject.html
    /// </summary>
    public class BuildManifestObject : ScriptableObject
    {
        // マニフェストの Value を取得しようとしています - Key が見つかり、T 型にキャスト可能ならば true、そうでない場合は false を返します。
        public bool TryGetValue<T>(string key, out T result)
        {
            result = default(T);
            return true;
        }
        // マニフェストの Value を取得しようとしています。指定したKey が見つからなければ例外を投げます。
        public T GetValue<T>(string key)
        {
            return default(T);
        }
        // 指定したKey に Value を設定します。
        public void SetValue(string key, object value)
        {

        }
        // 辞書から Value をコピーします。格納する前に、辞書の Value に対して ToString() が呼び出されます。
        public void SetValues(Dictionary<string, object> sourceDict)
        {

        }
        // すべての Key/Value を削除します。
        public void ClearValues()
        {

        }
        // 現在の BuildManifestObject を表す辞書を返します。
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>();
        }
        // 現在のBuildManifestObject を表す JSON 形式の文字列を返します。 
        public string ToJson()
        {
            return string.Empty;
        }
        // 現在のBuildManifestObject を表す INI 形式の文字列を返します。
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
#endif

/// <summary>
/// https://docs.unity3d.com/2019.4/Documentation/Manual/UnityCloudBuildPreAndPostExportMethods.html
/// </summary>
public static class CloudBuildExportMethod
{
    public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
    {
        CopyFirebaseConfigFiles();


        // Set bundle version
        var bundleVersion = System.Environment.GetEnvironmentVariable("HCG_BUILD_VERSION");
        Debug.Log($"Environment:HCG_BUILD_VERSION:{bundleVersion}");
        if (string.IsNullOrEmpty(bundleVersion) == false)
        {
            var ver = new System.Version(bundleVersion); // check only
            PlayerSettings.bundleVersion = bundleVersion;
        }
        // Set build number / bundle version code
        var buildNumberBase = System.Environment.GetEnvironmentVariable("HCG_BUILD_NUMBER_BASE");
        Debug.Log($"Environment:HCG_BUILD_NUMBER_BASE:{buildNumberBase}");
        int numBase = 1;
        if(string.IsNullOrEmpty(buildNumberBase)==false && System.Int32.TryParse(buildNumberBase, out numBase))
        {
            // プラットフォーム判別はせず、両方変えてしまう
            var buildNumber = manifest.GetValue<int>("buildNumber") + numBase;
            PlayerSettings.iOS.buildNumber = buildNumber.ToString();
            PlayerSettings.Android.bundleVersionCode = buildNumber;
        }
        
#if NO_UIDEBUG
        SRDebugger.Editor.SRDebugEditor.SetEnabled(false);
#endif
#if NO_FACEBOOK
        DisableFacebookSDK();
#endif

        // For Debug
        Debug.Log($"manifest:projectId:{manifest.GetValue<string>("projectId")}");//15668200785771/7d95d840-a414-4f3f-9264-87b1d83b2392
        Debug.Log($"manifest:bundleId:{manifest.GetValue<string>("bundleId")}");//cloud.VolcanoGames.LatteTime
        Debug.Log($"manifest:cloudBuildTargetName:{manifest.GetValue<string>("cloudBuildTargetName")}");//default-ios
        Debug.Log($"CloudProjectSettings:organizationId:{CloudProjectSettings.organizationId}");//(empty)
        Debug.Log($"CloudProjectSettings:organizationName:{CloudProjectSettings.organizationName}");//(empty)
        Debug.Log($"CloudProjectSettings:projectId:{CloudProjectSettings.projectId}");//(empty)
        Debug.Log($"CloudProjectSettings:projectName:{CloudProjectSettings.projectName}");//(empty)
        Debug.Log($"CloudProjectSettings:userId:{CloudProjectSettings.userId}");//anonymous
        Debug.Log($"CloudProjectSettings:userName:{CloudProjectSettings.userName}");//anonymous
        Debug.Log($"PlayerSettings:applicationIdentifier:{PlayerSettings.applicationIdentifier}");//cloud.VolcanoGames.LatteTime
        Debug.Log($"PlayerSettings:productName:{PlayerSettings.productName}");//Latte Time Dev
        Debug.Log($"PlayerSettings:bundleVersion:{PlayerSettings.bundleVersion}");//1.3
        Debug.Log($"PlayerSettings:iOS.buildNumber:{PlayerSettings.iOS.buildNumber}");//10
        Debug.Log($"PlayerSettings:Android.bundleVersionCode:{PlayerSettings.Android.bundleVersionCode}");//4
    }

    private static string _FirebaseConfigParentPath = "/";

    private static string FirebaseConfigParentPath
    {
        get
        {
            if (_FirebaseConfigParentPath == "/")
            {
                if (File.Exists($"{Application.dataPath}/{jsonFileName}"))
                {
                    _FirebaseConfigParentPath = "";
                }
                else
                {
                    _FirebaseConfigParentPath = "FirebaseConfig/";
                }
            }
            return _FirebaseConfigParentPath;
        }
    }

    private static readonly string jsonFileName = "google-services.json";
    private static readonly string plistFileName = "GoogleService-Info.plist";
    private static readonly string xmlName = "google-services.xml";
    public static void CopyFirebaseConfigFile(string source, string dest)
    {
#if false // Tools/Build/pre_build_adjust_projectid_prod.sh でコピーするので、ここではカット
        if(!CopyIfExists(source, dest))
        {
            var result= CopyIfExists($"FirebaseConfig/{source}", $"FirebaseConfig/{dest}");
            if (result)
            {
                FirebaseConfigParentPath = "FirebaseConfig/";
            }
        }
#endif
    }
    public static bool CopyIfExists(string source, string dest)
    {
        if (File.Exists($"{Application.dataPath}/{source}"))
        {
            AssetDatabase.DeleteAsset($"Assets/{dest}");
            AssetDatabase.CopyAsset($"Assets/{source}", $"Assets/{dest}");
            Debug.Log($"{source} => {dest}");
            return true;
        }
        return false;
    }

    public static string FindXmlFile(string assetsPath)
    {
        Debug.Log($"FindXmlFile:{assetsPath}, {xmlName}");
        var files = Directory.GetFiles($"{assetsPath}/Plugins", xmlName, SearchOption.AllDirectories);
        for (var i = 0; i < files.Length; i++)
        {
            Debug.Log($"FindXmlFile:{files[i]}");
        }
        return files.FirstOrDefault();
    }

    public static void ForceUpdateXml()
    {
#if false
        // 1回のbatchmodeの中で起こした変化をImportAsset()でどうにかすることはできない模様
        Debug.Log($"Force ImportAsset:Assets/{FirebaseConfigParentPath}");
        AssetDatabase.ImportAsset($"Assets/{FirebaseConfigParentPath}", ImportAssetOptions.ForceSynchronousImport);
#endif
        // 1回のbatchmodeの中で起こした変化はForceJsonUpdate()で反応してくれない模様
        Debug.Log("GenerateXmlFromGoogleServicesJson.ForceJsonUpdate");
        Firebase.Editor.GenerateXmlFromGoogleServicesJson.ForceJsonUpdate();
    }

#if UNITY_ANDROID
    public static bool CheckFirebaseXml()
    {
        var xmlPath = FindXmlFile(Application.dataPath);
        if (string.IsNullOrEmpty(xmlPath))
        {
            Debug.LogWarning($"{xmlName} not found");
            return false;
        }
        var xml = XDocument.Load(xmlPath);
        var gcm_defaultSenderId = xml.Descendants("string").Where(elm => elm.Attribute("name")?.Value == "gcm_defaultSenderId").Select(elm => elm?.Value).First();
        Debug.Log($"gcm_defaultSenderId:{gcm_defaultSenderId}");
        var json = File.ReadAllLines($"{Application.dataPath}/{FirebaseConfigParentPath}{jsonFileName}");
        var result = json.FirstOrDefault(line => line.Contains(gcm_defaultSenderId));
        Debug.Log($"CheckFirebaseXml:{result}");
        return !string.IsNullOrEmpty(result);
    }
#endif
    [MenuItem("Tools/Build/CopyFirebaseConfigFiles")]
    public static void CopyFirebaseConfigFiles()
    {
#if PRODUCTION
        Debug.Log($"symbol:PRODUCTION:{true}");
        // Copy Firebase files
        CopyFirebaseConfigFile("GoogleService-Info-prod.plist", plistFileName);
        CopyFirebaseConfigFile("google-services-prod.json", jsonFileName);
        // remove "Dev"
        Debug.Log($"PlayerSettings:productName:(before):{PlayerSettings.productName}");//Latte Time Dev
        PlayerSettings.productName = System.Text.RegularExpressions.Regex.Replace(PlayerSettings.productName, "(.+) Dev$", "$1");
#else
        Debug.Log($"symbol:PRODUCTION:{false}");
        // Copy Firebase files
        CopyFirebaseConfigFile( "GoogleService-Info-dev.plist", plistFileName);
        CopyFirebaseConfigFile( "google-services-dev.json", jsonFileName);
#endif
        
        #if UNITY_ANDROID
        // force xml update
        ForceUpdateXml();
        // check xml
        if (CheckFirebaseXml() == false)
        {
            Debug.LogWarning($"CheckFirebaseXml failure !!!!!!!!!" );
            // 結果はとりあえず無視　EditorApplication.Exit(1); // 抜けてしまう
        }
        #endif
    }

    [MenuItem("Tools/Build/DisableFacebookSDK")]
    public static void DisableFacebookSDK()
    {
        SetCompatibleFacebookSDK(false);
        RemoveFacebookFromAndroidManifest();
    }
    [MenuItem("Tools/Build/EnableFacebookSDK")]
    public static void EnableFacebookSDK()
    {
        SetCompatibleFacebookSDK(true);
    }

    private static void SetCompatibleFacebookSDK(bool enable)
    {
        PluginImporter importer = null;
        // 本体
        importer = AssetImporter.GetAtPath("Assets/FacebookSDK/Plugins/Facebook.Unity.dll") as PluginImporter;
        importer.SetCompatibleWithAnyPlatform(enable);
        // Editor
        importer = AssetImporter.GetAtPath("Assets/FacebookSDK/Plugins/Editor/Facebook.Unity.Editor.dll") as PluginImporter;
        importer.SetCompatibleWithEditor(enable);
        // Any
        string[] guids = AssetDatabase.FindAssets("", new[]
        {
            "Assets/FacebookSDK/Plugins/Settings"
            ,"Assets/FacebookSDK/Plugins/Gameroom"
        });
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"{enable}:{path}");
            importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer != null)
            {
                importer.SetCompatibleWithAnyPlatform(enable);
            }
        }
        // Android
        guids = AssetDatabase.FindAssets("", new[] {"Assets/FacebookSDK/Plugins/Android"});
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"{enable}:{path}");
            importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer != null)
            {
                importer.SetCompatibleWithPlatform(BuildTarget.Android, enable);
            }
        }
        // iOS
        guids = AssetDatabase.FindAssets("", new[] {"Assets/FacebookSDK/Plugins/iOS"});
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"{enable}:{path}");
            importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer != null)
            {
                importer.SetCompatibleWithPlatform(BuildTarget.iOS, enable);
            }
        }
        // WebGL
        guids = AssetDatabase.FindAssets("", new[] {"Assets/FacebookSDK/Plugins/Canvas"});
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"{enable}:{path}");
            importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer != null)
            {
                importer.SetCompatibleWithPlatform(BuildTarget.WebGL, enable);
            }
        }
        
    }

    [MenuItem("Tools/Build/RemoveFacebookFromAndroidManifest")]
    public static void RemoveFacebookFromAndroidManifest()
    {
        var xmlPath = Path.GetFullPath(Application.dataPath+"/Plugins/Android/AndroidManifest.xml");
        var xml = XDocument.Load(xmlPath);
        var facebookElements = xml
            .Descendants("activity")
            .Where(elm => elm.Attribute("{http://schemas.android.com/apk/res/android}name")?.Value.StartsWith("com.facebook") == true).ToList();
        facebookElements.AddRange(
            xml.Descendants("meta-data")
                .Where(elm => elm.Attribute("{http://schemas.android.com/apk/res/android}name")?.Value.StartsWith("com.facebook") == true)
            );
        facebookElements.AddRange(
            xml.Descendants("provider")
                .Where(elm => elm.Attribute("{http://schemas.android.com/apk/res/android}name")?.Value.StartsWith("com.facebook") == true)
        );
        for (var i = 0; i < facebookElements.Count; i++)
        {
            Debug.Log($"remove: {facebookElements[i]}");
            facebookElements[i].Remove();
        }
        xml.Save(xmlPath);
    }
    
    public static void PostExport(string exportPath)
    {
        Debug.Log($"exportPath:{exportPath}");
#if PRODUCTION
        Debug.Log($"symbol:PRODUCTION:{true}");
#else
        Debug.Log($"symbol:PRODUCTION:{false}");
#endif
#if UNITY_ANDROID
        // force xml update
        // PostExportで実行しても紛らわしいだけだったのでカット ForceUpdateXml();
        // check xml
        if (CheckFirebaseXml() == false)
        {
            Debug.LogException(new Exception($"CheckFirebaseXml failure") );
            EditorApplication.Exit(1); // 抜けてしまう
        }
#endif
        // For Debug
        Debug.Log($"CloudProjectSettings:organizationId:{CloudProjectSettings.organizationId}");
        Debug.Log($"CloudProjectSettings:organizationName:{CloudProjectSettings.organizationName}");
        Debug.Log($"CloudProjectSettings:projectId:{CloudProjectSettings.projectId}");
        Debug.Log($"CloudProjectSettings:projectName:{CloudProjectSettings.projectName}");
        Debug.Log($"CloudProjectSettings:userId:{CloudProjectSettings.userId}");
        Debug.Log($"CloudProjectSettings:userName:{CloudProjectSettings.userName}");
        Debug.Log($"PlayerSettings:applicationIdentifier:{PlayerSettings.applicationIdentifier}");//cloud.VolcanoGames.LatteTime
        Debug.Log($"PlayerSettings:productName:{PlayerSettings.productName}");//Latte Time Dev
        Debug.Log($"PlayerSettings:bundleVersion:{PlayerSettings.bundleVersion}");//1.3
        Debug.Log($"PlayerSettings:iOS.buildNumber:{PlayerSettings.iOS.buildNumber}");//
        Debug.Log($"PlayerSettings:Android.bundleVersionCode:{PlayerSettings.Android.bundleVersionCode}");//
    }
#if UNITY_EDITOR && false
    [MenuItem("Tools/PreExportTest")]
    public static void PreExportTest()
    {
        var manifest = new UnityEngine.CloudBuild.BuildManifestObject();
        manifest.SetValue("projectId", "15668200785771/7d95d840-a414-4f3f-9264-87b1d83b2392");
        manifest.SetValue("bundleId", "cloud.VolcanoGames.HyperCasualTemplate");
        manifest.SetValue("cloudBuildTargetName", "default-ios");
        PreExport(manifest);
    }
#endif
}

