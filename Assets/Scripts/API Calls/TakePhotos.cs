using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TakePhotos : MonoBehaviour
{
    [HideInInspector] public string fileName;
    [HideInInspector] public string filePath;
    public void TakePhoto()
    {
        StartCoroutine(TakeAPhoto());
    }
    IEnumerator TakeAPhoto()
    {
        yield return new WaitForEndOfFrame(); 
        Camera camera = Camera.main;

        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;

        //render texture in rendertexture.active is the
        //one that will be read by readpixels

        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;
        RenderTexture.active = rt;
        

        //render the cameras view
        camera.Render();

        //make a new texture and read the active render texture into it
        Texture2D image = new Texture2D(width, height);
        image.ReadPixels(new Rect(0, 0, width, height),0,0);
        image.Apply();


        camera.targetTexture = null;
        //replace original active render texture
        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();
        fileName = "userPlantImage.png";
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(filePath, bytes);

        Destroy(rt);
        Destroy(image);
    }
}
