using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using HyperCasualTemplate;
using UnityEngine.SceneManagement;

public partial class SROptions
{
    public const string ActionCategory = "Action";

    [Sort(0)]
    [Category(ActionCategory)]
    public void Restart()
    {
        ConfigData.CloseSRDebuggerWindow();
        
        var sceneName = ConfigData.FirebaseSDKInstance.StartScene;
        SceneManager.LoadScene(sceneName);
    }

}
