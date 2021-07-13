using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class Line : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    GameObject[] points;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLine();
    }

    // Update is called once per frame
    public void UpdateLine()
    {
        lineRenderer.SetPosition(0, points[0].transform.position);
        lineRenderer.SetPosition(1, points[1].transform.position);
    }
}