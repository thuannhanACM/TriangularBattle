using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TriangularBattle
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField]
        Text LevelText;
        [SerializeField]
        GameObject levelRoot;

        private void Awake()
        {
            instance=this;
        }

        // Start is called before the first frame update
        void Start()
        {
            Global.isSoundOn=Data.isSoundOn;
            Global.isVibrationOn=Data.isVibrationOn;
            Global.curLevel=Data.CurrentLevel;

            CsvUtil.parse("Levels");
            string[] LevelInfo = CsvUtil.find("Level"+Global.curLevel);
            var levelPrefab = Resources.Load<GameObject>("Levels/"+LevelInfo[0].Trim());
            Instantiate(levelPrefab, levelRoot.transform);

            LevelText.text="LEVEL "+Global.curLevel;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}