using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;


public static class ManualBuildPostProcessBuild
{
    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        // Only perform these steps for iOS builds
        if (buildTarget == BuildTarget.iOS)
        {
            ProcessPostBuildIOS(buildTarget, path);
        }
    }
#if UNITY_CLOUD_BUILD
    [System.Diagnostics.Conditional("DUMMYSYMBOL_UNITY_CLOUD_BUILD")]
#endif
    private static void ProcessPostBuildIOS(BuildTarget buildTarget, string path)
    {

        Debug.Log("[ManualBuildPostProcessBuild] ProcessPostBuild - iOS - Adding CODE_SIGN_IDENTITY, and more.");

        // get XCode project path
        string pbxPath = PBXProject.GetPBXProjectPath(path);

        // Add linked frameworks
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromString(File.ReadAllText(pbxPath));
#if UNITY_2019_3_OR_NEWER
        string targetGUID = pbxProject.GetUnityMainTargetGuid();
#else
        string targetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
        pbxProject.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY", "Apple Distribution");
        pbxProject.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", "Apple Distribution");
        File.WriteAllText(pbxPath, pbxProject.WriteToString());
    }
}
