using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XmlManager : Singleton<XmlManager>
{
      public string ConfigFile = "pre_resource/broadcast/config/BroadcastConfig.xml";
     //public string ConfigFile = "C:/Users/laker.liu/Desktop/NoTest/pre_resource/broadcast/config/BroadcastConfig.xml";
    public static string Directory;

    //Config
    public XmlElement Config
    {
        get; set;
    }

    //Client
    public XmlElement Client
    {
        get; set;
    }

    //APP
    public XmlElement App
    {
        get;set;
    }
    //Video
    public XmlElement Video
    {
        get; set;
    }

    //Picture
    public XmlElement Picture
    {
        get; set;
    }

    public XmlManager()
    {
        Directory = System.IO.Directory.GetParent(Application.dataPath).FullName;
        ConfigFile = Path.Combine(Directory,ConfigFile);
        createXml();
        LoadXml();
    }

    public void createXml()
    {
        if (!File.Exists(ConfigFile))
        {
            //Create XML document instance
            XmlDocument xmlDoc = new XmlDocument();
            //Create the root node, the top node
            XmlElement root = xmlDoc.CreateElement("root");
            //Network config
            XmlElement elmConfig = xmlDoc.CreateElement("config");
            elmConfig.SetAttribute("ssid", "");
            elmConfig.SetAttribute("pswd", "");
            elmConfig.SetAttribute("serverip", "");
            elmConfig.SetAttribute("port", "");
            root.AppendChild(elmConfig);
            //voice
            //Network config
            elmConfig.SetAttribute("ssid", "");
            elmConfig.SetAttribute("pswd", "");
            elmConfig.SetAttribute("serverip", "");
            elmConfig.SetAttribute("port", "");
            root.AppendChild(elmConfig);

            XmlElement elmApp = xmlDoc.CreateElement("Apps");
            XmlNode elmAppList = elmApp.AppendChild(xmlDoc.CreateElement("AppList"));
            XmlNode elmAppCategory = elmApp.AppendChild(xmlDoc.CreateElement("AppCategories"));
            root.AppendChild(elmApp);

            XmlElement elmClient = xmlDoc.CreateElement("Clients");
            XmlNode elmClientList = elmClient.AppendChild(xmlDoc.CreateElement("ClientList"));
            XmlNode elmClientCategory = elmClient.AppendChild(xmlDoc.CreateElement("ClientCategories"));
            root.AppendChild(elmClient);

            XmlElement elmVideo = xmlDoc.CreateElement("Videos");
            XmlNode elmVideoList = elmVideo.AppendChild(xmlDoc.CreateElement("VideoList"));
            XmlNode elmVideoCategory = elmVideo.AppendChild(xmlDoc.CreateElement("VideoCategories"));
            root.AppendChild(elmVideo);

            XmlElement elmPicture = xmlDoc.CreateElement("Pictures");
            XmlNode elmPictureList = elmPicture.AppendChild(xmlDoc.CreateElement("PictureList"));
            XmlNode elmPictureCategory = elmPicture.AppendChild(xmlDoc.CreateElement("PictureCategories"));
            root.AppendChild(elmPicture);

            xmlDoc.AppendChild(root);
            //Save XML file locally
            xmlDoc.Save(ConfigFile);
            Debug.Log("createXml:createXml OK!");
        }
    }

    //Load
    public void LoadXml()
    {
        //Create XML document
        XmlDocument xml = new XmlDocument();        
        xml.Load(ConfigFile);
        //Get all the child nodes under the objects node
        XmlNodeList xmlNodeList = xml.SelectSingleNode("root").ChildNodes;
        //Traverse all child nodes
        foreach (XmlElement xmle in xmlNodeList)
        {
            if (xmle.Name.Equals("config")) Config = xmle;
            else if (xmle.Name.Equals("Clients")) Client = xmle;
            else if (xmle.Name.Equals("Videos")) Video = xmle;
            else if (xmle.Name.Equals("Apps")) App = xmle;
            else if (xmle.Name.Equals("Pictures")) Picture = xmle;
        }
    }

    /// <summary>
    /// Get all app categories
    /// </summary>
    /// <returns></returns>
    public XmlNodeList getAppCategories()
    {
        if (App != null && App.SelectSingleNode("AppCategories") != null)
        {
            return App.SelectSingleNode("AppCategories").ChildNodes;
        }
        return null;
    }

    /// <summary>
    /// Get the corresponding app according to the categoryindex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public XmlElement getAppInCategoryIndex(string index)
    {
        foreach (XmlElement xmle in App.SelectSingleNode("AppList").ChildNodes)
        {
            if (xmle.Attributes["category"].Equals(index))
            {
                return xmle;
            }
        }
        Debug.Log("getAppInCategoryIndex:SX---No corresponding AppCategoryIndex");
        return null;
    }

    /// <summary>
    /// Get all video categories
    /// </summary>
    /// <returns></returns>
    public XmlNodeList getVideoCategories()
    {
        if (Video != null && Video.SelectSingleNode("VideoCategories") != null)
        {
            return Video.SelectSingleNode("VideoCategories").ChildNodes;
        }
        return null;
    }

    /// <summary>
    ///Get the corresponding video according to the categoryindex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public XmlElement getVideoInCategoryIndex(string index)
    {
        foreach (XmlElement xmle in Video.SelectSingleNode("VideoList").ChildNodes)
        {
            if (xmle.Attributes["category"].Equals(index))
            {
                return xmle;
            }
        }
        Debug.Log("getVideoInCategoryIndex:SX---No corresponding VideoCategoryIndex");
        return null;
    }

    /// <summary>
    /// Get all picture categories
    /// </summary>
    /// <returns></returns>
    public XmlNodeList getPictureCategories()
    {
        if (Picture != null && Picture.SelectSingleNode("PictureCategories") != null)
        {
            return Picture.SelectSingleNode("PictureCategories").ChildNodes;
        }
        return null;
    }

    /// <summary>
    /// Get the corresponding picture according to the categoryindex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public XmlElement getPictureInCategoryIndex(string index)
    {
        foreach (XmlElement xmle in Picture.SelectSingleNode("PictureList").ChildNodes)
        {
            if (xmle.Attributes["category"].Equals(index))
            {
                return xmle;
            }
        }
        Debug.Log("getPictureInCategoryIndex:SX---No corresponding PictureCategoryIndex");
        return null;
    }

    /// <summary>
    /// Get all client categories
    /// </summary>
    /// <returns></returns>
    public XmlNodeList getClientCategories()
    {
        if (Client != null && Client.SelectSingleNode("ClientCategories") != null)
        {
            return Client.SelectSingleNode("ClientCategories").ChildNodes;
        }
        return null;
    }

    /// <summary>
    /// Get the corresponding client according to the categoryindex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public XmlElement getClientInCategoryIndex(string index)
    {
        foreach (XmlElement xmle in Client.SelectSingleNode("ClientList").ChildNodes)
        {
            if (xmle.Attributes["category"].Equals(index))
            {
                return xmle;
            }
        }
        Debug.Log("getClientInCategoryIndex:SX---No corresponding ClientCategoryIndex");
        return null;
    }

}