// TouristSpotData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "TouristSpot", menuName = "Tourist Spots/Spot Data")]
public class TouristSpotData : ScriptableObject
{
    public string spotName;
    public string title;
    public string description;
    public string address;
    public Sprite image;
    public double latitude;
    public double longitude;
    
    // New fields
    public string classification;    // e.g., "Natural Attraction", "Historical Site", "Cultural Landmark"
    public string operationalHours; // e.g., "Monday-Sunday: 8:00 AM - 5:00 PM"
    public string environmentalFee;  // e.g., "₱100 per person" or "Free"
    [TextArea(3, 5)]
    public string transportationOptions; // Different ways to reach the location
}