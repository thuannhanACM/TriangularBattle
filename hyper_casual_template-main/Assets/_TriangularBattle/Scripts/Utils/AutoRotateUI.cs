using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateUI : MonoBehaviour
{
    [SerializeField]
    Vector3 AngularSpeed = new Vector3(0, 0, 180);
    // Start is called before the first frame update
    private void Awake()
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var localEulerAngles = transform.localEulerAngles;
        transform.localEulerAngles += AngularSpeed * Time.deltaTime;
    }
}
