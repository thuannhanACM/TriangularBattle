using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        [SerializeField]
        GameObject startLevelObject;
        [SerializeField] 
        GameObject playerTurnAnimObject;
        [SerializeField] 
        GameObject enemyTurnAnimObject;
        [SerializeField]
        GameObject winnerUIObject;
        [SerializeField]
        GameObject LoseUIObject;
        [SerializeField]
        CanvasGroup HintCanvasGroup;
        [SerializeField]
        GameObject PointingGameObject;
        [SerializeField]
        GameObject PointingImgGameObject;

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

        public void OnRestart()
        {
            Global.LoadScene(null);
        }

        public void NextLevel()
        {
            if(Global.curLevel>=CsvUtil.getLevelCount())
                Global.curLevel=1;
            else
                Global.curLevel++;
            if(Global.curLevel!=Data.CurrentLevel)
                Data.CurrentLevel=Global.curLevel;
            Global.LoadScene(null);
        }

        public void ShowStartAnimation()
        {
            startLevelObject.SetActive(true);
            Invoke("FinishStartAnimation", 2f);
        }

        private void FinishStartAnimation()
        {
            startLevelObject.SetActive(false);
            GameManager.instance.SwitchState(GameManager.GameState.show_player_turn);
        }

        public void ShowPlayerTurnAnimation()
        {
            playerTurnAnimObject.SetActive(true);
            Invoke("FinishPlayerTurnAnimation", 1.5f);
        }

        public void FinishPlayerTurnAnimation()
        {
            playerTurnAnimObject.SetActive(false);
            GameManager.instance.SwitchState(GameManager.GameState.player_input);
        }

        public void ShowEnemyTurnAnimation()
        {
            enemyTurnAnimObject.SetActive(true);
            Invoke("FinishEnemyTurnAnimation", 2f);
        }

        private void FinishEnemyTurnAnimation()
        {
            enemyTurnAnimObject.SetActive(false);
            GameManager.instance.SwitchState(GameManager.GameState.enemy_input);
        }

        public void ShowWinnerUI()
        {
            winnerUIObject.SetActive(true);
        }

        public void ShowLoseUI()
        {
            LoseUIObject.SetActive(true);
        }

        public IEnumerator ShowHintAtPoint(Vector3 worldStartPos, Vector3 worldEndPos)
        {
            Vector2 startViewPos = Camera.main.WorldToScreenPoint(worldStartPos);
            Vector2 endViewPos=Camera.main.WorldToScreenPoint(worldEndPos);

            PointingGameObject.transform.position=startViewPos;
            PointingImgGameObject.transform.localScale=Vector3.one*1.2f;
            HintCanvasGroup.gameObject.SetActive(true);

            HintCanvasGroup.DOFade(1.0f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            DOTween.Clear();

            PointingImgGameObject.transform.DOScale(1.0f, 0.5f);
            yield return new WaitForSeconds(0.55f);
            DOTween.Clear();

            PointingGameObject.transform.DOMove(endViewPos, 1f);
            yield return new WaitForSeconds(1.5f);
            DOTween.Clear();

            HintCanvasGroup.DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);
            DOTween.Clear();

            GameManager.instance.ResetIdleTime();
            HintCanvasGroup.gameObject.SetActive(false);
        }
    }
}