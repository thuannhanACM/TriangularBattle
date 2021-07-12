using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

/// <summary>
/// Firebase7.0.2以下とFacebookSDK9.x.xの組み合わせで発生する不具合を修正する(7.1.0になったら不要)
/// 参考：https://github.com/firebase/quickstart-unity/issues/862#issuecomment-771546659
/// </summary>
public static class AddFirebaseFilePostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuildAddFirebaseFile(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS) 
        {
            // Go get pbxproj file
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
  
            // PBXProject class represents a project build settings file,
            // here is how to read that in.
            PBXProject proj = new PBXProject ();
            proj.ReadFromFile (projPath);

#if UNITY_2019_3_OR_NEWER
            string targetGUID = proj.GetUnityMainTargetGuid();
#else
        string targetGUID = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
            // Copy plist from the project folder to the build folder
            proj.AddFileToBuild (targetGUID, proj.AddFile("GoogleService-Info.plist", "GoogleService-Info.plist"));

            // 2019.3以上限定?
            string googleInfoPlistGuid = proj.FindFileGuidByProjectPath( "GoogleService-Info.plist");
            proj.AddFileToBuild(targetGUID,googleInfoPlistGuid);
            
            // Write PBXProject object back to the file
            proj.WriteToFile (projPath);
        }
    }
}
