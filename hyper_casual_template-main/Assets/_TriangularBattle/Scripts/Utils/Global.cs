using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;

public class Global : MonoBehaviour
{
    public static bool isSoundOn, isVibrationOn;
    public static int curLevel;

    public static void VibrationHaptic(int TypeNum)
	{
		if (!Data.isVibrationOn) return; 
		switch(TypeNum) 
		{
			case 1:
				MMVibrationManager.Haptic(HapticTypes.Success);
				break;
			case 2:
				MMVibrationManager.Haptic(HapticTypes.Warning);
				break;
			case 3:
				MMVibrationManager.Haptic(HapticTypes.Failure);
				break;
			case 4:
				MMVibrationManager.Haptic(HapticTypes.LightImpact);
				break;
			case 5:
				MMVibrationManager.Haptic(HapticTypes.MediumImpact);
				break;
			case 6:
				MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
				break;
			/*case 7:
				MMVibrationManager.Haptic(HapticTypes.RigidImpact);
				break;
			case 8:
				MMVibrationManager.Haptic(HapticTypes.SoftImpact);
				break;*/
			default:
				MMVibrationManager.Haptic(HapticTypes.Selection);
				break;
		}
	}

    public static void LoadScene(string scene)
    {
		if (scene != null)
        SceneManager.LoadScene(scene);
		else
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static string LoadResourceTextfile(string path)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        return targetFile.text;
    }

    public static IEnumerator Delay(float seconds, System.Action Callback)
    {
        yield return new WaitForSeconds(seconds);
        Callback();
    }
}
