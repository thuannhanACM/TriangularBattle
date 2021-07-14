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

        public string Id;
        public List<AnimatingCharacter> characters = new List<AnimatingCharacter>();
        public int Side = -1;

        public Point StartPoint;
        public Point EndPoint;
        public Vector3 MiddleOfLine { get 
            {
                Vector3 v = EndPoint.Pos-StartPoint.Pos;
                float l = v.magnitude;
                return StartPoint.Pos+(v.normalized*(l/2));
            } }

        public Dictionary<string, Triangle> relativeTriangles = new Dictionary<string, Triangle>();

        // Start is called before the first frame update
        void Start()
        {
            UpdateLine();
        }

        public void SetPoints(Point pA, Point pB)
        {
            StartPoint = pA;
            EndPoint = pB;
            UpdateLine();
        }

        public void AddTriangle(Triangle triangle)
        {
            if(!relativeTriangles.ContainsKey(triangle.Id))
                relativeTriangles.Add(triangle.Id, triangle);
        }

        // Update is called once per frame
        public void UpdateLine()
        {
            lineRenderer.SetPosition(0, StartPoint.Pos);
            lineRenderer.SetPosition(1, EndPoint.Pos);

            lineRenderer.alignment=LineAlignment.TransformZ;
        }

        public void AddCharacter(int side)
        {
            Vector3 lineVector = EndPoint.Pos-StartPoint.Pos;
            float Length = lineVector.magnitude;

            for(int i = 0; i<3; i++)
            {
                AnimatingCharacter newChar = Instantiate<AnimatingCharacter>(GameManager.instance.soldierPrefab[side]);
                newChar.transform.parent=transform;
                newChar.transform.localScale=Vector3.one*1.25f;
                newChar.transform.position=StartPoint.Pos+lineVector.normalized*((1+i)*Length/4);
                newChar.transform.eulerAngles=Vector3.zero;

                characters.Add(newChar);
            }
        }

        public void SwitchSide()
        {
            Side=(Side+1)%2;
            //play dust particles
            GameObject fx = Instantiate<GameObject>(GameManager.instance.KickOutExplosion);
            fx.transform.position=MiddleOfLine;
            Destroy(fx, 3.05f);

            Global.VibrationHaptic(4);

            //remove all characters
            foreach(var c in characters)
            {
                Destroy(c.gameObject);
            }
            characters.Clear();

            //then re add new Characters
            AddCharacter(Side);
        }

        public static bool AreLinesIntersect(Point p1, Point p2, Line line)
        {
            return isLinesIntersect(p1, p2, line.StartPoint, line.EndPoint);
        }

        public static bool AreLinesIntersect(Line line1, Line line2)
        {
            return isLinesIntersect(line1.StartPoint, line1.EndPoint, line2.StartPoint, line2.EndPoint);
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

            return doIntersect(pA1, pA2, pB1, pB2);
        }

        static bool onSegment(Point p, Point q, Point r)
        {
            if(q.Pos.x<=Mathf.Max(p.Pos.x, r.Pos.x)&&q.Pos.x>=Mathf.Min(p.Pos.x, r.Pos.x)&&
                q.Pos.z<=Mathf.Max(p.Pos.z, r.Pos.z)&&q.Pos.z>=Mathf.Min(p.Pos.z, r.Pos.z))
                return true;

            return false;
        }

        static int orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            float val = (q.Pos.z-p.Pos.z)*(r.Pos.x-q.Pos.x)-
                    (q.Pos.x-p.Pos.x)*(r.Pos.z-q.Pos.z);

            if(val==0) return 0; // colinear

            return (val>0) ? 1 : 2; // clock or counterclock wise
        }

        static bool doIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if(o1!=o2&&o3!=o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if(o1==0&&onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1
            if(o2==0&&onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if(o3==0&&onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if(o4==0&&onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }
        public static bool operator ==(Line l1, Line l2)
        {
            if(object.ReferenceEquals(l1, null)||object.ReferenceEquals(l2, null))
            {
                return (object.ReferenceEquals(l1, null)&&object.ReferenceEquals(l2, null));
            }
            return (l1.Id==l2.Id);
        }

        public static bool operator !=(Line l1, Line l2)
        {
            if(object.ReferenceEquals(l1, null)||object.ReferenceEquals(l2, null))
            {
                return !(object.ReferenceEquals(l1, null)&&object.ReferenceEquals(l2, null));
            }
            return !(l1.Id==l2.Id);
        }

    }
}
