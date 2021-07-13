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

        private Level currentLevel = null;

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
            currentLevel = Instantiate(levelPrefab, levelRoot.transform).GetComponent<Level>();
            currentLevel.transform.localScale=Vector3.one;
            ScalingLevel();

            LevelText.text="LEVEL "+Global.curLevel;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ScalingLevel()
        {
            //first calculating the point in viewport
            Vector3 v3ViewPort = new Vector3(0.1f, 0.1f, 10);
            Vector3 v3BottomLeft = Camera.main.ViewportToWorldPoint(v3ViewPort);
            v3ViewPort.Set(0.9f, 0.9f, 10);
            Vector3 v3TopRight = Camera.main.ViewportToWorldPoint(v3ViewPort);

            currentLevel.GetLevelSizes(out float levelWidth, out float levelHeight);
            float scaleX = (v3TopRight.x-v3BottomLeft.x)/levelWidth;
            float scaleY = (v3TopRight.y-v3BottomLeft.y)/levelHeight;
            float minScale = Mathf.Min(scaleX, scaleY);
            currentLevel.transform.localScale=new Vector3(minScale, minScale, minScale);
        }
    }
}