using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriangularBattle
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        Point[] connectablePoints;

        public Vector3 Pos
        {
            get { return transform.position; }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowFeedback()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.material.color=Color.red;
        }
    }}
