using UnityEngine;
using System.Collections;

public class AndroidVolume
{
    private static int curVolume;
    public static int Volume
    {
        get
        {
            return curVolume;
        }
        set
        {
            curVolume = value;
        }
    }
    public static void SetVolume(int volume)
    {
        Debug.Log("SetVolume Volume is " + volume);
#if UNITY_EDITOR
        AndroidVolume.Volume = volume;
#elif UNITY_ANDROID
       PicoUnityActivity.CallObjectMethod ("setVolume", volume);
#endif
    }
    public static int GetVolume()
    {
        int result = 0;
#if UNITY_EDITOR
        result = 20;
#elif UNITY_ANDROID
        PicoUnityActivity.CallObjectMethod<int> (ref result, "getVolume");
		Debug.Log ("get current Volume is "+result);
#endif
        curVolume = result;
        return result;
    }

    public static void AdjustVolume(bool ispositive)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            PicoUnityActivity.CallObjectMethod("AdjustVolume", ispositive.ToString());
        }
    }
}
