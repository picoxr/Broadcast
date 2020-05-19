using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
public class ServerPlayer : MonoBehaviour
{

    public GameObject leftcube;
    public GameObject rightcube;
    public GameObject sphere;
    public LoadXML xml;
    public GameObject pictureSphere;
    private void Awake()
    {
      
    }
    // Use this for initialization
    void Start()
    {
        NetManager.network.onNetStateChanged += ConnectionStatus;
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    /// <summary>
    /// Control Picture&Video
    /// </summary>
    /// <param name="command"></param>
    public void ServerPlayerControl(string command)
    {

        Debug.Log("ServerPlayerControl" + command);
        switch (command)
        {
            case "Stop"://Video Stop
                if (rightcube.GetComponent<VideoPlayer>().isPlaying)
                {
                    leftcube.SetActive(false);
                    rightcube.SetActive(false);
                    rightcube.GetComponent<VideoPlayer>().Stop();
                    rightcube.GetComponent<VideoPlayer>().url = null;
                    rightcube.GetComponent<MeshRenderer>().material.mainTexture = null;
                    leftcube.GetComponent<MeshRenderer>().material.mainTexture = null;
                }
                else
                {
                    sphere.SetActive(false);
                    sphere.GetComponent<VideoPlayer>().Stop();
                    sphere.GetComponent<VideoPlayer>().url = null;
                    sphere.GetComponent<MeshRenderer>().material.mainTexture = null;
                }
                break;
            case "Continue"://Video Continue
                if (rightcube.activeSelf)
                {
                    rightcube.GetComponent<VideoPlayer>().Play();
                }
                else
                {
                    sphere.GetComponent<VideoPlayer>().Play();
                }
                break;
            case "Pause"://Video Pause
                if (rightcube.activeSelf)
                {
                    rightcube.GetComponent<VideoPlayer>().Pause();
                }
                else
                {
                    sphere.GetComponent<VideoPlayer>().Pause();
                }
                break;
            case "reset"://Quit Picture
                pictureSphere.GetComponent<Renderer>().material.mainTexture = null;
                pictureSphere.SetActive(false);
                GC.Collect();
                break;

        }
    }
    /// <summary>
    /// Play Video
    /// </summary>
    /// <param name="videotype"></param>
    /// <param name="videoname"></param>
    public void ServerPlayerVideo(string videotype, string videoname)
    {
       string videoname1 = xml.videodc[videoname];
        Debug.Log("ServerPlayerVideo"+videoname1);
        Debug.Log("ServerPlayerVideo"+videotype);
        switch (videotype)
        {
            case "1"://Play 1803D_LR Video
                leftcube.SetActive(true);
                rightcube.SetActive(true);
                rightcube.GetComponent<VideoPlayer3D>().enabled = true;
                rightcube.GetComponent<VideoPlayer>().url = videoname1;
                rightcube.GetComponent<VideoPlayer>().Play();
                break;
            case "2"://Play 1803D_TB Video
                sphere.SetActive(true);
                sphere.GetComponent<VideoPlayer>().url = videoname1;
                sphere.GetComponent<VideoPlayer>().Play();
                break;
            case "7"://Play 360 Video
                leftcube.SetActive(true);
                rightcube.SetActive(true);
                rightcube.GetComponent<VideoPlayer3DOverAndUnder>().enabled = true;
                rightcube.GetComponent<VideoPlayer>().url = videoname1;
                rightcube.GetComponent<VideoPlayer>().Play();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Play Picture
    /// </summary>
    /// <param name="PicName"></param>
    public void ServerPlayPicture(string PicName)
    {
        Debug.Log("ServerPlayPicture" + PicName);
        string path = xml.picdc[PicName];
        pictureSphere.SetActive(true);
        PicoUnityActivity.CallObjectMethod("CreateBitMap", path);
    }
    /// <summary>
    ///  server is disconnected? 
    /// </summary>
    /// <param name="isconnect"></param>
    private void ConnectionStatus(bool isConnected)
    {
        if (!isConnected)
        {
            if (pictureSphere.activeSelf)
            {
                ServerPlayerControl("reset");
            }
            else
            {
                ServerPlayerControl("Stop");
            }
            Main.isfreeMode = true;
            Main.freeMode = true;
            if (!AndroidMsgReciver.allowChangeVolume)
            {
                AndroidMsgReciver.allowChangeVolume = true;
                PicoUnityActivity.CallObjectMethod("setVolumeKeyEnabled", true);
            }
            AndroidTools.EnableKeyAndroid(true);
            Main.statusBar.GetComponent<Canvas>().enabled = Main.isfreeMode;
            Main.statusBar.showMessage.text = "Under Free Mode";
        }
    }
}
