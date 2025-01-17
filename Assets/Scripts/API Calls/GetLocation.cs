using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class GetLocation : MonoBehaviour
{
    private string IPAddress;
    public LocationInfo Info;
    public float latitude;
    public float longitude;
    public WeatherData weatherData;

    public Ip ip;
    void Start()
    {
        StartCoroutine(GetIP());
    }

    private IEnumerator GetIP()
    {
        var www = new UnityWebRequest("https://api.ipify.org?format=json")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }

        ip = JsonUtility.FromJson<Ip>(www.downloadHandler.text);
        IPAddress = ip.ip;
        Debug.Log(IPAddress);
        StartCoroutine(GetCoordinates());
    }

    private IEnumerator GetCoordinates()
    {
        var www = new UnityWebRequest("http://ip-api.com/json/" + IPAddress)
        {
            downloadHandler = new DownloadHandlerBuffer(),
        };

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }

        Info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
        latitude = Info.lat;
        longitude = Info.lon;
        weatherData.Begin();
    }
}

[Serializable]
public class LocationInfo
{
    public string status;
    public string country;
    public string countryCode;
    public string region;
    public string regionName;
    public string city;
    public string zip;
    public float lat;
    public float lon;
    public string timezone;
    public string isp;
    public string org;
    public string @as;
    public string query;
}

[Serializable]
public class Ip
{
    public string ip;
}
