using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine.Android;

public class ImageTracking : MonoBehaviour
{
    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _OpenMapsWithAddress(string address, double latitude, double longitude);

    [DllImport("__Internal")]
    private static extern void _OpenURL(string url);

    [DllImport("__Internal")]
    private static extern void _ShowShareSheet(string text);
    #endif

    [System.Serializable]
    public class ModelInfo
    {
        public string modelName;
        public string title;
        public string description;
        public LocationData location;
        public string websiteUrl;
        public AudioClip voiceOver;
    }

    [System.Serializable]
    public class LocationData
    {
        public string address;
        public double latitude;
        public double longitude;
        public string googlePlaceId;
    }

    [SerializeField]
    private GameObject[] modelPrefabs;
    [SerializeField]
    private GameObject showInfoPanel;
    [SerializeField]
    private GameObject hiddenPanel;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private ModelInfo[] modelInfos;
    [SerializeField]
    private GameObject mapButton;
    [SerializeField]
    private GameObject shareButton;
    [SerializeField]
    private GameObject urlButton;
    [SerializeField]
    private AudioSource infoAudioSource;
    private Dictionary<string, GameObject> spawnedModels = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> rotatedModels = new Dictionary<string, bool>();
    private ARTrackedImageManager trackedImageManager;
    private int currentModelIndex = -1;
    [SerializeField] private bool useAndroidOptimizations = true;
    private const int TARGET_FRAME_RATE = 60;
    private const float LOD_BIAS = 0.75f; // Reduce texture quality

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        Debug.Log("[AR_SETUP] ARTrackedImageManager found: " + (trackedImageManager != null));
        
        #if UNITY_ANDROID
            // Disable AR components until we have permission
            if (trackedImageManager != null)
            {
                trackedImageManager.enabled = false;
            }
            StartCoroutine(CheckAndRequestCameraPermission());
        #endif
        
        // Initialize panels
        showInfoPanel?.SetActive(false);
        hiddenPanel?.SetActive(false);
        
        // Add click listener to the showInfoPanel
        if (showInfoPanel != null)
        {
            Button button = showInfoPanel.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnShowInfoPanelClicked);
            }
            else
            {
                Debug.LogError("[AR_SETUP] No Button component found on showInfoPanel");
            }
        }
        
        // Log prefabs configuration
        if (modelPrefabs != null)
        {
            Debug.Log($"[AR_SETUP] Number of model prefabs: {modelPrefabs.Length}");
            for (int i = 0; i < modelPrefabs.Length; i++)
            {
                Debug.Log($"[AR_SETUP] Prefab {i}: {(modelPrefabs[i] != null ? modelPrefabs[i].name : "null")}");
            }
        }
        else
        {
            Debug.LogError("[AR_SETUP] Model prefabs array is null!");
        }

        SetupButtonListeners();

        // Reset AR Session on startup
        ResetARSession();
    }

    private void OnEnable()
    {
        #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Debug.LogWarning("[AR_SETUP] Camera permission is not granted!");
                return;
            }
        #endif

        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("[AR_SETUP] Tracked images changed event handler registered");

        // Subscribe to AR session state changed event
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        showInfoPanel?.SetActive(false);
        hiddenPanel?.SetActive(false);
        Debug.Log("[AR_SETUP] Tracked images changed event handler unregistered");
        
        // Clean up the click listener
        if (showInfoPanel != null)
        {
            Button button = showInfoPanel.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(OnShowInfoPanelClicked);
            }
        }

        // Unsubscribe from AR session state changed event
        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            Debug.Log($"[AR_TRACKING] New image detected: {trackedImage.referenceImage.name}");
            
            // Find the matching prefab index
            int prefabIndex = -1;
            for (int i = 0; i < modelPrefabs.Length; i++)
            {
                if (modelPrefabs[i].name == trackedImage.referenceImage.name)
                {
                    prefabIndex = i;
                    Debug.Log($"[AR_TRACKING] Found matching prefab at index {i}: {modelPrefabs[i].name}");
                    break;
                }
            }

            if (prefabIndex >= 0)
            {
                GameObject prefab = modelPrefabs[prefabIndex];
                Vector3 position = trackedImage.transform.position;
                GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);
                
                // Add the TouchInteractionModel component if it doesn't exist
                if (spawnedObject.GetComponent<TouchInteractionModel>() == null)
                {
                    spawnedObject.AddComponent<TouchInteractionModel>();
                }
                
                spawnedModels.Add(trackedImage.referenceImage.name, spawnedObject);
                
                // Show and update info panel for newly detected image
                if (showInfoPanel != null)
                {
                    showInfoPanel.SetActive(true);
                    currentModelIndex = prefabIndex;
                    UpdateInfoPanelText(prefabIndex);
                    Debug.Log($"[AR_UI] Showing info panel for new model: {trackedImage.referenceImage.name}");
                }
                
                Debug.Log($"[AR_TRACKING] Spawned model: {spawnedObject.name} at position: {position}");
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateTrackingImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            Debug.Log($"[AR_TRACKING] Image removed: {trackedImage.referenceImage.name}");
            if (spawnedModels.TryGetValue(trackedImage.referenceImage.name, out GameObject obj))
            {
                Destroy(obj);
                spawnedModels.Remove(trackedImage.referenceImage.name);
                
                // Hide info panel when image is removed
                if (showInfoPanel != null)
                {
                    showInfoPanel.SetActive(false);
                    Debug.Log("[AR_UI] Hiding info panel due to removed image");
                }
            }
        }
    }

    private void UpdateTrackingImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        
        // Find the matching prefab index for updating info panel
        int prefabIndex = -1;
        for (int i = 0; i < modelPrefabs.Length; i++)
        {
            if (modelPrefabs[i].name == imageName)
            {
                prefabIndex = i;
                break;
            }
        }
        
        if (spawnedModels.TryGetValue(imageName, out GameObject spawnedObject))
        {
            // Update existing model
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log($"[AR_TRACKING] Updating tracked image: {imageName}, State: {trackedImage.trackingState}");
                
                // Show model and update transform
                spawnedObject.SetActive(true);
                UpdateModelTransform(spawnedObject, trackedImage);
                
                // Show info panel and update content
                if (showInfoPanel != null)
                {
                    showInfoPanel.SetActive(true);
                    if (prefabIndex >= 0)
                    {
                        currentModelIndex = prefabIndex;
                        UpdateInfoPanelText(prefabIndex);
                    }
                    Debug.Log($"[AR_UI] Showing info panel for model: {imageName}");
                }
            }
            else
            {
                Debug.Log($"[AR_TRACKING] Image lost tracking: {imageName}, State: {trackedImage.trackingState}");
                spawnedObject.SetActive(false);
                
                // Hide info panel when tracking is lost
                if (showInfoPanel != null)
                {
                    showInfoPanel.SetActive(false);
                    Debug.Log("[AR_UI] Hiding info panel due to lost tracking");
                }
            }
        }
    }

    private void UpdateModelTransform(GameObject model, ARTrackedImage trackedImage)
    {
        var touchInteraction = model.GetComponent<TouchInteractionModel>();
        if (touchInteraction != null)
        {
            touchInteraction.UpdateFromTracking(new Pose(trackedImage.transform.position, trackedImage.transform.rotation));
        }
        else
        {
            // Fallback for models without TouchInteractionModel
            model.transform.position = trackedImage.transform.position;
            model.transform.rotation = trackedImage.transform.rotation;
        }
    }

    private IEnumerator EnableWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(true);
            // Show info panel when model becomes visible
            showInfoPanel?.SetActive(true);
            Debug.Log($"[AR_UPDATE] Enabled object: {obj.name} and showing info panel");
        }
    }

    private IEnumerator DisableWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(false);
            // Hide info panel when model becomes invisible
            showInfoPanel?.SetActive(false);
            Debug.Log($"[AR_UPDATE] Disabled object: {obj.name} and hiding info panel");
        }
    }

    private void UpdateInfoPanelText(int modelIndex)
    {
        if (modelIndex >= 0 && modelIndex < modelInfos.Length)
        {
            ModelInfo info = modelInfos[modelIndex];
            if (titleText != null)
            {
                titleText.text = info.title;
                Debug.Log($"[AR_UI] Updated title to: {info.title}");
            }
            if (descriptionText != null)
            {
                descriptionText.text = info.description;
                Debug.Log($"[AR_UI] Updated description to: {info.description}");
            }
            
            if (infoAudioSource != null)
            {
                infoAudioSource.Stop();
                infoAudioSource.clip = info.voiceOver;
                
                if (info.voiceOver != null)
                {
                    infoAudioSource.Play();
                    Debug.Log($"[AR_AUDIO] Playing audio for model: {info.modelName}");
                }
                else
                {
                    Debug.LogWarning($"[AR_AUDIO] No audio clip assigned for model: {info.modelName}");
                }
            }
            else
            {
                Debug.LogError("[AR_AUDIO] AudioSource reference is missing!");
            }
        }
        else
        {
            Debug.LogError($"[AR_AUDIO] Invalid model index: {modelIndex}");
        }
    }

    public void OnShowInfoPanelClicked()
    {
        Debug.Log("[AR_UI] Show Info Panel clicked");
        hiddenPanel?.SetActive(true);
    }

    public void CloseHiddenPanel()
    {
        hiddenPanel?.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        Debug.Log("[AR_BUTTONS] Starting button listener setup");

        // Map Button Setup
        if (mapButton != null)
        {
            Button mapBtnComponent = mapButton.GetComponent<Button>();
            if (mapBtnComponent != null)
            {
                Debug.Log("[AR_BUTTONS] Map button component found, adding listener");
                mapBtnComponent.onClick.RemoveAllListeners(); // Clear any existing listeners
                mapBtnComponent.onClick.AddListener(() => {
                    Debug.Log("[AR_BUTTONS] Map button clicked");
                    if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
                    {
                        Debug.Log($"[AR_BUTTONS] Opening directions for model: {modelInfos[currentModelIndex].modelName}");
                        OpenDirections(modelInfos[currentModelIndex].location);
                    }
                    else
                    {
                        Debug.LogWarning("[AR_BUTTONS] Cannot open directions - no model selected");
                    }
                });
            }
            else
            {
                Debug.LogError("[AR_BUTTONS] No Button component found on mapButton GameObject!");
            }
        }
        else
        {
            Debug.LogError("[AR_BUTTONS] Map button reference is null!");
        }

        // Share Button Setup
        if (shareButton != null)
        {
            Button shareBtnComponent = shareButton.GetComponent<Button>();
            if (shareBtnComponent != null)
            {
                Debug.Log("[AR_BUTTONS] Share button component found, adding listener");
                shareBtnComponent.onClick.RemoveAllListeners(); // Clear any existing listeners
                shareBtnComponent.onClick.AddListener(() => {
                    Debug.Log("[AR_BUTTONS] Share button clicked");
                    if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
                    {
                        Debug.Log($"[AR_BUTTONS] Sharing info for model: {modelInfos[currentModelIndex].modelName}");
                        ShareInfo();
                    }
                    else
                    {
                        Debug.LogWarning("[AR_BUTTONS] Cannot share - no model selected");
                    }
                });
            }
            else
            {
                Debug.LogError("[AR_BUTTONS] No Button component found on shareButton GameObject!");
            }
        }
        else
        {
            Debug.LogError("[AR_BUTTONS] Share button reference is null!");
        }

        // URL Button Setup
        if (urlButton != null)
        {
            Button urlBtnComponent = urlButton.GetComponent<Button>();
            if (urlBtnComponent != null)
            {
                Debug.Log("[AR_BUTTONS] URL button component found, adding listener");
                urlBtnComponent.onClick.RemoveAllListeners(); // Clear any existing listeners
                urlBtnComponent.onClick.AddListener(() => {
                    Debug.Log("[AR_BUTTONS] URL button clicked");
                    if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
                    {
                        Debug.Log($"[AR_BUTTONS] Opening URL for model: {modelInfos[currentModelIndex].modelName}");
                        OpenWebURL(modelInfos[currentModelIndex].websiteUrl);
                    }
                    else
                    {
                        Debug.LogWarning("[AR_BUTTONS] Cannot open URL - no model selected");
                    }
                });
            }
            else
            {
                Debug.LogError("[AR_BUTTONS] No Button component found on urlButton GameObject!");
            }
        }
        else
        {
            Debug.LogError("[AR_BUTTONS] URL button reference is null!");
        }
    }

    public void ShareInfo()
    {
        Debug.Log("[AR_BUTTONS] Starting share process");
        
        if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
        {
            ModelInfo currentModel = modelInfos[currentModelIndex];
            string textToShare = $"Check out {currentModel.modelName}!";
            
            Debug.Log($"[AR_BUTTONS] Sharing text: {textToShare}");
            
            #if UNITY_IOS
                Debug.Log("[AR_BUTTONS] Using iOS share method");
                _ShowShareSheet(textToShare);
            #elif UNITY_ANDROID
                Debug.Log("[AR_BUTTONS] Using Android share method");
                AndroidShare(textToShare);
            #endif
        }
    }

    private void AndroidShare(string text)
    {
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
        
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
        currentActivity.Call("startActivity", jChooser);
    }

    public void OpenDirections(LocationData location)
    {
        Debug.Log($"[AR_BUTTONS] Opening directions for location: {location.latitude}, {location.longitude}");
        
        #if UNITY_IOS
            Debug.Log("[AR_BUTTONS] Platform: iOS");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log("[AR_BUTTONS] Opening Apple Maps...");
                try
                {
                    URLHandler.OpenMaps($"{location.latitude},{location.longitude}");
                    Debug.Log("[AR_BUTTONS] Successfully called OpenMaps");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[AR_BUTTONS] Error opening maps: {e.Message}");
                    Debug.LogError($"[AR_BUTTONS] Stack trace: {e.StackTrace}");
                }
            }
            else
            {
                Debug.LogWarning("[AR_BUTTONS] Not running on iPhone device");
            }
        #elif UNITY_ANDROID
            Debug.Log("[AR_BUTTONS] Platform: Android");
            try
            {
                // Try to open in Google Maps app first
                string googleMapsApp = $"google.navigation:q={location.latitude},{location.longitude}";
                Debug.Log($"[AR_BUTTONS] Trying to open Google Maps app: {googleMapsApp}");
                Application.OpenURL(googleMapsApp);

                // Fallback to browser if Google Maps app is not installed
                string browserUrl = $"https://www.google.com/maps/dir/?api=1&destination={location.latitude},{location.longitude}";
                if (!string.IsNullOrEmpty(location.googlePlaceId))
                {
                    browserUrl += $"&destination_place_id={location.googlePlaceId}";
                }
                Debug.Log($"[AR_BUTTONS] Fallback URL: {browserUrl}");
                Application.OpenURL(browserUrl);
                
                Debug.Log("[AR_BUTTONS] Successfully opened maps URL");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AR_BUTTONS] Error opening maps: {e.Message}");
                Debug.LogError($"[AR_BUTTONS] Stack trace: {e.StackTrace}");
            }
        #endif
    }

    void Start()
    {
        #if UNITY_ANDROID
        if (useAndroidOptimizations)
        {
            ApplyAndroidOptimizations();
        }

        // Disable AR components initially
        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = false;
        }
        
        // Request camera permission first
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        
        // Start coroutine to initialize AR after permissions
        StartCoroutine(InitializeARComponents());
        #endif

        // Verify button references and setup listeners
        // ... rest of existing Start() code ...
    }

    private IEnumerator InitializeARComponents()
    {
        #if UNITY_ANDROID
        // Wait for permission response
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("[AR_SETUP] Waiting for camera permission...");
            yield return new WaitForSeconds(0.5f);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("[AR_SETUP] Camera permission granted, initializing AR");
            
            // Wait a frame to ensure everything is ready
            yield return null;
            
            // Initialize AR components
            if (trackedImageManager != null)
            {
                trackedImageManager.enabled = true;
            }
            
            // Reset AR Session after initialization
            ResetARSession();
        }
        else
        {
            Debug.LogError("[AR_SETUP] Camera permission denied!");
        }
        #endif
    }

    public void OnMapButtonClicked()
    {
        Debug.Log("[AR_BUTTONS] Map button clicked");
        if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
        {
            Debug.Log($"[AR_BUTTONS] Opening directions for model: {modelInfos[currentModelIndex].modelName}");
            OpenDirections(modelInfos[currentModelIndex].location);
        }
        else
        {
            Debug.LogWarning("[AR_BUTTONS] Cannot open directions - no model selected");
        }
    }

    public void OpenWebURL(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("[AR_BUTTONS] URL is empty or null");
            return;
        }

        Debug.Log($"[AR_BUTTONS] Opening URL: {url}");
        
        try
        {
            #if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                _OpenURL(url);
            }
            #elif UNITY_ANDROID
            Application.OpenURL(url);
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AR_BUTTONS] Error opening URL: {e.Message}");
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("[AR_SESSION] Application gained focus, resetting AR session");
            ResetARSession();
            
            #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Debug.Log("[AR_SETUP] Requesting camera permission after focus");
                    Permission.RequestUserPermission(Permission.Camera);
                }
            #endif
        }
    }

    private IEnumerator CheckAndRequestCameraPermission()
    {
        #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(0.1f);
                
                while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Debug.Log("[AR_SETUP] Waiting for camera permission...");
                    yield return new WaitForSeconds(0.5f);
                }
            }
            
            if (Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Debug.Log("[AR_SETUP] Camera permission granted");
                // Initialize AR components here
                if (trackedImageManager != null)
                {
                    trackedImageManager.enabled = true;
                }
            }
            else
            {
                Debug.LogError("[AR_SETUP] Camera permission denied!");
            }
        #else
            yield return null;
        #endif
    }

    public void OnURLButtonClicked()
    {
        Debug.Log("[AR_BUTTONS] URL button clicked");
        if (currentModelIndex >= 0 && currentModelIndex < modelInfos.Length)
        {
            Debug.Log($"[AR_BUTTONS] Opening URL for model: {modelInfos[currentModelIndex].modelName}");
            OpenWebURL(modelInfos[currentModelIndex].websiteUrl);
        }
        else
        {
            Debug.LogWarning("[AR_BUTTONS] Cannot open URL - no model selected");
        }
    }

    private void ApplyAndroidOptimizations()
    {
        // Frame rate and VSync
        Application.targetFrameRate = TARGET_FRAME_RATE;
        QualitySettings.vSyncCount = 0;

        // Reduce texture quality
        QualitySettings.masterTextureLimit = 1; // Use half-resolution textures
        QualitySettings.globalTextureMipmapLimit = 1;
        QualitySettings.lodBias = LOD_BIAS;

        // Disable anti-aliasing for performance
        QualitySettings.antiAliasing = 0;

        // Reduce shadow quality
        QualitySettings.shadowResolution = ShadowResolution.Low;
        QualitySettings.shadowDistance = 15f;

        // Optimize for mobile
        QualitySettings.skinWeights = SkinWeights.TwoBones;
        QualitySettings.asyncUploadTimeSlice = 2;
        QualitySettings.asyncUploadBufferSize = 16;

        Debug.Log("[AR_PERFORMANCE] Android optimizations applied");
    }

    private void OptimizeModelForAndroid(GameObject model)
    {
        // Optimize meshes
        MeshRenderer[] meshRenderers = model.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.receiveShadows = false; // Disable shadow receiving
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Disable shadow casting
            
            // Optimize materials
            foreach (Material mat in renderer.materials)
            {
                mat.enableInstancing = true; // Enable GPU instancing
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }

        // Optimize animations if present
        Animator animator = model.GetComponent<Animator>();
        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.CullCompletely;
            animator.updateMode = AnimatorUpdateMode.Normal;
        }

        // Disable unnecessary components when not visible
        MonoBehaviour[] components = model.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour comp in components)
        {
            if (comp != null)
            {
                comp.enabled = false;
            }
        }

        Debug.Log($"[AR_PERFORMANCE] Optimized model: {model.name}");
    }

    // Add this method to handle model visibility
    private void SetModelVisibility(GameObject model, bool visible)
    {
        if (model == null) return;

        #if UNITY_ANDROID
        if (useAndroidOptimizations)
        {
            // Enable/disable components based on visibility
            MonoBehaviour[] components = model.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour comp in components)
            {
                if (comp != null && comp.enabled != visible)
                {
                    comp.enabled = visible;
                }
            }
        }
        #endif

        model.SetActive(visible);
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Debug.Log($"[AR_SESSION] AR Session state changed to: {args.state}");
        if (args.state == ARSessionState.SessionInitializing)
        {
            // Clear tracked images when session is initializing
            ClearTrackedImages();
        }
    }

    private void ResetARSession()
    {
        Debug.Log("[AR_SESSION] Resetting AR Session");
        
        // Clear all spawned models
        foreach (var model in spawnedModels.Values)
        {
            if (model != null)
            {
                Destroy(model);
            }
        }
        spawnedModels.Clear();

        // Reset the AR Session
        if (ARSession.state > ARSessionState.None)
        {
            Debug.Log("[AR_SESSION] Resetting AR tracking");
            var arSession = FindObjectOfType<ARSession>();
            if (arSession != null)
            {
                arSession.Reset();
            }

            // Reset the tracked image manager
            if (trackedImageManager != null)
            {
                trackedImageManager.enabled = false;
                trackedImageManager.enabled = true;
            }
        }
    }

    private void ClearTrackedImages()
    {
        Debug.Log("[AR_SESSION] Clearing tracked images");
        
        // Clear existing tracked images
        if (trackedImageManager != null && trackedImageManager.trackables != null)
        {
            foreach (var trackable in trackedImageManager.trackables)
            {
                if (spawnedModels.ContainsKey(trackable.referenceImage.name))
                {
                    var model = spawnedModels[trackable.referenceImage.name];
                    if (model != null)
                    {
                        Destroy(model);
                    }
                    spawnedModels.Remove(trackable.referenceImage.name);
                }
            }
        }
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Application is resuming
        {
            Debug.Log("[AR_SESSION] Application resuming, resetting AR session");
            ResetARSession();
        }
    }

    public void OpenMaps(string address, double latitude, double longitude)
    {
        try
        {
            #if UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Debug.Log($"[AR_MAPS] Opening Maps for iOS: {address} ({latitude}, {longitude})");
                _OpenMapsWithAddress(address, latitude, longitude);
            }
            #elif UNITY_ANDROID
            // Android implementation
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AR_MAPS] Error opening maps: {e.Message}");
        }
    }
}