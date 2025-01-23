// TouristSpotInfoManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TouristSpotInfoManager : MonoBehaviour
{
    [SerializeField] private TouristSpotData[] touristSpots;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI addressText;
    [SerializeField] private Image spotImage;
    [SerializeField] private GoogleMapSnapshot mapSnapshot;

    void Start()
    {
        string selectedSpot = SceneDataManager.GetSelectedSpot();
        UpdateInfo(selectedSpot);
    }

    private void UpdateInfo(string spotName)
    {
        TouristSpotData spotData = System.Array.Find(touristSpots, spot => spot.spotName == spotName);
        
        if (spotData != null)
        {
            titleText.text = spotData.title;
            descriptionText.text = spotData.description;
            addressText.text = spotData.address;
            spotImage.sprite = spotData.image;
            
            // Update map snapshot with the location
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
}