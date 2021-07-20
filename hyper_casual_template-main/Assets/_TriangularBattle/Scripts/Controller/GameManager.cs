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
            player_input_animation,
            check_win_lose,
            show_enemy_turn,
            enemy_input,
            enemy_input_animation
        }

        public static int PLAYER_SIDE = 0;
        public static int ENEMY_SIDE = 1;
        public MainUI mainUI;
        public AnimatingCharacter[] soldierPrefab;
        public Triangle[] trianglesPrefab;

        public Transform fxRoot;
        public GameObject winFX;
        public Text LevelText;
        public GameObject levelRoot;
        public GameObject KickOutExplosion;
        public Image playerScoreImage;
        public Transform fakeDraggingPos;
        public GameState currentGameState;
        public GameState lastGameState;

        private Level currentLevel = null;

        private Point lastSelectedPoint = null;
        private Point selectingPoint = null;

        private float playerIdleTime = 0.0f;
        private Line animatingLine = null;
        private Vector3 animatingLine_StartPos = Vector3.zero;

        private void Awake()
        {
            instance=this;
        }

        LineRenderer GetLineRenderer(int side)
        {
            return currentLevel.SelectingLineRenderers[side];
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateScore(0, 0);
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
            GetLineRenderer(0).alignment=LineAlignment.TransformZ;
            GetLineRenderer(1).alignment=LineAlignment.TransformZ;
            LevelText.text="LEVEL "+Global.curLevel;
        }

        void ScalingLevel()
        {
            //first calculating the point in viewport
            Vector3 v3ViewPort = new Vector3(0.0f, 0.0f, 15.0f);
            Vector3 v3BottomLeft = Camera.main.ViewportToWorldPoint(v3ViewPort);
            v3ViewPort.Set(1.0f, 1.0f, 15);
            Vector3 v3TopRight = Camera.main.ViewportToWorldPoint(v3ViewPort);

            currentLevel.GetLevelAnchors(out float levelWidth, out float levelHeight);
            float scaleX = Mathf.Abs((v3TopRight.x-v3BottomLeft.x)/levelWidth);
            float scaleY = Mathf.Abs((v3TopRight.y-v3BottomLeft.y)/levelHeight);
            float minScale = Mathf.Min(scaleX, scaleY);
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
                case GameState.player_input_animation:
                case GameState.enemy_input_animation:
                    animatingLine.SetPositions(animatingLine_StartPos, fakeDraggingPos.position);
                    break;
                case GameState.enemy_input:
#if USE_NO_ENEMY_AI
                    UpdateMouseInput(ENEMY_SIDE);
#else
                    if(selectingPoint!=null)
                    {
                        GetLineRenderer(ENEMY_SIDE).SetPosition(1, fakeDraggingPos.position);
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
                        selectingPoint=null;
                        lastSelectedPoint=null;
                        mainUI.ShowPlayerTurnAnimation();
                        break;
                    case GameState.player_input:
                        ResetIdleTime();
                        break;
                    case GameState.player_input_animation:
                        fakeDraggingPos.DOMove(lastSelectedPoint.Pos, 0.5f);
                        Invoke("OnPlayerInputAnimatingDone", 0.5f);
                        break; 
                    case GameState.enemy_input_animation:
                        fakeDraggingPos.DOMove(lastSelectedPoint.Pos, 0.5f);
                        Invoke("OnEnemyInputAnimatingDone", 0.5f);
                        break;
                    case GameState.check_win_lose:
                        StartCoroutine(CheckAndSwitchTurn());
                        break;
                    case GameState.show_enemy_turn:
                        selectingPoint=null;
                        mainUI.ShowEnemyTurnAnimation();
                        break;
#if !USE_NO_ENEMY_AI
                    case GameState.enemy_input:
                        selectingPoint=FindAvailablePoint(lastSelectedPoint);
                        if(selectingPoint==null)
                        {
                            foreach(var p in currentLevel.Points)
                            {
                                selectingPoint=FindAvailablePoint(p);
                                if(selectingPoint!=null)
                                {
                                    lastSelectedPoint=p;
                                    break;
                                }
                            }

                            if(selectingPoint==null)
                            {
                                StartCoroutine(ShowWinUI());
                                break;
                            }
                        }

                        {
                            GetLineRenderer(ENEMY_SIDE).gameObject.SetActive(true);
                            GetLineRenderer(ENEMY_SIDE).SetPosition(0, lastSelectedPoint.Pos);
                            GetLineRenderer(ENEMY_SIDE).SetPosition(1, lastSelectedPoint.Pos);
                            fakeDraggingPos.position=lastSelectedPoint.Pos;
                            fakeDraggingPos.DOMove(selectingPoint.Pos, 0.5f);
                            Invoke("MakeEnemyLine", 0.5f);
                        }
                        break;
#endif
                }
            }
        }

        private void UpdateSelection(int side, Vector3 touchPos)
        {
            GetLineRenderer(side).SetPosition(1, touchPos);
        }

        private bool UpdateMouseInput(int side)
        {
            if(Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit))
                {
                    Point p = hit.transform.GetComponent<Point>();
                    if(p!=null)
                    {
                        if(selectingPoint==null)
                        {
                            selectingPoint=p;
                            selectingPoint.ToggleHilightOnRelativePoints(true);

                            GetLineRenderer(side).SetPosition(0, p.Pos);
                            GetLineRenderer(side).SetPosition(1, p.Pos);
                            GetLineRenderer(side).gameObject.SetActive(true);
                        }
                        else
                        {
                            if(p!=null&&selectingPoint.AvailableRelativePoints.Contains(p))
                                UpdateSelection(side, p.Pos);
                            else
                                UpdateSelection(side, hit.point);// new Vector3(hit.point.x, 0.1f, hit.point.z));
                        }
                    }
                    else if(selectingPoint!=null)
                    {
                        UpdateSelection(side, hit.point);// new Vector3(hit.point.x, 0.1f, hit.point.z));
                    }
                }
                return true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                GetLineRenderer(side).gameObject.SetActive(false);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit))
                {
                    Point p = hit.collider.gameObject.GetComponent<Point>();
                    if(p!=null&&selectingPoint.AvailableRelativePoints.Contains(p))
                    {
                        var line = currentLevel.AddLine(side, selectingPoint, p);
                        if(line!=null)
                        {
                            animatingLine=line;
                            animatingLine_StartPos=selectingPoint.Pos;
                            fakeDraggingPos.position=selectingPoint.Pos;
                            lastSelectedPoint=p;
                            StartCoroutine(animatingLine.AddCharacter(PLAYER_SIDE, currentLevel.numArmyPerLine, currentLevel.armyScale, 0.15f));
                            SwitchState(GameState.player_input_animation);
                        }
                    }
                }

                if(selectingPoint!=null)
                    selectingPoint.ToggleHilightOnRelativePoints(false);

                selectingPoint=null;
                return true;
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
                if(lastGameState==GameState.player_input_animation)
                    SwitchState(GameState.show_enemy_turn);
                else if(lastGameState==GameState.enemy_input_animation)
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
            var points = startP.AvailableRelativePoints.OrderBy(x => random.Next()).ToList();
            foreach(var p in points)
            {
                if(currentLevel.IsLineAvailable(startP, p))
                    return p;
            }
            return null;
        }

        private void MakeEnemyLine()
        {
            GetLineRenderer(ENEMY_SIDE).gameObject.SetActive(false);
            var line = currentLevel.AddLine(ENEMY_SIDE, lastSelectedPoint, selectingPoint);
            if(line!=null)
            {
                animatingLine=line;
                animatingLine_StartPos=lastSelectedPoint.Pos;
                fakeDraggingPos.position=lastSelectedPoint.Pos;
                lastSelectedPoint=selectingPoint;
                StartCoroutine(animatingLine.AddCharacter(ENEMY_SIDE, currentLevel.numArmyPerLine, currentLevel.armyScale, 0.15f));
                SwitchState(GameState.enemy_input_animation);
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
                            StartCoroutine(mainUI.ShowHintAtPoint(p.Pos, endPoint.Pos));
                        }
                    }
                }
            }
        }

        public void ResetIdleTime()
        {
            playerIdleTime=1.5f;
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

        private void OnPlayerInputAnimatingDone()
        {
            animatingLine.SetPositions(animatingLine.StartPoint.Pos, animatingLine.EndPoint.Pos);
            currentLevel.CreateTriangles(PLAYER_SIDE, animatingLine.StartPoint);
            currentLevel.CreateTriangles(PLAYER_SIDE, animatingLine.EndPoint);
            UpdateScore(currentLevel.GetScore(PLAYER_SIDE), currentLevel.GetScore(ENEMY_SIDE));
            SwitchState(GameState.check_win_lose);
            animatingLine=null;
        }

        private void OnEnemyInputAnimatingDone()
        {
            animatingLine.SetPositions(animatingLine.StartPoint.Pos, animatingLine.EndPoint.Pos);
            currentLevel.CreateTriangles(ENEMY_SIDE, animatingLine.StartPoint);
            currentLevel.CreateTriangles(ENEMY_SIDE, animatingLine.EndPoint);
            UpdateScore(currentLevel.GetScore(PLAYER_SIDE), currentLevel.GetScore(ENEMY_SIDE));
            SwitchState(GameState.check_win_lose);
            animatingLine=null;
        }
    }
}