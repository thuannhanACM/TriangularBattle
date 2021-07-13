using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriangularBattle
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        Point[] serializedPoints;
        [SerializeField]
        Transform linesRoot;
        
        private List<Line> connectingLines = new List<Line>();
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GetLevelSizes(out float maxWidth, out float maxHeight)
        {
            float minX = 0.0f, maxX = 0.0f, minZ = 0.0f, maxZ = 0.0f;
            foreach(var p in serializedPoints)
            {
                if(p.Pos.x<minX)
                    minX=p.Pos.x;

                if(p.Pos.x>maxX)
                    maxX=p.Pos.x;

                if(p.Pos.z<minZ)
                    minZ=p.Pos.z;

                if(p.Pos.z>maxZ)
                    maxZ=p.Pos.z;
            }

            maxWidth=maxX-minX;
            maxHeight=maxZ-minZ;
        }

        public bool IsLineAvailable(Point pA, Point pB)
        {
            bool isvalidLine = true;
            foreach(var line in connectingLines)
            {
                if((line.points[0]==pA&&line.points[1]==pB)||(line.points[0]==pB&&line.points[1]==pA))
                {
                    isvalidLine=false;
                    break;
                }

                if(Line.AreLinesIntersect(pA, pB, line))
                {
                    isvalidLine=false;
                    break;
                }
            }
            return isvalidLine;
        }

        public void AddLine(Point pA, Point pB)
        {
            Line lineTemplate = Resources.Load<Line>("Generals/Line");
            bool isvalidLine = true;
            
            if(IsLineAvailable(pA, pB))
            {
                Line line = Instantiate(lineTemplate, linesRoot);
                connectingLines.Add(line);
                line.SetPoints(pA, pB);
                Global.VibrationHaptic(1);
            }
            else
                Global.VibrationHaptic(3);
        }
    }
}