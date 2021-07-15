//#define USE_NO_ENEMY_AI
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

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
            check_win_lose,
            show_enemy_turn,
            enemy_input
        }

        public static int PLAYER_SIDE = 0;
        public static int ENEMY_SIDE = 1;
        public MainUI mainUI;
        public AnimatingCharacter[] soldierPrefab;
        public Triangle[] trianglesPrefab;
        public LineRenderer[] SelectingLineRenderers;

        public Transform fxRoot;
        public GameObject winFX;
        public Text LevelText;
        public GameObject levelRoot;
        public GameObject KickOutExplosion;
        public Image playerScoreImage;
        public Transform enemyFakeDraggingPos;
        public GameState currentGameState;
        public GameState lastGameState;

        private Level currentLevel = null;

        private Point lastSelectedPoint = null;
        private Point selectingPoint = null;

        private float playerIdleTime = 0.0f;

        private void Awake()
        {
            instance=this;
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateScore(0, 0);
            SelectingLineRenderers[0].alignment=LineAlignment.TransformZ;
            SelectingLineRenderers[1].alignment=LineAlignment.TransformZ;
            currentGameState=lastGameState=GameState.main_menu;

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
            Vector3 v3ViewPort = new Vector3(0.0f, 0.0f, 10);
            Vector3 v3BottomLeft = Camera.main.ViewportToWorldPoint(v3ViewPort);
            v3ViewPort.Set(1.0f, 1.0f, 10);
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
                    if(!UpdateMouseInput(PLAYER_SIDE))
                        CheckedAndShowHint();
                    else
                        ResetIdleTime();
                    break;
                case GameState.enemy_input:
#if USE_NO_ENEMY_AI
                    UpdateMouseInput(ENEMY_SIDE);
#else
                    if(selectingPoint!=null)
                    {
                        SelectingLineRenderers[ENEMY_SIDE].SetPosition(1, enemyFakeDraggingPos.position);
                    }
#endif
                    break;
            }
        }

        public void SwitchState(GameState newState)
        {
            if(currentGameState!=newState)
            {
                lastGameState=currentGameState;
                currentGameState=newState;
                switch(currentGameState)
                {
                    case GameState.main_menu:
                        break;
                    case GameState.start:
                        mainUI.ShowStartAnimation();
                        break;
                    case GameState.show_player_turn:
                        mainUI.ShowPlayerTurnAnimation();
                        break;
                    case GameState.player_input:
                        ResetIdleTime();
                        break;
                    case GameState.check_win_lose:
                        selectingPoint=null;
                        StartCoroutine(CheckAndSwitchTurn());
                        break;
                    case GameState.show_enemy_turn:
                        mainUI.ShowEnemyTurnAnimation();
                        break;
#if !USE_NO_ENEMY_AI
                    case GameState.enemy_input:
                        selectingPoint=FindAvailablePoint(lastSelectedPoint);
                        if(selectingPoint==null)
                        {
                            //force Player win => as in document
                            StartCoroutine(ShowWinUI());
                        }
                        else
                        {
                            SelectingLineRenderers[ENEMY_SIDE].gameObject.SetActive(true);
                            SelectingLineRenderers[ENEMY_SIDE].SetPosition(0, lastSelectedPoint.Pos);
                            SelectingLineRenderers[ENEMY_SIDE].SetPosition(1, lastSelectedPoint.Pos);
                            enemyFakeDraggingPos.position=lastSelectedPoint.Pos;
                            enemyFakeDraggingPos.DOMove(selectingPoint.Pos, 0.5f);
                            Invoke("MakeEnemyLine", 0.5f);
                        }
                        break;
#endif
                }
            }
        }

        private void UpdateSelection(int side, Vector3 touchPos)
        {
            SelectingLineRenderers[side].SetPosition(1, touchPos);
        }

        private bool UpdateMouseInput(int side)
        {
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

                            SelectingLineRenderers[side].SetPosition(0, hitPoint.transform.position);
                            SelectingLineRenderers[side].SetPosition(1, hitPoint.transform.position);
                            SelectingLineRenderers[side].gameObject.SetActive(true);

                            return true;
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
                        UpdateSelection(side, hit.point);
                    }
                    return true;
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    SelectingLineRenderers[side].gameObject.SetActive(false);
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(ray, out hit))
                    {
                        Point p = hit.collider.gameObject.GetComponent<Point>();
                        if(p!=null&&selectingPoint.connectablePoints.Contains(p))
                        {
                            var line = currentLevel.AddLine(side, selectingPoint, p);
                            if(line!=null)
                            {
                                currentLevel.CreateTriangles(side, line.StartPoint);
                                currentLevel.CreateTriangles(side, line.EndPoint);
                                UpdateScore(currentLevel.GetScore(PLAYER_SIDE), currentLevel.GetScore(ENEMY_SIDE));
                                SwitchState(GameState.check_win_lose);
                                lastSelectedPoint=p;
                            }
                        }
                    }
                    selectingPoint=null;
                    return true;
                }
            }
            return false;
        }

        public bool IsGameEnd()
        {
            return currentLevel.IsGameEnd();
        }

        private IEnumerator CheckAndSwitchTurn()
        {
            //checking win/lose
            if(IsGameEnd())
            {
                int playerScore = currentLevel.GetScore(PLAYER_SIDE);
                int enemyScore = currentLevel.GetScore(ENEMY_SIDE);
                if(playerScore>enemyScore)
                    StartCoroutine(ShowWinUI());
                else
                    StartCoroutine(ShowLoseUI());
            }
            else//if not switch for next turn
            {
                yield return new WaitForSeconds(0.5f);
                if(lastGameState==GameState.player_input)
                    SwitchState(GameState.show_enemy_turn);
                else if(lastGameState==GameState.enemy_input)
                    SwitchState(GameState.show_player_turn);
            }
        }

        public void UpdateScore(int playerScore, int enemyScore)
        {
            if(playerScore+enemyScore==0)
            {
                playerScoreImage.fillAmount=0.5f;
            }
            else
            {
                playerScoreImage.DOFillAmount((float)playerScore/(playerScore+enemyScore), 0.5f);
            }
        }

        private IEnumerator ShowWinUI()
        {
            yield return new WaitForSeconds(0.25f);
            mainUI.ShowWinnerUI();
            GameObject fx = Instantiate<GameObject>(winFX, fxRoot);
        }

        private IEnumerator ShowLoseUI()
        {
            yield return new WaitForSeconds(0.25f);
            mainUI.ShowLoseUI();
        }

        private Point FindAvailablePoint(Point startP)
        {
            System.Random random = new System.Random();
            var points = startP.connectablePoints.OrderBy(x => random.Next()).ToList();
            foreach(var p in points)
            {
                if(currentLevel.IsLineAvailable(startP, p))
                    return p;
            }
            return null;
        }

        private void MakeEnemyLine()
        {
            SelectingLineRenderers[ENEMY_SIDE].gameObject.SetActive(false);
            var line = currentLevel.AddLine(ENEMY_SIDE, selectingPoint, lastSelectedPoint);
            if(line!=null)
            {
                currentLevel.CreateTriangles(ENEMY_SIDE, line.StartPoint);
                currentLevel.CreateTriangles(ENEMY_SIDE, line.EndPoint);
                UpdateScore(currentLevel.GetScore(PLAYER_SIDE), currentLevel.GetScore(ENEMY_SIDE));
                SwitchState(GameState.check_win_lose);

                lastSelectedPoint=null;
            }
        }

        public void CheckedAndShowHint()
        {
            if(selectingPoint!=null)
                return;

            if(playerIdleTime>0.0f)
            {
                //show Hint
                playerIdleTime-=Time.deltaTime;
                if(playerIdleTime<0)
                {
                    //find Available Point

                    var shufflePoints = ShufflePoints(currentLevel.Points);
                    foreach(var p in shufflePoints)
                    {
                        Point endPoint = FindAvailablePoint(p);
                        if(endPoint!=null)
                        {
                            mainUI.ShowHintAtPoint(p.Pos, endPoint.Pos);
                        }
                    }
                }
            }
        }

        public void ResetIdleTime()
        {
            playerIdleTime=5.0f;
        }

        public Point[] ShufflePoints(List<Point> inPoints)
        {
            return ShufflePoints(inPoints.ToArray());
        }

        public Point[] ShufflePoints(Point[] inPoints)
        {
            System.Random random = new System.Random();
            var points = inPoints.OrderBy(x => random.Next()).ToArray();
            return points;
        }
    }
}