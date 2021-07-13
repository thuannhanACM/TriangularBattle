using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    Point[] serializedPoints;

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
        float minX = 0.0f, maxX = 0.0f, minY = 0.0f, maxY = 0.0f;
        foreach(var p in serializedPoints)
        {
            if(p.transform.position.x<minX)
                minX=p.transform.position.x;

            if(p.transform.position.x>maxX)
                maxX=p.transform.position.x;

            if(p.transform.position.y<minY)
                minY=p.transform.position.y;

            if(p.transform.position.y>maxY)
                maxY=p.transform.position.y;
        }

        maxWidth=maxX-minX;
        maxHeight=maxY-minY;
    }
}
