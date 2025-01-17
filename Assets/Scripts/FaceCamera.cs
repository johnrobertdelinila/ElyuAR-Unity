using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
    Transform cam;
    Vector3 targetAngle =Vector3.zero;

    private void Start()
    {
        cam = Camera.main.transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam);
        targetAngle = transform.localEulerAngles;
        targetAngle.x = 0;
        targetAngle.z = 0;
        transform.localEulerAngles = targetAngle;
    }
}
