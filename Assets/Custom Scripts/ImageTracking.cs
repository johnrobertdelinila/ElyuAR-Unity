using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private ModelInfo[] modelInfos;
    
    private Dictionary<string, GameObject> activeModels = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        
        // Initially hide all models
        foreach (var modelInfo in modelInfos)
        {
            if (modelInfo.prefab != null)
            {
                modelInfo.prefab.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle added images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            ToggleModelVisibility(trackedImage, true);
        }

        // Handle updated images
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateModelVisibility(trackedImage);
        }

        // Handle removed images
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            ToggleModelVisibility(trackedImage, false);
        }
    }

    private void ToggleModelVisibility(ARTrackedImage trackedImage, bool show)
    {
        string imageName = trackedImage.referenceImage.name;
        ModelInfo info = System.Array.Find(modelInfos, x => x.name == imageName);
        
        if (info?.prefab == null) return;

        info.prefab.SetActive(show);
        
        if (show)
        {
            activeModels[imageName] = info.prefab;
            Debug.Log($"[AR_TRACKING] Showing model for {imageName}");
        }
        else
        {
            activeModels.Remove(imageName);
            Debug.Log($"[AR_TRACKING] Hiding model for {imageName}");
        }
    }

    private void UpdateModelVisibility(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        if (activeModels.TryGetValue(imageName, out GameObject model))
        {
            bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
            model.SetActive(isTracking);

            if (isTracking)
            {
                // Update only the position
                model.transform.position = trackedImage.transform.position;
                Debug.Log($"[AR_TRACKING] Updated {imageName} position");
            }
        }
    }
}