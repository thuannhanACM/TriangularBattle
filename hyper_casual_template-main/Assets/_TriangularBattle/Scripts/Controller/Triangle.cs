using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriangularBattle
{
    [RequireComponent(typeof(MeshFilter))]
    public class Triangle : MonoBehaviour
    {
        public string Id;
        public Point[] points;
        public Point P1 { get { return points[0]; } }
        public Point P2 { get { return points[1]; } }        
        public Point P3 { get { return points[2]; } }

        public int Side = -1;
        private Mesh mesh;

        // Start is called before the first frame update
        void Start()
        {
            mesh=GetComponent<MeshFilter>().mesh;
            CreateTriangleMesh();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPositions(List<Point> inPoints)
        {
            this.points=inPoints.ToArray();
        }

        void CreateTriangleMesh()
        {
            Vector3[] vertices = new Vector3[3] { P1.Pos - Vector3.up * 0.01f, P2.Pos-Vector3.up*0.01f, P3.Pos-Vector3.up*0.01f };
            mesh.Clear();
            mesh.vertices=vertices;

            Vector3 v12 = P2.Pos-P1.Pos;
            Vector3 v23 = P3.Pos-P2.Pos;
            Vector3 cross = Vector3.Cross(v12, v23);
            if(Vector3.Dot(Vector3.up, cross)>0.0f)
            {
                int[] triangles = new int[3] { 0, 1, 2 };
                mesh.triangles=triangles;
            }
            else
            {
                int[] triangles = new int[3] { 2, 1, 0 };
                mesh.triangles=triangles;
            }
        }

        public static bool operator ==(Triangle t1, Triangle t2)
        {
            if(object.ReferenceEquals(t1, null)||object.ReferenceEquals(t2, null))
            {
                return (object.ReferenceEquals(t1, null)&&object.ReferenceEquals(t2, null));
            }
            return (t1.Id==t2.Id);
        }

        public static bool operator !=(Triangle t1, Triangle t2)
        {
            if(object.ReferenceEquals(t1, null)||object.ReferenceEquals(t2, null))
            {
                return (object.ReferenceEquals(t1, null)&&object.ReferenceEquals(t2, null));
            }
            return !(t1.Id==t2.Id);
        }
    }
}
