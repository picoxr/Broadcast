﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Pico unity activity.
/// </summary>
public class PicoUnityActivity
{
    private static AndroidJavaObject picoUnityAcvity = null;

    private static AndroidJavaObject CurrentActivity()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string packageName = "com.picovr.lianboplayer.UnityActivity";
            if (picoUnityAcvity == null)
            {
                picoUnityAcvity = new AndroidJavaClass(packageName).GetStatic<AndroidJavaObject>("unityActivity");
            }
        }
        return picoUnityAcvity;
    }

    public static bool CallObjectMethod(string name, params object[] args)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return false;
        }
        if (CurrentActivity() == null)
        {
            Debug.LogWarning("Object is null when calling method " + name);
            return false;
        }
        try
        {
            CurrentActivity().Call(name, args);
            return true;
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError("Exception calling method " + name + ": " + e);
            return false;
        }
    }

    //add by Licky start
    public static bool CallObjectMethod<T>(ref T result, string name)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return false;
        }
        if (CurrentActivity() == null)
        {
            Debug.LogWarning("Object is null when calling method " + name);
            return false;
        }
        try
        {
            result = CurrentActivity().Call<T>(name);
            return true;
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError("Exception calling method " + name + ": " + e);
            return false;
        }
    }

    //add by Licky end

    public static bool CallObjectMethod<T>(ref T result, string name,
                                              params object[] args)
    {
        if (CurrentActivity() == null)
        {
            Debug.LogError("Object is null when calling method " + name);
            return false;
        }
        try
        {
            result = CurrentActivity().Call<T>(name, args);
            return true;
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError("Exception calling method " + name + ": " + e);
            return false;
        }
    }

    /// <summary>
    /// Get the specified activity
    /// </summary>
    /// <param name="package_name">Package name of Activity</param>
    /// <param name="activity_name">Name of Activity</param>
    /// <returns>Specified activity handle</returns>
    public static AndroidJavaObject GetActivity(string package_name, string activity_name)
    {
        return new AndroidJavaClass(package_name).GetStatic<AndroidJavaObject>(activity_name);
    }
}