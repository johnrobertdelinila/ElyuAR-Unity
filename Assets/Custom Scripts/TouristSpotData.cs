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
}