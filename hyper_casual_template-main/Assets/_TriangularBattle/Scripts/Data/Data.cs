using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static bool isSoundOn
    {
        get
        {
            return PlayerPrefs.GetInt("sound", 1)!=0;
        }
        set
        {
            PlayerPrefs.SetInt("sound", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool isVibrationOn
    {
        get
        {
            return PlayerPrefs.GetInt("vibrate", 1)!=0;
        }
        set
        {
            PlayerPrefs.SetInt("vibrate", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }

    public static int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt("curLvl", 1);
        }
        set
        {
            PlayerPrefs.SetInt("curLvl", value);
            PlayerPrefs.Save();
        }
    }

}
