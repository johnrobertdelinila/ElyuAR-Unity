using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public GameObject PlanetObject;
    public Vector3 RotationVector;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlanetObject.transform.Rotate(RotationVector * Time.deltaTime);
    }
}
