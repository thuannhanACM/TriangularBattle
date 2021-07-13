using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TriangularBattle
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField]
        GameObject SettingPanel;
        [SerializeField]
        GameObject StartButton;
        [SerializeField]
        Toggle SoundToggle;
        [SerializeField]
        Toggle VibrateToggle;

        private void Start()
        {
            SoundToggle.isOn = Data.isSoundOn;

            VibrateToggle.isOn = Data.isVibrationOn;
            if(VibrateToggle.isOn) 
                Global.VibrationHaptic(0);
        }

        public void OnStartBtnClicked()
        {
            StartButton.SetActive(false);
            GameManager.instance.SwitchState(GameManager.GameState.start);
        }

        public void OnSettingBtnClicked()
        {
            SettingPanel.SetActive(true);
        }

        public void OnSettingCloseBtnClicked()
        {
            SettingPanel.SetActive(false);
        }

        public void OnSoundChanged(bool enable)
        {
            Global.isSoundOn=SoundToggle.isOn;
            if(Global.isSoundOn!=Data.isSoundOn)
                Data.isSoundOn=Global.isSoundOn;
        }

        public void OnVibrateChanged(bool enable)
        {
            Global.isVibrationOn=VibrateToggle.isOn;
            if(Global.isVibrationOn!=Data.isVibrationOn)
                Data.isVibrationOn=Global.isVibrationOn;

            if(VibrateToggle.isOn)
                Global.VibrationHaptic(0);
        }

        public void OnPrivacyBtnClicked()
        {
            Application.OpenURL("https://site.nicovideo.jp/app/privacy/");
        }

        public void OnAppTermsBtnClicked()
        {
            Application.OpenURL("https://site.nicovideo.jp/app/terms/");
        }
    }
}