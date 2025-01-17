using UnityEngine;
using System.Runtime.InteropServices;

public class URLHandler
{
    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _OpenMapsWithAddress(string address);
    
    public static void OpenMaps(string address)
    {
        Debug.Log($"[URLHandler] Opening maps with address: {address}");
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            try
            {
                _OpenMapsWithAddress(address);
                Debug.Log($"[URLHandler] Successfully called native maps function");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[URLHandler] Error opening maps: {e.Message}");
                Debug.LogError($"[URLHandler] Stack trace: {e.StackTrace}");
            }
        }
        else
        {
            Debug.LogWarning($"[URLHandler] Not running on iPhone device");
        }
    }
    #endif
} 