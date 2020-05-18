using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class CPlayVideo : MonoBehaviour
{
    public static CPlayVideo Instance;
    public GameObject picoPlayerObject;
    // Use this for initialization
    void Start()
    {
       // picoPlayerObject = GameObject.Find("shipin");
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.U))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void fnPlayVideo(string videoType,string name)
    {
        Debug.Log("fnPlayVideo" + name);
        string strModel = Server.Instance.fnGetServerStatus();
        if (!strModel.Equals("Free"))
        {
            picoPlayerObject.SetActive(true);
            picoPlayerObject.GetComponent<VideoPlayer>().url = CBaseData.strPlayVideoName;
            picoPlayerObject.GetComponent<VideoPlayer>().Play();
            Debug.Log("fnPlayVideo" + videoType);
            Server.Instance.fnStartVideo(videoType, name);
        }
        //Debug.Log("fnPlayVideo:Select  Movie ..... " + CBaseData.strPlayVideoName + "--------" + CBaseData.strPlayVideoTitle);
    }

    public void fnCancelPlay()
    {
        if (picoPlayerObject.activeSelf)
        {
            picoPlayerObject.GetComponent<VideoPlayer>().Stop();
            picoPlayerObject.SetActive(false);
            Server.Instance.fnStopVideo();
        }
    }
    public void fnCountinuelPlay()
    {
        if (!picoPlayerObject.GetComponent<VideoPlayer>().isPlaying)
        {
            picoPlayerObject.GetComponent<VideoPlayer>().Play();
            Server.Instance.fnCountinueVideo();
        }
    }
    public void fnPauselPlay()
    {
        if (picoPlayerObject.GetComponent<VideoPlayer>().isPlaying)
        {
            picoPlayerObject.GetComponent<VideoPlayer>().Pause();
            Server.Instance.fnPauseVideo();
        }
    }

    //public string GetVideoFullPath(string fileName)
    //{
    //    string videoPath = Path.Combine(Path.GetFullPath(Path.Combine(Application.dataPath, "..")), fileName);
    //    return videoPath;
    //    //return videoPath.Replace(@"\", @"\\");
    //}
}
