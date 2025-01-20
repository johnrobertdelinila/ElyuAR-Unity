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
    private static extern void _OpenMapsWithAddress(string address, double latitude, double longitude);
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

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        // Initially hide all models and UI
        foreach (var modelInfo in modelInfos)
        {
            if (modelInfo.prefab != null)
            {
                modelInfo.prefab.SetActive(false);
            }
        }
        
        hiddenInfoPanel.SetActive(false);
        showPanelButton.SetActive(false);
    }

    private void Start()
    {
        // Remove the onClick listener since we'll set it in the Inspector
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
        
        // Hide UI
        HideUI();
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
            // Only hide UI when model is actually hidden
            HideUI();
            Debug.Log($"[AR_TRACKING] Hiding model for {imageName}");
        }
    }

    private void UpdateUI(ModelInfo info)
    {
        currentModelInfo = info;
        
        // Update text fields
        titleText.text = info.title;
        descriptionText.text = info.description;
        
        // Prepare audio source
        audioSource.clip = info.audio;
        
        // Show the panel button
        showPanelButton.SetActive(true);
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
        string locationName = currentModelInfo.title;

        #if UNITY_IOS
            // Use the native iOS implementation
            _OpenMapsWithAddress(locationName, latitude, longitude);
        #elif UNITY_ANDROID
            // Open Google Maps
            string uri = $"geo:{latitude},{longitude}?q={latitude},{longitude}({WWW.EscapeURL(locationName)})";
            try
            {
                Application.OpenURL(uri);
                Debug.Log($"[UI] Opening map at {latitude}, {longitude} for {locationName}");
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