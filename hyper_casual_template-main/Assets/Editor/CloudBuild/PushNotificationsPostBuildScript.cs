using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public static class PushNotificationsPostBuildScript {
    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {    
       // Only perform these steps for iOS builds
      if (buildTarget == BuildTarget.iOS) {
        ProcessPostBuildIOS(buildTarget, path);
      }
    }
#if NO_PUSH
    [System.Diagnostics.Conditional("DUMMYSYMBOL_NO_PUSH")]
#endif
    private static void ProcessPostBuildIOS(BuildTarget buildTarget, string path) {
 
      Debug.Log("[PushNotificationsPostBuildScript] ProcessPostBuild - iOS - Adding Push Notification capabilities.");
 
      // get XCode project path
      string pbxPath = PBXProject.GetPBXProjectPath(path);
 
      // Add linked frameworks
      PBXProject pbxProject = new PBXProject();
      pbxProject.ReadFromString(File.ReadAllText(pbxPath));
#if UNITY_2019_3_OR_NEWER
      string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();
#else
      string targetGUID = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
      pbxProject.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);
      File.WriteAllText(pbxPath, pbxProject.WriteToString());
 
      // Add required capabilities: Push Notifications
      var capabilities = new ProjectCapabilityManager(pbxPath, "app.entitlements", "Unity-iPhone");
      capabilities.AddPushNotifications(true);
      capabilities.WriteToFile();
    }
}
