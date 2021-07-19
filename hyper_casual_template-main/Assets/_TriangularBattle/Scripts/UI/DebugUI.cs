using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TriangularBattle
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField]
        GameObject DebugMenu;
        [SerializeField]
        GameObject DebugBtn;

        private int CurrentLevel;

        private void Start()
        {
        #if !UNITY_EDITOR
            DebugBtn.SetActive(false); //hide DEBUG btn on device
        #endif
            CurrentLevel=Data.CurrentLevel;
        }

        public void OnNextLvlBtnClicked()
        {
            if(Global.curLevel>=CsvUtil.getLevelCount())
                Global.curLevel=1;
            else
                Global.curLevel++;
            
            if(Global.curLevel!=Data.CurrentLevel)
                Data.CurrentLevel=Global.curLevel;
            if(IsChangedLevel())
                Global.LoadScene(null);
        }

        public void OnPreviousLvlBtnClicked()
        {
            Global.curLevel-=1;
            if(Global.curLevel<1)
                Global.curLevel=CsvUtil.getLevelCount();

            if(Global.curLevel!=Data.CurrentLevel)
                Data.CurrentLevel=Global.curLevel;
            if(IsChangedLevel())
                Global.LoadScene(null);
        }

        public void OnSelectLvlBtnClicked(InputField input)
        {
            int.TryParse(input.text, out int level);
            if(level==CurrentLevel)
                return;
            if(level<1) level=1;
            if(level>CsvUtil.getLevelCount()) Data.CurrentLevel=CsvUtil.getLevelCount();
            else Data.CurrentLevel=level;
            Global.LoadScene(null);
        }

        public void OnRestarLvlBtnClicked()
        {
            Global.LoadScene(null);
        }

        public void OnCloseLvlBtnClicked()
        {
            DebugMenu.SetActive(false);
        }

        public void OnOpenDebugMenu()
        {
            DebugMenu.SetActive(true);
        }

        private bool IsChangedLevel()
        {
            return CurrentLevel!=Data.CurrentLevel;
        }
    }
}
