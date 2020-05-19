using LitJson;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

internal class AndroidTools
{
    public static string language { get; private set; }
    public static string configDir { get; private set; }
    public static string appIconsDir { get; private set; }
    public static string baseDir { get; private set; }
    public static string externalSDCardDir { get; private set; }

    public static string serverIp { get; private set; }
    public static int serverPort { get; private set; }

    public static string serialNumber { get; private set; }

    // UnityActivity.java
    private enum ConfigError
    {
        ConfigFileNotFound = 1,
        InvalidSSID,
        InvalidPassword,
        InvalidServerIpAddress,
        InvalidServerIpPort,
        InvalidConfigFile,
    }

    static public bool Initialize(ref string configError)
    {
#if UNITY_EDITOR
        return InitializeEditor(ref configError);
#else
		return InitializeAndroid(ref configError);
#endif
    }

    static public bool IsVideoExists(string fullPath)
    {
#if UNITY_EDITOR
        return IsVideoExistsEditor(fullPath);
#else
		return IsVideoExistsAndroid(fullPath);
#endif
    }

    static public bool LaunchApp(string packageName)
    {
#if UNITY_EDITOR

        return true;
#else
		return LaunchAppAndroid(packageName);
#endif
    }

    static public bool EnableKey(bool enable)
    {
#if UNITY_EDITOR
        return true;
#else
		return EnableKeyAndroid(enable);
#endif
    }

    static public bool GetWifiStatus(ref string ans)
    {
#if UNITY_EDITOR
        ans = "1#3#2";
        return true;
#else
		return GetWifiStatusAndroid(ref ans);
#endif
    }

    static public bool GetTime(ref string ans)
    {
#if UNITY_EDITOR
        ans = "00:00";
        return true;
#else
		return GetTimeAndroid(ref ans);
#endif
    }

    static public bool GetBatteryStatus(ref string ans)
    {
#if UNITY_EDITOR
        ans = "0#90";
        return true;
#else
		return GetBatteryStatusAndroid(ref ans);
#endif
    }

    private static bool InitializeEditor(ref string configError)
    {
        serverIp = "127.0.0.1";
        serverPort = 8081;
        language = "zh";
        configDir = "sdcard/pre_resource/broadcast/config/";
        appIconsDir = "sdcard/pre_resource/broadcast/appicons/";
        baseDir = "sdcard";
        externalSDCardDir = "";
        configError = "";
        serialNumber = "VR00000317350001";
        return true;
    }
    private static bool InitializeAndroid(ref string configError)
    {
        serverIp = "127.0.0.1";
        serverPort = 8081;
        language = "zh";
        baseDir = "/storage/emulated/0/";
        externalSDCardDir = "";

        string json = string.Empty;
        if (!PicoUnityActivity.CallObjectMethod(ref json, "Initialize"))
        {
            Debug.LogError("failed to call Initialize");
            return false;
        }
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("failed to call Initialize: empty json");
            return false;
        }
        Debug.Log("json:" + json);
        IDictionary dict = JsonMapper.ToObject(json) as IDictionary;


        serverIp = string.IsNullOrEmpty(dict["ip"].ToString()) ? "127.0.0.1" : dict["ip"].ToString();
        int port = 8081;
        int.TryParse(dict["port"].ToString(),out port);
        serverPort = port == 0 ? 8081: port;
        configDir = dict["configDir"].ToString();
        language = dict["language"].ToString();
        baseDir = dict["baseDir"].ToString();
        appIconsDir = dict["appIconsDir"].ToString();
        serialNumber = dict["serialNumber"].ToString();
        externalSDCardDir = dict["externalSDCardDir"].ToString();

        Debug.Log(string.Format("InitializeAndroid: ip = {0}, port = {1}, baseDir = {2}, externalSDCardDir = {3},language = {4}", serverIp, serverPort, baseDir, externalSDCardDir, language));

        return true;
    }

    private static bool GetTimeAndroid(ref string ans)
    {
        if (!PicoUnityActivity.CallObjectMethod(ref ans, "getDateTime"))
        {
            Debug.LogError("faield to call getDateTime");
            return false;
        }

        return true;
    }

    private static bool GetWifiStatusAndroid(ref string ans)
    {
        if (!PicoUnityActivity.CallObjectMethod(ref ans, "getWifiStatus"))
        {
            Debug.LogError("failed to call getWifiStatus");
            return false;
        }

        return true;
    }

    private static bool GetBatteryStatusAndroid(ref string ans)
    {
        if (!PicoUnityActivity.CallObjectMethod(ref ans, "getBatteryStatus"))
        {
            Debug.LogError("faield to call getBatteryStatus");
            return false;
        }

        return true;
    }

    private static bool IsVideoExistsEditor(string name)
    {
        name = name.Replace("file:///", "");
        return File.Exists(name);
    }

    private static bool IsVideoExistsAndroid(string name)
    {
        bool ans = false;
        if (!PicoUnityActivity.CallObjectMethod(ref ans, "CheckFileExit", name))
        {
            Debug.LogError("failed to call CheckFileExit");
            return false;
        }

        return ans;
    }

    private static bool LaunchAppAndroid(string packageName)
    {
        if (!PicoUnityActivity.CallObjectMethod("OpenApp", packageName))
        {
            Debug.LogError("failed to call OpenApp");
            return false;
        }

        return true;
    }

    private static bool istrue = false;

    public static bool EnableKeyAndroid(bool enable)
    {
        PicoUnityActivity.CallObjectMethod<bool>(ref istrue, "EnableKey", enable ? "1" : "0");
        PicoUnityActivity.CallObjectMethod("setControllerIsEnbleHomeKey", enable);
        PicoUnityActivity.CallObjectMethod("setMulkeyEnable", enable);
        return istrue;
    }
}