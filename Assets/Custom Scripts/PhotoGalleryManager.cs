// PhotoGalleryManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotoGalleryManager : MonoBehaviour
{
    public void OnSpotSelected(string spotName)
    {
        SceneDataManager.SetSelectedSpot(spotName);
        SceneManager.LoadScene("TouristSpotInfo");
    }
}