using System;
using UnityEngine;

public class AndroidMsgReciver : MonoBehaviour
{
    private static AndroidMsgReciver _instance = null;
    public static bool isShow = false;
    public static bool changeVolume = false;
    public static bool allowChangeVolume = true;
    public static AndroidMsgReciver instance

    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AndroidMsgReciver");
                _instance = go.AddComponent<AndroidMsgReciver>();
          
            }
            return _instance;
        }
    }

    public void Init()
    {

    }

    public void Android_SendVolumeToUnity(String volume)
    {
        Debug.Log("Android_SendVolumeToUnity:" + volume + " " + AndroidVolume.Volume);
        if (AndroidVolume.Volume == int.Parse(volume))
        {
            return;
        }
        float vaule = float.Parse(volume) / 100.0f;
        if (vaule < 0.0f || vaule > 1.0f)
            Debug.Log("Android_SendVolumeToUnity:"+"the volume value is " + volume + " !!!!!");
        else
        {
            AndroidVolume.Volume = int.Parse(volume);
            changeVolume = true;
        }
    }
    public void Android_SendPlayerStatusToUnity(string msg)
    {
     
        if (string.IsNullOrEmpty(msg))
        {
            Debug.LogError("Android_SendPlayerStatusToUnity" + "the msg is null in Android_SendPlayerStatusToUnity!");
        }
        else if (msg == "1")
        {

        }
        else if (msg == "2")
        {
            //Application.Quit();
        }
        else if (msg == "3")
        {
        }
    }

    public void Android_SendHeadsetPlugToUnity(string volume)
    {
        Debug.Log("Android_SendHeadsetPlugToUnity" + "the Headset Plug ! volume value " + volume + " !!!!!");
        AndroidVolume.GetVolume();
    }

   
    public void OnHomeKeyEvent(string str)
    {


        if (str.Equals("recentapps"))  
        {
  
        }
        else if (str.Equals("homekey"))  
        {

        }
    }
}
