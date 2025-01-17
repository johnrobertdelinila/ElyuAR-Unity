using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class Classification : MonoBehaviour
{
    const int IMAGE_SIZE = 640;
    //const string INPUT_NAME = "images";
    const string INPUT_NAME = "actual_input";
    //const string OUTPUT_NAME = "Softmax";
    const string OUTPUT_NAME = "output";

    [Header("Model Stuff")]
    public NNModel modelFile;
    public TextAsset labelAsset;

    [Header("Scene Stuff")]
    public CameraView cameraView;
    public Preprocess preProcess;
    public Text uiText;

    string[] labels;
    string[] labelsPlants;
    IWorker worker;
    void Start()
    {
        var model = ModelLoader.Load(modelFile);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        LoadLabels();
    }

    void LoadLabels()
    {
        var stringArray = labelAsset.text.Split('"').Where((item, index) => index % 2 != 0);
        labels = stringArray.Where((x, i ) => i % 2 != 0).ToArray();
        string[] labelsPlants  = {
            "ginger", "banana", "tobacco", "ornamental", "rose", "soybean", "papaya", "garlic", "raspberry", "mango", 
            "cotton", "corn", "pomegranate", "strawberry", "blueberry", "brinjal", "potato", "wheat", "olive", "rice", 
            "lemon", "cabbage", "guava", "chilli", "capsicum", "sunflower", "cherry", "cassava", "apple", "tea", 
            "sugarcane", "groundnut", "weed", "peach", "coffee", "cauliflower", "tomato", "onion", "gram", "chiku", 
            "jamun", "castor", "pea", "cucumber", "grape", "cardamom"
        };
    }

    // Update is called once per frame
    void Update()
    {
        WebCamTexture webCamTexture = cameraView.GetCamImage();

        if(webCamTexture.didUpdateThisFrame && webCamTexture.width > 100)
        {
            preProcess.ScaleAndCropImage(webCamTexture, IMAGE_SIZE, RunModel);
        }
    }

    void RunModel(byte[] pixels)
    {
        StartCoroutine(RunModelRoutine(pixels));
    }

    IEnumerator RunModelRoutine(byte[] pixels)
    {

        //Tensor tensor = TransformInput(pixels);
        float[] transformedPixels = new float[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            transformedPixels[i] = (float)pixels[i];
        }

        Tensor tensor = new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);

        var inputs = new Dictionary<string, Tensor> {
            { INPUT_NAME, tensor }
        };

        worker.Execute(inputs);
        Tensor outputTensor = worker.PeekOutput(OUTPUT_NAME);

        //get largest output
        List<float> temp = outputTensor.ToReadOnlyArray().ToList();
        float max = temp.Max();
        int index = temp.IndexOf(max);

        //set UI text
        //uiText.text = labels[index];
            uiText.text = labelsPlants[index];

        //dispose tensors
        tensor.Dispose();
        outputTensor.Dispose();
        yield return null;
    }

    //transform from 0-255 to -1 to 1
    Tensor TransformInput(byte[] pixels)
    {
        float[] transformedPixels = new float[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            transformedPixels[i] = (pixels[i] - 127f) / 128f;
        }
        return new Tensor(1, IMAGE_SIZE, IMAGE_SIZE, 3, transformedPixels);
    }
}
