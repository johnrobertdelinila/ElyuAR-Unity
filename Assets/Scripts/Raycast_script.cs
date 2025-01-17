using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Raycast_script : MonoBehaviour
{
    public GameObject spawnPrefab;
    GameObject spawnedObject;
    bool objectSpawned = false;
    ARRaycastManager arrayman;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    ARPlaneManager planeManager;
    [SerializeField] ARSession arSession;
    [SerializeField] Slider slider;
    void Start()
    {
        objectSpawned = false;
        arrayman = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if(arrayman.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                Vector3 hitPosition = hitPose.position;
                hitPosition.y += 0.2f;
                if(!objectSpawned )
                {
                    spawnedObject = Instantiate(spawnPrefab, hitPosition, spawnPrefab.transform.rotation);
                    objectSpawned = true;
                    planeManager.enabled = false;
                }
                else
                {
                    spawnedObject.transform.position = hitPosition;
                } 
            }
        }
    }

    public void ResetPlanes()
    {
        
        arSession.Reset();
        planeManager.enabled = true;
        if(spawnedObject != null)
        {
            Destroy(spawnedObject);
            objectSpawned = false;
        }
        
    }

    public void adjustScale()
    {
        Debug.Log(slider.value);
        if(slider.value > 0f )
        {
            spawnedObject.transform.localScale = new Vector3(slider.value, slider.value, slider.value);

        }

    }
}
