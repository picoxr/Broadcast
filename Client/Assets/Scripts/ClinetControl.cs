using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class ClinetControl : MonoBehaviour {

    public LoadXML loadxml;
    public GameObject rightcube;
    public GameObject leftcube;
    public GameObject sphere;
    public GameObject pictureSphere;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
     
    }
    public void LoadApp()
    {

        HidePlayer();
        loadxml.ReadAppList();
    }
    public void LoadVideo()
    {

        HidePlayer();
        loadxml.ReadVideoList();
    }
    public void LoadPicture()
    {
     
        HidePlayer();
        loadxml.ReadPictureList();
    }
    public void Refresh()
    {

        HidePlayer();
        loadxml.RefreshXML();
    }
    public void HidePlayer()
    {
        if (rightcube.GetComponent<VideoPlayer>().isPlaying)
        {
            rightcube.GetComponent<VideoPlayer>().Stop();
            leftcube.SetActive(false);
            rightcube.SetActive(false);

        }
        else if (sphere.GetComponent<VideoPlayer>().isPlaying)
        {
            sphere.GetComponent<VideoPlayer>().Stop();
            sphere.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
            sphere.SetActive(false);
        }
        if (pictureSphere.activeSelf)
        {
            pictureSphere.SetActive(false);
        }
    }
}
