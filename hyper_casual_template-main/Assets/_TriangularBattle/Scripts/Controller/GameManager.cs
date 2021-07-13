using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TriangularBattle
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public enum GameState
        {
            main_menu,
            start,
            show_player_turn,
            player_input,
        }

        public GameObject character;
        public Text LevelText;
        public GameObject levelRoot;
        public LineRenderer SelectingLineRenderer;
        public GameObject startLevelObject;
        public GameObject playerTurnAnimObject;

        public GameState currentGameState;
        private Level currentLevel = null;

        Point selectingPoint = null;

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
            currentLevel=Instantiate(levelPrefab, levelRoot.transform).GetComponent<Level>();
            currentLevel.transform.localScale=Vector3.one;
            ScalingLevel();

            LevelText.text="LEVEL "+Global.curLevel;
        }

        void ScalingLevel()
        {
            //first calculating the point in viewport
            Vector3 v3ViewPort = new Vector3(0.05f, 0.05f, 10);
            Vector3 v3BottomLeft = Camera.main.ViewportToWorldPoint(v3ViewPort);
            v3ViewPort.Set(0.95f, 0.95f, 10);
            Vector3 v3TopRight = Camera.main.ViewportToWorldPoint(v3ViewPort);

            currentLevel.GetLevelSizes(out float levelWidth, out float levelHeight);
            float scaleX = (v3TopRight.x-v3BottomLeft.x)/levelWidth;
            float scaleY = (v3TopRight.y-v3BottomLeft.y)/levelHeight;
            float minScale = Mathf.Min(scaleX, scaleY);
            currentLevel.transform.position+=Vector3.forward*(v3TopRight.z-v3BottomLeft.z)/2;
            currentLevel.transform.localScale=new Vector3(minScale, minScale, minScale);
        }

        // Update is called once per frame
        void Update()
        {
            switch(currentGameState)
            {
                case GameState.main_menu:
                    break;
                case GameState.start:
                    break;
                case GameState.player_input:
                    if(selectingPoint==null)
                    {
                        if(Input.GetMouseButtonDown(0))
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;
                            if(Physics.Raycast(ray, out hit))
                            {
                                if(hit.collider.gameObject.GetComponent<Point>()!=null)
                                {
                                    Point hitPoint = hit.collider.gameObject.GetComponent<Point>();
                                    selectingPoint=hitPoint;

                                    SelectingLineRenderer.SetPosition(0, hitPoint.transform.position);
                                    SelectingLineRenderer.SetPosition(1, hitPoint.transform.position);
                                    SelectingLineRenderer.gameObject.SetActive(true);
                                    SelectingLineRenderer.alignment=LineAlignment.TransformZ;
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        if(Input.GetMouseButton(0))
                        {
                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if(Physics.Raycast(ray, out hit))
                            {
                                DrawSelection(hit.point);
                            }
                        }
                        else if(Input.GetMouseButtonUp(0))
                        {
                            SelectingLineRenderer.gameObject.SetActive(false);
                            RaycastHit hit;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if(Physics.Raycast(ray, out hit))
                            {
                                Point p = hit.collider.gameObject.GetComponent<Point>();
                                if(p!=null &&selectingPoint.connectablePoints.Contains(p))
                                {
                                    currentLevel.AddLine(selectingPoint, p);
                                }
                            }
                            selectingPoint=null;
                        }
                    }
                    
                    break;
            }
        }

        public void SwitchState(GameState newState)
        {
            if(currentGameState!=newState)
                currentGameState=newState;

            switch(currentGameState)
            {
                case GameState.main_menu:
                    break;

                case GameState.start:
                    ShowStartAnimation();
                    break;
                case GameState.show_player_turn:
                    ShowPlayerTurnAnimation();
                    break;
            }
        }

        private void ShowStartAnimation()
        {
            startLevelObject.SetActive(true);
            Invoke("FinishStartAnimation", 2f);
        }

        private void FinishStartAnimation()
        {
            startLevelObject.SetActive(false);
            SwitchState(GameState.show_player_turn);
        }

        private void ShowPlayerTurnAnimation()
        {
            playerTurnAnimObject.SetActive(true);
            Invoke("FinishPlayerTurnAnimation", 2f);
        }

        private void FinishPlayerTurnAnimation()
        {
            playerTurnAnimObject.SetActive(false);
            SwitchState(GameState.player_input);
        }

        private void DrawSelection(Vector3 touchPos)
        {
            SelectingLineRenderer.SetPosition(1, touchPos);
        }
    }
}