using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using System;
using UnityEngine.EventSystems;



public class Launch : MonoBehaviour {

    public LoadXML xml;
    public GameObject sphere;
    public GameObject leftcube;
    public GameObject rightcube;
    public GameObject pictureSphere;
    public Canvas uiMenu;
    private  bool isPlay;
    private string appName;
    private string videoName;
    private string videoType;
    private string picName;
    private GameObject currentgoal;
    public Pvr_UIPointer[] pointers;
    // Use this for initialization
    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            pointers[i].UIPointerElementClick += StartLaunch;
        }
    }
    void Start () {
    }
	
	void Update () {

        if (Main.isfreeMode&&(xml.staus==MenuStaus.picture||xml.staus==MenuStaus.video)&&(Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(0,Pvr_UnitySDKAPI.Pvr_KeyCode.APP)|| Pvr_UnitySDKAPI.Controller.UPvr_GetKeyDown(1, Pvr_UnitySDKAPI.Pvr_KeyCode.APP)||Input.GetKeyDown(KeyCode.W)))
        {
            uiMenu.GetComponent<Canvas>().enabled = true;
        }

    }

    public void StartLaunch(object goal,UIPointerEventArgs currentUI)
    {
     
        if (currentUI.currentTarget.gameObject.layer == 11)
        {
            if (xml.staus==MenuStaus.app)
            {
                appName = currentUI.currentTarget.GetComponentInChildren<Text>().text;
                AndroidTools.LaunchApp(xml.dc[appName]);
            }
            else if (xml.staus == MenuStaus.video)
            {
                videoName = currentUI.currentTarget.GetComponentInChildren<Text>().text;
                videoType = currentUI.currentTarget.name;
                Video(xml.videodc[videoName], videoType);
            }
            else if (xml.staus == MenuStaus.picture)
            {
                picName = currentUI.currentTarget.GetComponentInChildren<Text>().text;
                DyChangeTextureNew(xml.picdc[picName]);
            }
        }
    }

    public void Video(string videoname, string videotype)
    {
        isPlay = true;
        uiMenu.GetComponent<Canvas>().enabled = false;
        int type = int.Parse(videotype);
        switch (type)
        {
            case 1:
                leftcube.SetActive(true);
                rightcube.SetActive(true);
                sphere.GetComponent<VideoPlayer>().url = videoname;
                rightcube.GetComponent<VideoPlayer3D>().enabled = true;
                rightcube.GetComponent<VideoPlayer>().url=videoname;
                rightcube.GetComponent<VideoPlayer>().Play();
                break;
            case 2:
                sphere.SetActive(true);
                sphere.GetComponent<VideoPlayer>().url=videoname;
                sphere.GetComponent<VideoPlayer>().Play();
                break;
            case 7:
                leftcube.SetActive(true);
                rightcube.SetActive(true);
                rightcube.GetComponent<VideoPlayer3DOverAndUnder>().enabled = true;
                rightcube.GetComponent<VideoPlayer>().url=videoname;
                rightcube.GetComponent<VideoPlayer>().Play();
                break;
            default:
                break;
        }
      
    }
    private void DyChangeTextureNew(string path)
    {
        pictureSphere.SetActive(true);
        uiMenu.GetComponent<Canvas>().enabled = false;
        PicoUnityActivity.CallObjectMethod("CreateBitMap", path);
    }
}

