// TouristSpotInfoManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class TouristSpotInfoManager : MonoBehaviour
{
    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _OpenMapsWithAddress(double latitude, double longitude);
    #endif

    [SerializeField] private TouristSpotData[] touristSpots;
    
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI addressText;
    [SerializeField] private TextMeshProUGUI classificationText;
    [SerializeField] private TextMeshProUGUI operationalHoursText;
    [SerializeField] private TextMeshProUGUI environmentalFeesText;
    [SerializeField] private TextMeshProUGUI transportationOptionsText;
    
    [Header("UI Image References")]
    [SerializeField] private Image spotImage;
    [SerializeField] private GoogleMapSnapshot mapSnapshot;
    
    private TouristSpotData currentSpotData;

    void Start()
    {
        string selectedSpot = SceneDataManager.GetSelectedSpot();
        UpdateInfo(selectedSpot);
        
        // Add click handler to map image
        if (mapSnapshot != null && mapSnapshot.GetComponent<Image>() != null)
        {
            Button mapButton = mapSnapshot.gameObject.GetComponent<Button>();
            if (mapButton == null)
            {
                mapButton = mapSnapshot.gameObject.AddComponent<Button>();
            }
            mapButton.onClick.AddListener(OnMapClicked);
        }
    }

    private void UpdateInfo(string spotName)
    {
        TouristSpotData spotData = System.Array.Find(touristSpots, spot => spot.spotName == spotName);
        
        if (spotData != null)
        {
            currentSpotData = spotData;  // Store the current spot data
            
            // Update all text fields
            titleText.text = spotData.title;
            descriptionText.text = spotData.description;
            addressText.text = spotData.address;
            classificationText.text = spotData.classification;
            operationalHoursText.text = spotData.operationalHours;
            environmentalFeesText.text = spotData.environmentalFee;
            transportationOptionsText.text = spotData.transportationOptions;
            
            // Update image
            spotImage.sprite = spotData.image;
            
            // Update map snapshot
            if (mapSnapshot != null)
            {
                mapSnapshot.UpdateLocation(spotData.latitude, spotData.longitude);
                Debug.Log($"Updating map location: {spotData.latitude}, {spotData.longitude}");
            }
            else
            {
                Debug.LogError("Map Snapshot reference is missing!");
            }
        }
        else
        {
            Debug.LogError($"Tourist spot data not found for: {spotName}");
        }
    }

    private void OnMapClicked()
    {
        if (currentSpotData == null) return;

        #if UNITY_IOS
            _OpenMapsWithAddress(currentSpotData.latitude, currentSpotData.longitude);
        #elif UNITY_ANDROID
            string uri = $"geo:{currentSpotData.latitude},{currentSpotData.longitude}?q={currentSpotData.latitude},{currentSpotData.longitude}";
            try
            {
                Application.OpenURL(uri);
                Debug.Log($"Opening map at {currentSpotData.latitude}, {currentSpotData.longitude}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error opening map: {e.Message}");
                // Fallback to web browser if no map app is installed
                Application.OpenURL($"https://www.google.com/maps/search/?api=1&query={currentSpotData.latitude},{currentSpotData.longitude}");
            }
        #endif
    }

    public void OnLocationSelected()
    {
        string locationName = SceneDataManager.GetSelectedSpot();
        Debug.Log($"[TOURISTSPOTINFO] Selected location: {locationName}");
        SceneManager.LoadScene("IndividualARScene");  // Your AR scene name
    }
}