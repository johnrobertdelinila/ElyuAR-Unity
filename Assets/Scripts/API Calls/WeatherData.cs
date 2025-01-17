using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherData : MonoBehaviour
{
    public GetLocation getLocation;
    private float latitude;
    private float longitude;
    private bool locationInitialized;
    private float timer;
    public float minutesBetweenUpdate;
    public WeatherInfo Info;

    public TextMeshProUGUI VisualLocation;
    public TextMeshProUGUI VisualTemp;
    public TextMeshProUGUI VisualWeather;

    public string API_key;
    public void Begin()
    {
        latitude = getLocation.latitude;
        longitude = getLocation.longitude;
        Debug.Log(latitude + " " + longitude);
        locationInitialized = true;
    }

    private void Update()
    {
        if (locationInitialized)
        {
            if (timer <= 0)
            {
                StartCoroutine(GetWeatherInfo());
                timer = minutesBetweenUpdate * 60;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        VisualLocation.text = getLocation.Info.city + ", " + Info.sys.country;
        VisualTemp.text = KelvinToCelsius(Info.main.temp).ToString().Substring(0, 4) + " C";
        VisualWeather.text = Info.weather[0].main;

    }

    private IEnumerator GetWeatherInfo()
    {
        var www = new UnityWebRequest("https://api.openweathermap.org/data/2.5/weather?lat=" + 
            latitude + "&lon=" + longitude + "&appid=" + API_key)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }

        Info = JsonUtility.FromJson<WeatherInfo>(www.downloadHandler.text);
    }

    public static double KelvinToCelsius(double kelvin)
    {
        return kelvin - 273.15;
    }
}
[Serializable]
public class WeatherInfo
{
    public Coord coord;
    public List<Weather> weather;
    public string @base;
    public Main main;
    public int visibility;
    public Wind wind;
    public Rain rain;
    public Clouds clouds;
    public int dt;
    public Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;
}

[Serializable]
public class Coord
{
    public float lon;
    public float lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class Main
{
    public float temp;
    public float feels_like;
    public float temp_min;
    public float temp_max;
    public int pressure;
    public int humidity;
}

[Serializable]
public class Wind
{
    public float speed;
    public int deg;
    public float gust;
}

[Serializable]
public class Rain
{
    public float r1h;
}

[Serializable]
public class Clouds
{
    public int all;
}

[Serializable]
public class Sys
{
    public int type;
    public int id;
    public string country;
    public int sunrise;
    public int sunset;
}

