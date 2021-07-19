using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriangularBattle
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        public int index = 0;
        [SerializeField]
        public List<Point> connectablePoints;
        [SerializeField]
        private GameObject hilight;

        private List<Point> availableRelativePoints;
        public Point[] AvailableRelativePoints { get { return availableRelativePoints.ToArray(); } }

        public Dictionary<string, Line> relativeLines = new Dictionary<string, Line>();
        public Dictionary<string, Triangle> relativeTriangles = new Dictionary<string, Triangle>();

        public Vector3 Pos
        {
            get { return transform.position; }
        }
        // Start is called before the first frame update
        void Start()
        {
            availableRelativePoints=new List<Point>(connectablePoints);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddLine(Level level, Line line)
        {
            if(!relativeLines.ContainsKey(line.Id))
                relativeLines.Add(line.Id, line);
            
            if(line.StartPoint==this)
                availableRelativePoints.Remove(line.EndPoint);
            else if(line.EndPoint=this)
                availableRelativePoints.Remove(line.StartPoint);

            List<Point> shouldRemovePoints = new List<Point>();
            foreach(var p in availableRelativePoints)
            {
                if(!level.IsLineAvailable(this, p))
                {
                    shouldRemovePoints.Add(p);
                }
            }

            foreach(var removeP in shouldRemovePoints)
            {
                availableRelativePoints.Remove(removeP);
            }
        }

        public void AddTriangle(Triangle triangle)
        {
            if(!relativeTriangles.ContainsKey(triangle.Id))
                relativeTriangles.Add(triangle.Id, triangle);
        }

        public List<Line> FindRelativesLine(int side)
        {
            List<Line> result = new List<Line>();
            foreach(var line in relativeLines)
            {
                if(line.Value.Side==side)
                    result.Add(line.Value);
            }
            return result;
        }

        public void ToggleHilightOnRelativePoints(bool isHilight)
        {
            if(isHilight)
            {
                foreach(var p in AvailableRelativePoints)
                    p.ToggleHilight(true);
            }
            else
            {
                foreach(var p in connectablePoints)
                    p.ToggleHilight(false);
            }
        }

        public void ToggleHilight(bool isHilight)
        {
            hilight.SetActive(isHilight);
        }

        public static bool operator == (Point p1, Point p2)
        {
            if(object.ReferenceEquals(p1, null) ||object.ReferenceEquals(p2, null))
            {
                return (object.ReferenceEquals(p1, null)&&object.ReferenceEquals(p2, null));
            }
            return (p1.index == p2.index);
        }

        public static bool operator != (Point p1, Point p2)
        {
            if(object.ReferenceEquals(p1, null) ||object.ReferenceEquals(p2, null))
            { 
                return !(object.ReferenceEquals(p1, null) && object.ReferenceEquals(p2, null));
            }
            return !(p1.index==p2.index);
        }
    }
}
