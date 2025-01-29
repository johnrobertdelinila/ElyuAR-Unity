using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

public class ImageTracking : MonoBehaviour
{
    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _OpenMapsWithAddress(double latitude, double longitude);
    [DllImport("__Internal")]
    private static extern void _ShowShareSheet(string text);
    [DllImport("__Internal")]
    private static extern void _OpenURL(string urlStr);
    #endif

    [SerializeField]
    private ModelInfo[] modelInfos;
    
    [Header("UI References")]
    [SerializeField]
    private GameObject hiddenInfoPanel;
    [SerializeField]
    private GameObject showPanelButton;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private AudioSource audioSource;
    
    [Header("Info Panel Buttons")]
    [SerializeField]
    private GameObject mapButton;
    [SerializeField]
    private GameObject urlButton;
    [SerializeField]
    private GameObject shareButton;

    private Dictionary<string, GameObject> activeModels = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;
    private ModelInfo currentModelInfo;
    private bool isOlderDevice = false;

    private void Awake()
    {
        Debug.Log("[AR_INIT] Starting AR initialization");
        try
        {
            // Check device model on iOS
            #if UNITY_IOS
            string deviceModel = UnityEngine.iOS.Device.generation.ToString();
            isOlderDevice = deviceModel.Contains("iPhone13") || 
                           (deviceModel.Contains("iPhone") && 
                            int.Parse(deviceModel.Replace("iPhone", "")) < 13);
            Debug.Log($"[AR_INIT] Device Model: {deviceModel}, Optimizations Enabled: {isOlderDevice}");
            #endif

            // Initialize with optimized settings for older devices
            trackedImageManager = GetComponent<ARTrackedImageManager>();
            if (trackedImageManager != null)
            {
                if (isOlderDevice)
                {
                    // Reduce tracking quality for better performance
                    var config = trackedImageManager.currentConfiguration;
                    if (config != null)
                    {
                        config.maxNumberOfMovingImages = 1; // Limit concurrent tracking
                        trackedImageManager.currentConfiguration = config;
                        Debug.Log("[AR_INIT] Applied optimization settings for older device");
                    }
                }
            }
            
            // Rest of your initialization code...
            InitializeModels();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AR_INIT] Error during initialization: {e.Message}");
        }
    }

    private void InitializeModels()
    {
        // Separate model initialization for better memory management
        foreach (var modelInfo in modelInfos)
        {
            if (modelInfo == null || modelInfo.prefab == null) continue;

            if (isOlderDevice)
            {
                // Optimize meshes for older devices
                OptimizeMesh(modelInfo.prefab);
            }
            
            modelInfo.prefab.SetActive(false);
            Debug.Log($"[AR_INIT] Initialized model: {modelInfo.name}");
        }
    }

    private void OptimizeMesh(GameObject prefab)
    {
        // Optimize mesh renderers
        var renderers = prefab.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.receiveShadows = false;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // Reduce texture quality if needed
        var materials = prefab.GetComponentsInChildren<Material>();
        foreach (var material in materials)
        {
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }
    }

    private void Start()
    {
        Debug.Log("[AR_CONFIG] Checking AR configuration");
        if (trackedImageManager != null)
        {
            var config = trackedImageManager.currentConfiguration;
            if (config != null)
            {
                Debug.Log($"[AR_CONFIG] Max tracked images: {config.maxNumberOfMovingImages}");
                Debug.Log($"[AR_CONFIG] Reference image count: {config.referenceImages.count}");
            }
            else
            {
                Debug.LogError("[AR_CONFIG] No configuration found!");
            }
        }
    }

    private void OnEnable()
    {
        Debug.Log("[AR_LIFECYCLE] OnEnable called");
        try
        {
            if (trackedImageManager != null)
            {
                trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
                Debug.Log("[AR_LIFECYCLE] Successfully subscribed to trackedImagesChanged");
            }
            else
            {
                Debug.LogError("[AR_LIFECYCLE] trackedImageManager is null in OnEnable");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AR_LIFECYCLE] Error in OnEnable: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    private void OnDisable()
    {
        // Clean up resources
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
        
        foreach (var model in activeModels.Values)
        {
            if (model != null)
            {
                model.SetActive(false);
            }
        }
        activeModels.Clear();
        
        // Force garbage collection when scene is disabled
        if (isOlderDevice)
        {
            System.GC.Collect();
        }
        
        // Hide UI
        HideUI();
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        try
        {
            if (isOlderDevice)
            {
                // Handle only one image at a time for older devices
                if (eventArgs.added.Count > 0)
                {
                    UpdateModelTransform(eventArgs.added[0], true);
                }
                else if (eventArgs.updated.Count > 0)
                {
                    var trackedImage = eventArgs.updated[0];
                    UpdateModelTransform(trackedImage, 
                        trackedImage.trackingState == TrackingState.Tracking);
                }
            }
            else
            {
                // Original handling for newer devices
                foreach (ARTrackedImage trackedImage in eventArgs.added)
                {
                    UpdateModelTransform(trackedImage, true);
                }

                // Handle updated images
                foreach (ARTrackedImage trackedImage in eventArgs.updated)
                {
                    Debug.Log($"[AR_TRACKING] Updated image: {trackedImage.referenceImage.name} - State: {trackedImage.trackingState}");
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
                    Debug.Log($"[AR_TRACKING] Removed image: {trackedImage.referenceImage.name}");
                    UpdateModelTransform(trackedImage, false);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AR_TRACKING] Error: {e.Message}");
        }
    }

    private void UpdateModelTransform(ARTrackedImage trackedImage, bool show)
    {
        string imageName = trackedImage.referenceImage.name;
        ModelInfo info = System.Array.Find(modelInfos, x => x.name == imageName);
        
        if (info?.prefab == null) return;

        if (isOlderDevice && activeModels.Count > 0 && !activeModels.ContainsKey(imageName))
        {
            // For older devices, deactivate other models before showing new one
            foreach (var model in activeModels.Values)
            {
                model.SetActive(false);
            }
            activeModels.Clear();
        }

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
            info.prefab.transform.position = imageCenter + (imageUp * 0.1f);
            
            // Maintain the prefab's original rotation
            info.prefab.transform.rotation = originalRotation;
            
            activeModels[imageName] = info.prefab;
            
            // Update UI without forcing panel to hide
            UpdateUI(info);
            
            Debug.Log($"[AR_TRACKING] Updated model for {imageName} - Position: {info.prefab.transform.position}, Rotation: {info.prefab.transform.rotation.eulerAngles}");
        }
        else
        {
            activeModels.Remove(imageName);
            // Only hide the show panel button, not the info panel
            showPanelButton.SetActive(false);
            Debug.Log($"[AR_TRACKING] Hiding model for {imageName}");
        }
    }

    private void UpdateUI(ModelInfo info)
    {
        // Check if this is a different model than the current one
        bool isNewModel = (currentModelInfo == null || currentModelInfo.name != info.name);
        
        currentModelInfo = info;
        
        // Update text fields
        titleText.text = info.title;
        descriptionText.text = info.description;
        
        // Prepare audio source
        audioSource.clip = info.audio;
        
        // Show the panel button
        showPanelButton.SetActive(true);
        
        // If the info panel is visible and this is a new model, play the audio
        if (hiddenInfoPanel.activeSelf && isNewModel && info.audio != null)
        {
            audioSource.Stop(); // Stop any currently playing audio
            audioSource.Play();
            Debug.Log($"[UI] Playing audio for new model: {info.name}");
        }
    }

    private void HideUI()
    {
        hiddenInfoPanel.SetActive(false);
        showPanelButton.SetActive(false);
        
        // Stop audio if playing
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        currentModelInfo = null;
    }

    public void OnShowPanelButtonClicked()
    {
        Debug.Log("[UI] Show Panel Button clicked");
        
        if (hiddenInfoPanel == null)
        {
            Debug.LogError("[UI] Hidden Info Panel reference is missing!");
            return;
        }
        
        if (showPanelButton == null)
        {
            Debug.LogError("[UI] Show Panel Button reference is missing!");
            return;
        }

        hiddenInfoPanel.SetActive(true);
        showPanelButton.SetActive(false);
        
        // Play audio if available
        if (currentModelInfo != null && currentModelInfo.audio != null)
        {
            if (audioSource == null)
            {
                Debug.LogError("[UI] Audio Source reference is missing!");
                return;
            }
            
            audioSource.clip = currentModelInfo.audio;
            audioSource.Play();
            Debug.Log("[UI] Playing audio for " + currentModelInfo.name);
        }
        else
        {
            Debug.Log("[UI] No audio to play or no current model info");
        }
    }

    public void OnMapButtonClicked()
    {
        Debug.Log("[UI] Map Button clicked");
        
        if (currentModelInfo == null)
        {
            Debug.LogError("[UI] No current model info available!");
            return;
        }

        double latitude = currentModelInfo.latitude;
        double longitude = currentModelInfo.longitude;

        #if UNITY_IOS
            // Use the native iOS implementation with just coordinates
            _OpenMapsWithAddress(latitude, longitude);
        #elif UNITY_ANDROID
            // Open Google Maps with just coordinates
            string uri = $"geo:{latitude},{longitude}?q={latitude},{longitude}";
            try
            {
                Application.OpenURL(uri);
                Debug.Log($"[UI] Opening map at {latitude}, {longitude}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[UI] Error opening map: {e.Message}");
                // Fallback to web browser if no map app is installed
                Application.OpenURL($"https://www.google.com/maps/search/?api=1&query={latitude},{longitude}");
            }
        #endif
    }

    public void OnShareButtonClicked()
    {
        Debug.Log("[UI] Share Button clicked");
        
        if (currentModelInfo == null)
        {
            Debug.LogError("[UI] No current model info available!");
            return;
        }

        // Create the share text
        string shareText = $"{currentModelInfo.title}\n\n{currentModelInfo.description}";

        #if UNITY_IOS
            // Use the native iOS implementation
            _ShowShareSheet(shareText);
        #elif UNITY_ANDROID
            // Use Android's native sharing
            using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
            using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
            {
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                intentObject.Call<AndroidJavaObject>("setType", "text/plain");
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
                    currentActivity.Call("startActivity", chooser);
                }
            }
        #endif
    }

    public void OnURLButtonClicked()
    {
        Debug.Log("[UI] URL Button clicked");
        
        if (currentModelInfo == null)
        {
            Debug.LogError("[UI] No current model info available!");
            return;
        }

        if (string.IsNullOrEmpty(currentModelInfo.websiteUrl))
        {
            Debug.LogError("[UI] No URL available for this model!");
            return;
        }

        #if UNITY_IOS
            // Use the native iOS implementation
            _OpenURL(currentModelInfo.websiteUrl);
        #elif UNITY_ANDROID
            // Use Android's built-in URL opening
            Application.OpenURL(currentModelInfo.websiteUrl);
        #endif
        
        Debug.Log($"[UI] Opening URL: {currentModelInfo.websiteUrl}");
    }

    public void OnInfoPanelClicked()
    {
        Debug.Log("[UI] Info Panel clicked - hiding panel");
        
        // Hide the info panel
        hiddenInfoPanel.SetActive(false);
        
        // Stop audio if playing
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("[UI] Stopped playing audio");
        }
    }
}