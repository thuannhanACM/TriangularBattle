using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace TriangularBattle
{
    public class Line : MonoBehaviour
    {
        [SerializeField]
        LineRenderer lineRenderer;
        
        public List<Point> points = new List<Point>();

        // Start is called before the first frame update
        void Start()
        {
            UpdateLine();
        }

        public void SetPoints(Point pA, Point pB)
        {
            points.Add(pA);
            points.Add(pB);
            UpdateLine();
        }

        // Update is called once per frame
        public void UpdateLine()
        {
            lineRenderer.SetPosition(0, points[0].Pos);
            lineRenderer.SetPosition(1, points[1].Pos);

            lineRenderer.alignment=LineAlignment.TransformZ;
        }

        public static bool AreLinesIntersect(Point p1, Point p2, Line line)
        {
            return isLinesIntersect(p1, p2, line.points[0], line.points[1]);
        }

        public static bool AreLinesIntersect(Line line1, Line line2)
        {
            return isLinesIntersect(line1.points[0], line1.points[1], line2.points[0], line2.points[1]);
        }

        private static bool checkPoints(Point pA, Point pB)
        {
            return (pA.Pos.x==pB.Pos.x&&pA.Pos.z==pB.Pos.z);
        }

        private static bool isLinesIntersect(Point pA1, Point pA2, Point pB1, Point pB2)
        {
            if(checkPoints(pA1, pB1)||
                checkPoints(pA1, pB2)||
                checkPoints(pA2, pB1)||
                checkPoints(pA2, pB2))
                return false;

            return ((Mathf.Max(pA1.Pos.x, pA2.Pos.x)>Mathf.Min(pB1.Pos.x, pB2.Pos.x))&&
                    (Mathf.Max(pB1.Pos.x, pB2.Pos.x)>Mathf.Min(pA1.Pos.x, pA2.Pos.x))&&
                    (Mathf.Max(pA1.Pos.z, pA2.Pos.z)>Mathf.Min(pB1.Pos.z, pB2.Pos.z))&&
                    (Mathf.Max(pB1.Pos.z, pB2.Pos.z)>Mathf.Min(pA1.Pos.z, pA2.Pos.z)));
        }
    }
}
