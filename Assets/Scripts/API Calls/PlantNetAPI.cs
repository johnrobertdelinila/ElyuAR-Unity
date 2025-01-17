using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using System;

public class PlantNetAPI : MonoBehaviour
{
    // Your API Key here
    public string apiKey;
    private string project = "all"; // Try specific floras: "weurope", "canada"…
    private string apiEndpoint;

    public TakePhotos takePhotos;
    public PlantDetails plantDetails;
    //public TextMeshProUGUI responseCode;
    public TextMeshProUGUI plantText;

    public string commonName;
    public string scientificName;
    // Paths to your images
    private string imagePath2 = "Assets/Images/image_2.jpeg";


    void Start()
    {
        apiEndpoint = $"https://my-api.plantnet.org/v2/identify/{project}?api-key={apiKey}";
        
    }

    public void AnalysePhoto()
    {
        StartCoroutine(IdentifyPlant());
    }

    IEnumerator IdentifyPlant()
    {
        // Load image bytes
        //byte[] imageBytes2 = File.ReadAllBytes(imagePath2);

        byte[] imageBytes1 = File.ReadAllBytes(takePhotos.filePath);

        // Create a WWWForm and add the images and other data
        WWWForm form = new WWWForm();
        //form.AddBinaryData("images", imageBytes2, "image_2.jpeg", "image/jpeg");

        form.AddBinaryData("images", imageBytes1, takePhotos.fileName, "image/png");
        

        form.AddField("organs", "leaf");
        //form.AddField("organs", "leaf");

        // Create UnityWebRequest
        using (UnityWebRequest www = UnityWebRequest.Post(apiEndpoint, form))
        {
            // Send the request and wait for a response
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;
                //responseCode.text = "Response Code: " + www.responseCode;

                // Parse and display the JSON result
                PlantNetResponse response = JsonUtility.FromJson<PlantNetResponse>(jsonResult);
                Debug.Log("Plant Identifications:");

                PlantNetResult topResult = response.results[0];

                commonName = topResult.species.commonNames[0];
                scientificName = topResult.species.scientificNameWithoutAuthor;

                plantText.text = commonName;

                //update plant info
                plantDetails.UpdatePlantInfo();


            }
        }
    }
}

// Define the classes for JSON parsing
[System.Serializable]
public class PlantNetResponse
{
    public PlantNetResult[] results;
}

[System.Serializable]
public class PlantNetResult
{
    public PlantNetSpecies species;
    public float score;
}

[System.Serializable]
public class PlantNetSpecies
{
    public string scientificNameWithoutAuthor;
    public string[] commonNames;
}

