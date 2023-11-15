using CameraControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LookAtCamera : MonoBehaviour
{

    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = cam.transform.eulerAngles;
    }
}
