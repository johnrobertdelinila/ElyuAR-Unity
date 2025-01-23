using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class GoogleMapSnapshot : MonoBehaviour 
{
    [SerializeField] private string apiKey; // Your Google Maps API key
    [SerializeField] private double latitude;
    [SerializeField] private double longitude;
    [SerializeField] private int zoom = 15;
    [SerializeField] private int width = 640;
    [SerializeField] private int height = 480;
    [SerializeField] private Image targetImage;

    void Start()
    {
        StartCoroutine(LoadMapSnapshot());
    }

    IEnumerator LoadMapSnapshot()
    {
        // Add error checking for coordinates
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Key not set!");
            yield break;
        }

        string url = $"https://maps.googleapis.com/maps/api/staticmap?" +
            $"center={latitude},{longitude}&" +
            $"zoom={zoom}&" +
            $"size={width}x{height}&" +
            $"maptype=roadmap&" +
            $"markers=color:red%7C{latitude},{longitude}&" +
            $"key={apiKey}";

        Debug.Log($"Requesting map with URL: {url}"); // For debugging

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            targetImage.sprite = Sprite.Create(texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogError($"Map snapshot failed: {request.error}\nResponse: {request.downloadHandler.text}");
        }
    }

    public void UpdateLocation(double newLatitude, double newLongitude)
    {
        latitude = newLatitude;
        longitude = newLongitude;
        StartCoroutine(LoadMapSnapshot());
    }
}

