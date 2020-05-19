using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;
using System.IO;


public class AppModel
{
    public string id;
    public string name;
    public string package;
    public string picture;
    public string category;
}

public class VideoModel
{
    public string mode;  //onLocal or onLine
    public string category;
    public string id;
    public string name;
    public string picture;
    public string video;
    public string type;
}
public class PicModel 
{
    public string mode;  //onLocal or onLine
    public string category;
    public string id;
    public string name;
    public string picture;
    public string pictureurl;
    public string type;
}


public enum MenuStaus
{
    app,
    video,
    picture
}

public class LoadXML : MonoBehaviour {

    public Dictionary<string, string> dc,videodc,picdc;
    public GameObject[] buttons;
    [HideInInspector]
    public MenuStaus staus;
    private int buttonnum;
    private  AppModel app;
    private VideoModel video;
    private PicModel pic;
    public const string VideoPath = "pre_resource/broadcast/video/videolist";
    public const string AppPosterPath = "pre_resource/broadcast/appicons";
    public const string PicturePath = "pre_resource/broadcast/picture/photo";
    public const string PicturePosterPath = "pre_resource/broadcast/picture/icons";
    void Start () {
        app = new AppModel();
        video = new VideoModel();
        pic = new PicModel();
        dc = new Dictionary<string, string>();
        videodc = new Dictionary<string, string>();
        picdc = new Dictionary<string, string>();
        XmlManager.Instance.LoadXml();
        ReadAppList();
        ReadPictureList();
        ReadVideoList();
        staus = MenuStaus.video;
    }

    // Update is called once per frame
    void Update () {
	}


    public void ReadAppList()
    {
        staus = MenuStaus.app;
        HideImage();
        dc.Clear();
        //AppModel app = new AppModel();
        if (XmlManager.Instance.GetSDXmlElement("Apps") != null)
        {
            XmlNodeList videos = XmlManager.Instance.GetSDXmlElement("Apps").SelectSingleNode("AppList").ChildNodes;
            foreach (XmlElement elm in videos)
            {
          
                app.id = elm.GetAttribute("id");
                app.category = elm.GetAttribute("category");
                app.name = elm["name"].InnerText;
                app.package = elm["pkgname"].InnerText;
                app.picture = "/storage/emulated/0/pre_resource/broadcast/appicons/" + elm["posterURL"].InnerText;
                //app.picture = "E:/pre_resource/broadcast/appicons/" + elm["posterURL"].InnerText;
                if (!dc.ContainsValue(app.name))
                {
                  
                    dc.Add(app.name, app.package);
                    IoReadImage(app.picture, app.id, app.name, " ");
                }
   

            }
        }


        if (!string.IsNullOrEmpty(AndroidTools.externalSDCardDir))
        {
            if (XmlManager.Instance.GetExternalSDXmlElement("Apps") != null)
            {
                XmlNodeList videos = XmlManager.Instance.GetExternalSDXmlElement("Apps").SelectSingleNode("AppList").ChildNodes;
                foreach (XmlElement elm in videos)
                {

                    app.id = elm.GetAttribute("id");
                    app.category = elm.GetAttribute("category");
                    app.name = elm["name"].InnerText;
                    app.package = elm["pkgname"].InnerText;
                    app.picture = Path.Combine(Path.Combine(AndroidTools.externalSDCardDir, AppPosterPath), elm["posterURL"].InnerText).Replace("file://", "");
                    if (!dc.ContainsValue(app.name))
                    {

                        dc.Add(app.name, app.package);
                        IoReadImage(app.picture, app.id, app.name, " ");
                    }

                }
            }
        }

    }
    public void ReadVideoList()
    {
        staus = MenuStaus.video;
        HideImage();
        videodc.Clear();
        //VideoModel video = new VideoModel();
        if (XmlManager.Instance.GetSDXmlElement("Videos") != null)
        {
            XmlNodeList videos = XmlManager.Instance.GetSDXmlElement("Videos").SelectSingleNode("VideoList").ChildNodes;
            foreach (XmlElement elm in videos)
            {

                video.id = elm.GetAttribute("id");
                video.category = elm.GetAttribute("category");
                video.name = elm["name"].InnerText;
                video.type = elm["videoType"].InnerText;
                //video.video = "E:/pre_resource/broadcast/video/videolist/" + elm["videoURL"].InnerText;
                //video.picture = "E:/pre_resource/broadcast/video/videolist/" + elm["posterURL"].InnerText;
                video.video = "/storage/emulated/0/pre_resource/broadcast/video/videolist/" + elm["videoURL"].InnerText;
                video.picture = "/storage/emulated/0/pre_resource/broadcast/video/videolist/" + elm["posterURL"].InnerText;

                if (!videodc.ContainsValue(video.name))
                {
                
                    videodc.Add(video.name, video.video);
                    IoReadImage(video.picture, video.id, video.name,video.type);
                }
             

            }
        }


        if (!string.IsNullOrEmpty(AndroidTools.externalSDCardDir))
        {
            if (XmlManager.Instance.GetExternalSDXmlElement("Videos") != null)
            {
                XmlNodeList videos = XmlManager.Instance.GetExternalSDXmlElement("Videos").SelectSingleNode("VideoList").ChildNodes;
                foreach (XmlElement elm in videos)
                {
                    video.id = elm.GetAttribute("id");
                    video.category = elm.GetAttribute("category");
                    video.name = elm["name"].InnerText;
                    video.video = Path.Combine(Path.Combine(AndroidTools.externalSDCardDir, VideoPath), elm["videoURL"].InnerText).Replace("file://", "");
                    video.type = elm["videoType"].InnerText;
                    video.picture = Path.Combine(Path.Combine(AndroidTools.externalSDCardDir, VideoPath), elm["posterURL"].InnerText).Replace("file://", "");
                    if (!videodc.ContainsValue(video.name))
                    {
                 
                        videodc.Add(video.name, video.video);
                        IoReadImage(video.picture, video.id, video.name, video.type);
                    }
                }
            }
        }
    }
    public void ReadPictureList()
    {
        staus = MenuStaus.picture;
        HideImage();
        picdc.Clear();
        //PicModel pic = new PicModel();
        if (XmlManager.Instance.GetSDXmlElement("Pictures") != null)
        {
            XmlNodeList videos = XmlManager.Instance.GetSDXmlElement("Pictures").SelectSingleNode("PictureList").ChildNodes;
            foreach (XmlElement elm in videos)
            {
                pic.id = elm.GetAttribute("id");
                pic.category = elm.GetAttribute("category");
                pic.name = elm["name"].InnerText;
                //pic.picture = "E:/pre_resource/broadcast/picture/photo/" + elm["pictureURL"].InnerText;
                //pic.pictureurl = "E:/pre_resource/broadcast/picture/icons/" + elm["posterURL"].InnerText;

                pic.picture = "/storage/emulated/0/pre_resource/broadcast/picture/photo/" + elm["pictureURL"].InnerText;
                pic.pictureurl = "/storage/emulated/0/pre_resource/broadcast/picture/icons/" + elm["posterURL"].InnerText;

                if (!picdc.ContainsValue(pic.name))
                {
                  
                    picdc.Add(pic.name, pic.picture);
                    IoReadImage(pic.pictureurl, pic.id, pic.name," ");
                }
            }
        }


        if (!string.IsNullOrEmpty(AndroidTools.externalSDCardDir))
        {
         
            if (XmlManager.Instance.GetExternalSDXmlElement("Pictures") != null)
            {
                XmlNodeList videos = XmlManager.Instance.GetExternalSDXmlElement("Pictures").SelectSingleNode("PictureList").ChildNodes;
                foreach (XmlElement elm in videos)
                {
                    pic.id = elm.GetAttribute("id");
                    pic.category = elm.GetAttribute("category");
                    pic.name = elm["name"].InnerText;
                    pic.picture = Path.Combine(Path.Combine(AndroidTools.externalSDCardDir, PicturePath), elm["pictureURL"].InnerText).Replace("file://", "");
                    pic.pictureurl = Path.Combine(Path.Combine(AndroidTools.externalSDCardDir, PicturePosterPath), elm["posterURL"].InnerText).Replace("file://", "");
                    if (!picdc.ContainsValue(pic.name))
                    {
                   
                        picdc.Add(pic.name, pic.picture);
                        IoReadImage(pic.pictureurl, pic.id, pic.name, " ");
                    }
                }
            }
        }
    }
    public void RefreshXML()
    {
        XmlManager.Instance.LoadXml();
        HideImage();
        if (staus == MenuStaus.app)
        {
            ReadAppList();
        }
        else if (staus == MenuStaus.picture)
        {
            ReadPictureList();
        }
        else {
            ReadVideoList();
        }
    }
    public void IoReadImage(string path, string id, string name,string type)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] bytes = new byte[fileStream.Length];

        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        int width = 512;
        int height = 512;
        Texture2D texture = new Texture2D(width, height);
        buttonnum = int.Parse(id) - 1;
        texture.LoadImage(bytes);
        buttons[buttonnum].SetActive(true);
        buttons[buttonnum].GetComponentInChildren<Text>().text = name;
        buttons[buttonnum].GetComponent<RawImage>().texture = texture;
        if (staus==MenuStaus.video)
        {
            buttons[buttonnum].name = type;
        }
    }
    private void HideImage()
    {

        for (int j=0; j <=buttonnum; j++)
        {
            buttons[j].GetComponent<RawImage>().texture = null;
            buttons[j].GetComponentInChildren<Text>().text = null;
            buttons[j].SetActive(false);
        }
    }

}
