using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class InfoPlistPostProcessBuild
{
    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        // Only perform these step
        if (buildTarget == BuildTarget.iOS)
        {
            UpdateInfoPlist(path);
        }
    }

    private static void UpdateInfoPlist(string path)
    {
        //Get Plist
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        //Get Root
        PlistElementDict rootDict = plist.root;

        PlistElementDict NSAppTransportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
        NSAppTransportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);
        
#if NO_ARM64
        var capabilities = rootDict["UIRequiredDeviceCapabilities"].AsArray();
        capabilities.values.RemoveAll(item => item.AsString() == "arm64");
#endif

        //WriteFile
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
