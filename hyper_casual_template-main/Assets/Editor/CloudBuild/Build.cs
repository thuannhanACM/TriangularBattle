using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

//#define UNITY_ANDROID

/// <summary>
/// 参考：JenkinsでUnityビルドをしよう
/// https://qiita.com/Yosh/items/44ded4e2ca8663401d82
/// 他、多数
/// </summary>
public class Build
{
    /// <summary>
    /// ビルド時に実行される関数
    /// </summary>
    public static void BuildProcess()
    {
        Debug.Log($"BuildProcess()");
        foreach (var arg in System.Environment.GetCommandLineArgs())
        {
            Debug.Log($"arg:{arg}");
        }
        // 引数から取得したパラメータをPlayerSettingsに設定する
        BuildTarget buildTarget = BuildTarget.iOS;
        BuildTargetGroup buildTargetGroup = BuildTargetGroup.iOS;
        var bt = CommandLineArgs.GetValue("buildTarget");
        Debug.Log($"buildTarget:{bt}");
        if (String.Compare(bt, "android", StringComparison.InvariantCulture) == 0)
        {
            buildTarget = BuildTarget.Android;
            buildTargetGroup = BuildTargetGroup.Android;
        }

        string productName = CommandLineArgs.GetValue("productName");
        Debug.Log($"productName:{productName}");
        PlayerSettings.bundleVersion = CommandLineArgs.GetValue("bundleVersion");
        Debug.Log($"bundleVersion:{PlayerSettings.bundleVersion}");
        var bundleIdentifier = CommandLineArgs.GetValue("bundleIdentifier");
        Debug.Log($"bundleIdentifier:{bundleIdentifier}");
        bool isDevelopmentBuild = CommandLineArgs.GetValue("developmentBuild")?.Equals("true") == true;
        Debug.Log($"isDevelopmentBuild:{isDevelopmentBuild}");

        // プラットフォームの切り替え
        Debug.Log($"SwitchActiveBuildTarget({buildTargetGroup},{buildTarget})");
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.development = isDevelopmentBuild;

        // ビルドオプションの設定
        BuildOptions option = BuildOptions.SymlinkLibraries;
        if (isDevelopmentBuild)
        {
            option |= BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
        }

#if UNITY_IOS
        PlayerSettings.iOS.applicationDisplayName = productName;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleIdentifier);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

        string buildNumber = CommandLineArgs.GetValue("buildNumber");
        Debug.Log($"buildNumber:{buildNumber}");
        if (!string.IsNullOrEmpty(buildNumber))
        {
            PlayerSettings.iOS.buildNumber = buildNumber;
        }
#endif
#if UNITY_ANDROID
        // AAB(Android App Bundle)ビルドを有効にする
        EditorUserBuildSettings.buildAppBundle = true;

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleIdentifier);

        // 引数から取得したパラメータをPlayerSettingsに設定する
#if false
        PlayerSettings.Android.keystoreName = CommandLineArgs.GetValue("keyStorePath");
        Debug.Log($"keystoreName:{PlayerSettings.Android.keystoreName}");
        PlayerSettings.Android.keystorePass = CommandLineArgs.GetValue("keyStorePass");
        Debug.Log($"keystorePass:{PlayerSettings.Android.keystorePass}");
        PlayerSettings.Android.keyaliasName = CommandLineArgs.GetValue("keyAliasName");
        Debug.Log($"keyaliasName:{PlayerSettings.Android.keyaliasName}");
        PlayerSettings.Android.keyaliasPass = CommandLineArgs.GetValue("keyAliasPass");
        Debug.Log($"keyaliasPass:{PlayerSettings.Android.keyaliasPass}");
#endif
        PlayerSettings.Android.bundleVersionCode = int.Parse(CommandLineArgs.GetValue("bundleVersionCode"));
        Debug.Log($"bundleVersionCode:{PlayerSettings.Android.bundleVersionCode}");

        // Armv7など、対象にしたいアーキテクチャを指定する
        // ARMv7 と ARM64
        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
#endif

        // ～～～～中略～～～～
        // その他必要な設定を書き換えていく 

        // ビルドの実行
        var scenes = GetAllScenePaths();
        var outputPath = CommandLineArgs.GetValue("outputPath");
        Debug.Log($"outputPath:{outputPath}");
        Debug.Log("BuildPlayer()");
        var report = BuildPipeline.BuildPlayer(scenes, outputPath, buildTarget, option);
        ResultAction(report);
    }
    static string[] GetAllScenePaths() {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for( int i = 0; i < EditorBuildSettings.scenes.Length ; i++ ) {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    private static void ResultAction(BuildReport report)
    {
        var result = report.summary.result;

        // ビルド時に発生したLogType毎のメッセージを取得
        var messages = report.steps.SelectMany(x => x.messages)
            .ToLookup(x => x.type, x => x.content);

        Debug.Log("BuildResult : " + result);

        switch (result)
        {
            case BuildResult.Succeeded:
                EditorApplication.Exit(0);
                break;
            case BuildResult.Failed:
                Debug.Log(string.Join("\n\t", messages[LogType.Error].ToArray()));
                EditorApplication.Exit(1);
                break;
            case BuildResult.Cancelled:
            case BuildResult.Unknown:
                EditorApplication.Exit(1);
                break;
        }
    }
}

/// <summary>
/// パラメータ取得用関数
/// </summary>
public static class CommandLineArgs
{
    public static string GetValue(string parameterName)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("-" + parameterName + "="))
            {
                var argValue = arg.Substring(parameterName.Length + 1 /* - */ + 1 /* = */);
                return argValue;
            }

            if (arg.StartsWith("-" + parameterName) && (i+1)<args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
