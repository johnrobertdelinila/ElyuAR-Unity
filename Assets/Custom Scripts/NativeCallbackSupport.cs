using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class NativeCallbackSupport : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // This empty method ensures proper IL2CPP code generation
        Debug.Log("[NATIVE] Initializing native callback support");
    }
} 