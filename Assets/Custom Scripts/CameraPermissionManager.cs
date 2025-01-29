using UnityEngine;
using UnityEngine.Android;
using System.Collections;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class CameraPermissionManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RequestCameraPermission());
    }

    IEnumerator RequestCameraPermission()
    {
        #if UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    Debug.LogWarning("Camera permission not granted");
                    // Optionally show a message to the user
                }
            }
        #elif UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(0.1f);
                
                if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Debug.LogWarning("Camera permission not granted");
                    // Optionally show a message to the user
                }
            }
        #endif

        yield return null;
    }

    // Optional: Add this method to check permission status before navigating to AR scene
    public static bool HasCameraPermission()
    {
        #if UNITY_IOS
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
        #elif UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.Camera);
        #else
            return true;
        #endif
    }
} 