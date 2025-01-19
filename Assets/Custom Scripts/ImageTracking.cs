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
        
        // Clean up when leaving scene
        foreach (var model in activeModels.Values)
        {
            if (model != null)
            {
                model.SetActive(false);
            }
        }
        activeModels.Clear();
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Handle added images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateModelTransform(trackedImage, true);
        }

        // Handle updated images
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                UpdateModelTransform(trackedImage, true);
            }
            else
            {
                UpdateModelTransform(trackedImage, false);
            }
        }

        // Handle removed images
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            UpdateModelTransform(trackedImage, false);
        }
    }

    private void UpdateModelTransform(ARTrackedImage trackedImage, bool show)
    {
        string imageName = trackedImage.referenceImage.name;
        ModelInfo info = System.Array.Find(modelInfos, x => x.name == imageName);
        
        if (info?.prefab == null) return;

        // Toggle visibility
        info.prefab.SetActive(show);
        
        if (show)
        {
            // Store the original rotation of the prefab
            Quaternion originalRotation = info.prefab.transform.rotation;
            
            // Update position to be above the tracked image
            Vector3 imageCenter = trackedImage.transform.position;
            Vector3 imageUp = trackedImage.transform.up;
            
            // Position the model above the image
            info.prefab.transform.position = imageCenter + (imageUp * 0.1f); // Adjust 0.1f as needed
            
            // Maintain the prefab's original rotation
            info.prefab.transform.rotation = originalRotation;
            
            activeModels[imageName] = info.prefab;
            Debug.Log($"[AR_TRACKING] Updated model for {imageName} - Position: {info.prefab.transform.position}, Rotation: {info.prefab.transform.rotation.eulerAngles}");
        }
        else
        {
            activeModels.Remove(imageName);
            Debug.Log($"[AR_TRACKING] Hiding model for {imageName}");
        }
    }
}