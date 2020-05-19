using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XmlManager : Singleton<XmlManager>
{
    public const string ConfigFile = "BroadcastConfig.xml";
    public const string ConfigDir = "pre_resource/broadcast/config/BroadcastConfig.xml";
    public Dictionary<string, string> VideoCategories, AppCategories, PictureCategories;
    //Config
    public XmlElement Config
    {
        get; set;
    }

    private XmlNodeList SDXmlNodeList, externalSDNodeList;

    public XmlManager()
    {

        VideoCategories = new Dictionary<string, string>();
        AppCategories = new Dictionary<string, string>();
        PictureCategories = new Dictionary<string, string>();

    }

    public void LoadXml()
    {
        VideoCategories.Clear();
        AppCategories.Clear();
        PictureCategories.Clear();
        SDXmlNodeList = null;
        externalSDNodeList = null;
        string configPath = "/storage/emulated/0/pre_resource/broadcast/config/BroadcastConfig.xml";
        //string configPath = "E:/pre_resource/broadcast/config/BroadcastConfig.xml";
        if (File.Exists(configPath))
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(configPath);
            SDXmlNodeList = xml.SelectSingleNode("root").ChildNodes;
           XmlNodeList appCategories = XmlManager.Instance.GetSDXmlElement("Apps").SelectSingleNode("AppList").ChildNodes;
            foreach (XmlElement elm in appCategories)
            {
                AppCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }


            XmlNodeList videoCategories = xml.SelectSingleNode("root/Videos/VideoCategories").ChildNodes;
            foreach (XmlElement elm in videoCategories)
            {
                VideoCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }

            XmlNodeList picCategories = xml.SelectSingleNode("root/Pictures/PictureCategories").ChildNodes;
            foreach (XmlElement elm in picCategories)
            {
                PictureCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }

        }
        configPath = Path.Combine(AndroidTools.externalSDCardDir, ConfigDir).Replace("file://", "");

        if (File.Exists(configPath))
        {
            Debug.Log("LoadXml:configPath:exist");
            XmlDocument xml = new XmlDocument();
            xml.Load(configPath);
            externalSDNodeList = xml.SelectSingleNode("root").ChildNodes;
            XmlNodeList appCategories = xml.SelectSingleNode("root/Apps/AppCategories").ChildNodes;
            foreach (XmlElement elm in appCategories)
            {
                if (!AppCategories.ContainsKey(elm.GetAttribute("index"))) AppCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }



            XmlNodeList videoCategories = xml.SelectSingleNode("root/Videos/VideoCategories").ChildNodes;
            foreach (XmlElement elm in videoCategories)
            {
                VideoCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }

            XmlNodeList picCategories = xml.SelectSingleNode("root/Pictures/PictureCategories").ChildNodes;
            foreach (XmlElement elm in picCategories)
            {
                PictureCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }




        }
    }

    public XmlElement GetSDXmlElement(string name)
    {
        if (SDXmlNodeList != null)
        {
            foreach (XmlElement xmle in SDXmlNodeList)
            {
                if (xmle.Name.Equals(name)) return xmle;
            }
        }
        return null;
    }

    public XmlElement GetExternalSDXmlElement(string name)
    {
     
        if (externalSDNodeList != null)
        {
            foreach (XmlElement xmle in externalSDNodeList)
            {
                if (xmle.Name.Equals(name)) return xmle;
            }
        }
        return null;
    }
}