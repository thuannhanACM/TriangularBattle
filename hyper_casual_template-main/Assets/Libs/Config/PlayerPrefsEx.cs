using UnityEngine;

namespace HyperCasualTemplate
{
    public static class PlayerPrefsEx
    {
        public static bool GetBool(string name, bool defValue)
        {
            return PlayerPrefs.GetInt(name, ((bool) defValue) ? 1 : 0) != 0;
        }

        public static void SetBool(string name, bool value)
        {
            PlayerPrefs.SetInt(name, ((bool) value) ? 1 : 0);
        }
    }
}