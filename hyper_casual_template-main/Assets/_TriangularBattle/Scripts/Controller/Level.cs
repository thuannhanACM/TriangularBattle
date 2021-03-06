using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TriangularBattle
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        Point[] serializedPoints;
        [SerializeField]
        Transform linesRoot;
        [SerializeField]
        Transform triangleRoot;
        [SerializeField]
        public Transform Anchor_BottomLeft;
        [SerializeField]
        public Transform Anchor_TopRight;
        [SerializeField]
        public LineRenderer[] SelectingLineRenderers;
        [SerializeField]
        public int numArmyPerLine = 3;
        [SerializeField]
        public float armyScale = 2.0f;

        private List<Point> availablePoints = new List<Point>();
        public Point[] Points { get { return availablePoints.ToArray(); } }

        private Dictionary<string, Line> connectingLines = new Dictionary<string, Line>();
        private Dictionary<string, Triangle> connectingTriangles = new Dictionary<string, Triangle>();
        Line lineTemplate = null;
        // Start is called before the first frame update
        void Start()
        {
            availablePoints=new List<Point>(serializedPoints);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GetLevelAnchors(out float maxWidth, out float maxHeight)
        {
            maxWidth=Anchor_TopRight.position.x-Anchor_BottomLeft.position.x;
            maxHeight=Anchor_TopRight.position.z-Anchor_BottomLeft.position.z;
        }

        public bool IsLineAvailable(Point pA, Point pB)
        {
            bool isvalidLine = true;
            foreach(var line in connectingLines)
            {
                if((line.Value.StartPoint==pA&&line.Value.EndPoint==pB)||(line.Value.StartPoint==pB&&line.Value.EndPoint==pA))
                {
                    isvalidLine=false;
                    break;
                }

                if(Line.AreLinesIntersect(pA, pB, line.Value))
                {
                    isvalidLine=false;
                    break;
                }
            }
            return isvalidLine;
        }

        public Line AddLine(int side, Point pA, Point pB, bool isTriggerByTriangle = false)
        {
            if(lineTemplate==null)
                lineTemplate=Resources.Load<Line>("Line");

            bool isvalidLine = true;

            if(IsLineAvailable(pA, pB))
            {
                Line line = Instantiate(lineTemplate, linesRoot);
                string id = "";
                if(pA.index<pB.index)
                    id=pA.index+"_"+pB.index;
                else
                    id=pB.index+"_"+pA.index;
                line.Id=id;
                connectingLines.Add(id, line);

                line.SetPoints(pA, pB);
                Global.VibrationHaptic(1);
                line.Side=side;
                if(isTriggerByTriangle)
                    StartCoroutine(line.AddCharacter(side, numArmyPerLine, armyScale, 0.05f));
                pA.AddLine(this, line);
                pB.AddLine(this, line);

                if(GetAvailableLinesNum(pA)==0)
                    availablePoints.Remove(pA);

                if(GetAvailableLinesNum(pB)==0)
                    availablePoints.Remove(pB);

                return line;
            }
            else
                Global.VibrationHaptic(3);
            return null;
        }

        public void CreateTriangles(int side, Point inPoint)
        {
            List<Line> lines = inPoint.FindRelativesLine(side);
            if(lines.Count>=2)
            {
                for(int i = 0; i<lines.Count; i++)
                {
                    Point targetPoint_i = lines[i].StartPoint!=inPoint ? lines[i].StartPoint : lines[i].EndPoint;
                    for(int j = i+1; j<lines.Count; j++)
                    {
                        Point targetPoint_j = lines[j].StartPoint!=inPoint ? lines[j].StartPoint : lines[j].EndPoint;
                        if(targetPoint_j.connectablePoints.Contains(targetPoint_i))
                        {
                            List<Point> indexList = new List<Point>() { inPoint, targetPoint_i, targetPoint_j };
                            List<Point> sortedList = indexList.OrderBy(p1 => p1.index).ToList();
                            string id = sortedList[0].index+"_"+sortedList[1].index+"_"+sortedList[2].index;
                            if(!connectingTriangles.ContainsKey(id))
                            {
                                //check if there is a line conntect 2 points above
                                Line newLine = FindLineBetweenPoints(targetPoint_i, targetPoint_j);
                                if(newLine==null)
                                {
                                    //create new line
                                    newLine=AddLine(side, targetPoint_i, targetPoint_j, true);
                                }
                                else
                                {
                                    if(newLine.Side!=side)
                                    {
                                        if(newLine.relativeTriangles.Count>0)
                                            continue;
                                        else
                                            newLine.SwitchSide(numArmyPerLine, armyScale);
                                    }
                                }
                                if(newLine!=null)
                                {
                                    Triangle triangle = Instantiate<Triangle>(GameManager.instance.trianglesPrefab[side]);
                                    triangle.Id=id;
                                    triangle.transform.parent=triangleRoot;
                                    triangle.SetPositions(sortedList);
                                    triangle.Side=side;
                                    connectingTriangles.Add(id, triangle);

                                    //set references to lines
                                    newLine.AddTriangle(triangle);
                                    lines[i].AddTriangle(triangle);
                                    lines[j].AddTriangle(triangle);

                                    //set references to points
                                    foreach(var p in sortedList)
                                        p.AddTriangle(triangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        public Line FindLineBetweenPoints(Point pA, Point pB)
        {
            string id = "";
            if(pA.index<pB.index)
                id=pA.index+"_"+pB.index;
            else
                id=pB.index+"_"+pA.index;

            if(connectingLines.ContainsKey(id))
                return connectingLines[id];
            return null;
        }

        public bool IsGameEnd()
        {
            foreach(var p in serializedPoints)
            {
                foreach(var relativePoint in p.AvailableRelativePoints)
                {
                    if(IsLineAvailable(p, relativePoint))
                        return false;
                }
            }
            return true;
        }

        public int GetScore(int side)
        {
            int numLines = 0;
            foreach(var line in connectingLines)
            {
                if(line.Value.Side==side)
                    numLines++;
            }
            return numLines;
        }

        public int GetAvailableLinesNum(Point point)
        {
            return point.AvailableRelativePoints.Length;
        }
    }
}