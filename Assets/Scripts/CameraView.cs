using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraView : MonoBehaviour
{
    RawImage rawImage;
    AspectRatioFitter fitter;
    WebCamTexture webCamTexture;
    bool ratioSet = false;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        fitter = GetComponent<AspectRatioFitter>();
        InitWebCam();
    }

    void Update()
    {
        //if(webCamTexture.width > 100 && !ratioSet)
        if(!ratioSet)
        {
            ratioSet = true;
            SetAspectRatio();
        }
    }

    void SetAspectRatio()
    {
        fitter.aspectRatio = (float)webCamTexture.width / (float)webCamTexture.height;
    }

    void InitWebCam()
    {
        string camName = WebCamTexture.devices[0].name;
        webCamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    public WebCamTexture GetCamImage()
    {
        return webCamTexture;
    }
}
