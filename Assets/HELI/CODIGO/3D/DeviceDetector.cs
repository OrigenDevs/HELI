using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceDetector : MonoBehaviour
{
    public static bool isMobileWebGl = false;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool IsMobileDevice();
#endif

    [SerializeField] private GameObject touchUI;

    void Start()
    {
        bool isMobile = false;


#if UNITY_WEBGL && !UNITY_EDITOR
        isMobile = IsMobileDevice();
        DeviceDetector.isMobileWebGl = IsMobileDevice();
#elif UNITY_ANDROID || UNITY_IOS
        isMobile = true;
        DeviceDetector.isMobileWebGl = true;
#endif

        touchUI.SetActive(isMobile);
    }
}
