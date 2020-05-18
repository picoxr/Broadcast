using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System.Xml;
public class DataManager : Singleton<DataManager>{

    public string ConfigPath = "pre_resource\\broadcast\\config";
    public string VideoPath = "pre_resource\\broadcast\\video\\videolist";
    public string VideoPosterPath = "pre_resource\\broadcast\\video\\videolist";
    public string SecretPath = "pre_resource\\broadcast\\video\\videolist";
    public string PCSecretPath = "pre_resource\\broadcast\\video\\videolist\\pc_keys";
    public string AppPosterPath = "pre_resource\\broadcast\\appicons";
    public string PicturePath = "pre_resource\\broadcast\\picture\\photo";
    public string PicturePosterPath = "pre_resource\\broadcast\\picture\\icons";
    public string ConfigFile = "pre_resource\\broadcast\\config\\BroadcastConfig.xml";
    public List<string> VideoTypes { get; set;}
    public SortedDictionary<string, string> VideoCategories, AppCategories, PictureCategories, ClientCategories;

    public static UnityEngine.Events.UnityAction<VideoData> OnAddVideo;
    public static UnityEngine.Events.UnityAction<VideoData> OnEditVideo;
    public static UnityEngine.Events.UnityAction<AppData> OnAddApp;
    public static UnityEngine.Events.UnityAction<AppData> OnEditApp;
    public static UnityEngine.Events.UnityAction<PictureData> OnAddPicture;
    public static UnityEngine.Events.UnityAction<PictureData> OnEditPicture;
    public static UnityEngine.Events.UnityAction<ClientData> OnAddClient;
    public static UnityEngine.Events.UnityAction<List<ClientData>> OnAddClients;
    public static UnityEngine.Events.UnityAction<ClientData> OnEditClient;
    public static UnityEngine.Events.UnityAction OnEditNet;
    public static UnityEngine.Events.UnityAction OnEditCategory;
    public string ConfigText { get; set; }

    public List<VideoData> VideoDatas;
    public List<ClientData> ClientDatas;
    public List<AppData> AppDatas;
    public List<PictureData> PictureDatas;

    public string DefaultPath
    {
        get; set;
    }
    public static int system_volume = 0;

    public void CreateDirectory()
    {
        string _Directory = System.IO.Directory.GetParent(Application.dataPath).FullName;
        ConfigPath = Path.Combine(_Directory, ConfigPath);
        VideoPath = Path.Combine(_Directory, VideoPath);
        VideoPosterPath = Path.Combine(_Directory, VideoPosterPath);
        SecretPath = Path.Combine(_Directory, SecretPath);
        PCSecretPath = Path.Combine(_Directory, PCSecretPath);
        AppPosterPath = Path.Combine(_Directory, AppPosterPath);
        PicturePath = Path.Combine(_Directory, PicturePath);
        PicturePosterPath = Path.Combine(_Directory, PicturePosterPath);
        ConfigFile = Path.Combine(_Directory, ConfigFile);

        if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
        //Video classification
        if (!Directory.Exists(VideoPath)) Directory.CreateDirectory(VideoPath);
        if (!Directory.Exists(PCSecretPath)) Directory.CreateDirectory(PCSecretPath);

        if (!Directory.Exists(AppPosterPath)) Directory.CreateDirectory(AppPosterPath);

        if (!Directory.Exists(PicturePath)) Directory.CreateDirectory(PicturePath);
        if (!Directory.Exists(PicturePosterPath)) Directory.CreateDirectory(PicturePosterPath);
    }

    public void Init()
    {
        List<string> config = new List<string>();
        config.Add(XmlManager.Instance.Config.GetAttribute("ssid"));
        config.Add(XmlManager.Instance.Config.GetAttribute("pswd"));
        config.Add(XmlManager.Instance.Config.GetAttribute("serverip"));
        config.Add(XmlManager.Instance.Config.GetAttribute("port"));
        config.Add(XmlManager.Instance.Config.GetAttribute("id"));
        string[] keys = new string[] { "ssid", "pswd", "serverip", "port", "id" };
        string text = "";
        for (int i = 0; i < keys.Length; i++)
        {
            text += string.Format("{0}:{1}\r\n", keys[i], config[i]);
        }
        ConfigText = text;

        VideoTypes = new List<string>{ "3D_LR", "3D_TB","360",};
        VideoDatas = new List<VideoData>();
        if (XmlManager.Instance.Video != null && XmlManager.Instance.Video.SelectSingleNode("VideoList") != null)
        {
            XmlNodeList videos = XmlManager.Instance.Video.SelectSingleNode("VideoList").ChildNodes;
            foreach (XmlElement elm in videos)
            {
                VideoData video = new VideoData();
                video.id = elm.GetAttribute("id");
                video.category = elm.GetAttribute("category");
                video.mode = elm.GetAttribute("mode");
                video.name = elm["name"].InnerText;
                video.type = elm["videoType"].InnerText;
                video.picture = elm["posterURL"].InnerText;
                video.video = elm["videoURL"].InnerText;
                video.secret = elm["secret"].InnerText;
                video.pcSecret = elm["pcSecret"].InnerText;
                VideoDatas.Add(video);
            }
        }
        XmlNodeList videoCategories = XmlManager.Instance.getVideoCategories();
        VideoCategories = new SortedDictionary<string, string>();
        if (videoCategories != null)
        {
            foreach (XmlElement elm in videoCategories)
            {
                VideoCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }
        }
        AppDatas = new List<AppData>();
        if (XmlManager.Instance.App != null && XmlManager.Instance.App.SelectSingleNode("AppList") != null)
        {
            XmlNodeList apps = XmlManager.Instance.App.SelectSingleNode("AppList").ChildNodes;
            foreach (XmlElement elm in apps)
            {
                AppData app = new AppData();
                app.id = elm.GetAttribute("id");
                app.category = elm.GetAttribute("category");
                app.name = elm["name"].InnerText;
                app.picture = elm["posterURL"].InnerText;
                app.pkgname = elm["pkgname"].InnerText;
                AppDatas.Add(app);
            }
        }

        XmlNodeList appCategories = XmlManager.Instance.getAppCategories();
        AppCategories = new SortedDictionary<string, string>();
        if (appCategories != null)
        {
            foreach (XmlElement elm in appCategories)
            {
                AppCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }
        }
        PictureDatas = new List<PictureData>();
        if (XmlManager.Instance.Picture != null && XmlManager.Instance.Picture.SelectSingleNode("PictureList") != null)
        {
            XmlNodeList pictures = XmlManager.Instance.Picture.SelectSingleNode("PictureList").ChildNodes;
            foreach (XmlElement elm in pictures)
            {
                PictureData picture = new PictureData();
                picture.id = elm.GetAttribute("id");
                picture.category = elm.GetAttribute("category");
                picture.mode = elm.GetAttribute("mode");
                picture.name = elm["name"].InnerText;
                //picture.type = elm["type"].InnerText;
                picture.posterURL = elm["posterURL"].InnerText;
                picture.pictureURL = elm["pictureURL"].InnerText;
                PictureDatas.Add(picture);
            }
        }
        XmlNodeList pictureCategories = XmlManager.Instance.getPictureCategories();
        PictureCategories = new SortedDictionary<string, string>();
        if (pictureCategories != null)
        {
            foreach (XmlElement elm in pictureCategories)
            {
                PictureCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }
        }
        ClientDatas = new List<ClientData>();
        if (XmlManager.Instance.Client != null && XmlManager.Instance.Client.SelectSingleNode("ClientList") != null)
        {
            XmlNodeList clients = XmlManager.Instance.Client.SelectSingleNode("ClientList").ChildNodes;
            foreach (XmlElement elm in clients)
            {
                ClientData client = new ClientData();
                client.id = elm.GetAttribute("id");
                client.category = elm.GetAttribute("category");
                client.sn = elm.GetAttribute("sn");
                ClientDatas.Add(client);
            }
        }

        XmlNodeList clientCategories = XmlManager.Instance.getClientCategories();
        ClientCategories = new SortedDictionary<string, string>();
        if (ClientCategories != null)
        {
            foreach (XmlElement elm in clientCategories)
            {
                ClientCategories[elm.GetAttribute("index")] = elm.GetAttribute("name");
            }
        }
        system_volume = PlayerPrefs.GetInt("volume", 80);
    }

    /* xml last */
    #region video
    public void AddVideo(VideoData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNode xmlNode = xml.SelectSingleNode("root/Videos/VideoList");
        XmlElement elm = xml.CreateElement("video");
        elm.SetAttribute("id", data.id);
        elm.SetAttribute("category", data.category);
        elm.SetAttribute("mode", data.mode);
        xmlNode.AppendChild(elm);
        XmlElement name = xml.CreateElement("name");
        name.InnerText = data.name;
        elm.AppendChild(name);
        XmlElement posterURL = xml.CreateElement("posterURL");
        posterURL.InnerText = Path.GetFileName(data.picture);
        elm.AppendChild(posterURL);
        XmlElement videoURL = xml.CreateElement("videoURL");
        videoURL.InnerText = data.video;
        elm.AppendChild(videoURL);
        XmlElement type = xml.CreateElement("videoType");
        type.InnerText = data.type;
        elm.AppendChild(type);
        XmlElement secret = xml.CreateElement("secret");
        secret.InnerText = data.secret;
        elm.AppendChild(secret);
        XmlElement pcSecret = xml.CreateElement("pcSecret");
        pcSecret.InnerText = data.pcSecret;
        elm.AppendChild(pcSecret);
        xmlNode.AppendChild(elm);
        xml.Save(ConfigFile);
    }

    public void RemoveVideo(string id)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Videos/VideoList").ChildNodes;
        int count = 1;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == id)
            {
                elm.ParentNode.RemoveChild(elm);
                i--;
                continue;
            }
            elm.SetAttribute("id", (count++).ToString());
        }
        xml.Save(ConfigFile);
    }

    public void UpdateVideo(VideoData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Videos/VideoList").ChildNodes;
        int count = 1;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == data.id)
            {
                elm.SetAttribute("id", data.id);
                elm.SetAttribute("category", data.category);
                elm.SetAttribute("mode", data.mode);
                elm["name"].InnerText = data.name;
                elm["posterURL"].InnerText = data.picture;
                elm["videoURL"].InnerText = data.video;
                elm["videoType"].InnerText = data.type;
                elm["secret"].InnerText = data.secret;
                elm["pcSecret"].InnerText = data.pcSecret;
                break;
            }
        }
        xml.Save(ConfigFile);
    }

    public bool VideoCategoryInUse(string category)
    {
        foreach (VideoData data in VideoDatas)
        {
            if (data.category.Equals(category)) return true;
        }
        return false;
    }

    public void EditVideo(VideoData video, bool add)
    {
        if (add)
        {
            video.id = (VideoDatas.Count + 1).ToString();
            VideoDatas.Add(video);
            AddVideo(video);
            if (OnAddVideo != null)
            {
                OnAddVideo(video);
            }
        }
        else {
            UpdateVideo(video);
            if (OnEditVideo != null)
            {
                OnEditVideo(video);
            }
        }
        //Server.Instance.SendData("config", string.Concat(VideoFile, "|" + text));
    }

    public void DelVideo(VideoData video)
    {
        VideoDatas.Remove(video);
        if (File.Exists(Path.Combine(VideoPosterPath, video.picture)) && GetVideoPicPathCount(video.picture) == 0) File.Delete(Path.Combine(VideoPosterPath, video.picture));
        if (File.Exists(Path.Combine(VideoPath, video.video)) && GetVideoPathCount(video.video) == 0) File.Delete(Path.Combine(VideoPath, video.video));
        RemoveVideo(video.id);
        //Server.Instance.SendData("config", string.Concat(VideoFile, "|" + text));
    }

    public bool ExistVideo(string name)
    {
        bool exist = false;
        for (int i = 0; i < VideoDatas.Count; i++)
        {
            if (name.ToLower().Equals(VideoDatas[i].name.ToLower()))
            {
                exist = true;
                break;
            }
        }
        return exist;
    }

    public VideoData GetVideoData(int index)
    {
        return VideoDatas[index];
    }

    public int GetVideoIndex(string id)
    {
        for (int i = 0; i < VideoDatas.Count; i++)
        {
            if (id == VideoDatas[i].id)
            {
                return i;
            }
        }
        return 0;
    }

    public int GetVideoMaxIndex()
    {
        return VideoDatas.Count - 1;
    }

    public void ExchangeVideoWithPre(int index)
    {
        VideoData data = VideoDatas[index];
        VideoDatas[index] = VideoDatas[index - 1];
        VideoDatas[index - 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Videos/VideoList").ChildNodes;
        XmlNode elm0 = list.Item(index) ;
        XmlNode elm1 = list.Item(index - 1);
        elm1.ParentNode.InsertAfter(elm1,elm0);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public void ExchangeVideoWithNext(int index)
    {    
        VideoData data = VideoDatas[index];
        VideoDatas[index] = VideoDatas[index + 1];
        VideoDatas[index + 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Videos/VideoList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index + 1);
        elm0.ParentNode.InsertAfter(elm0, elm1);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public int GetVideoPathCount(string url)
    {
        int count = 0;
        for (int i = 0; i < VideoDatas.Count; i++)
        {
            if (url.Equals(VideoDatas[i].video))
            {
                count++; ;
            }
        }
        return count;
    }

    public int GetVideoPicPathCount(string url)
    {
        int count = 0;
        for (int i = 0; i < VideoDatas.Count; i++)
        {
            if (url.Equals(VideoDatas[i].picture))
            {
                count++; ;
            }
        }
        return count;
    }

    #endregion
    #region app
    public void AddApp(AppData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNode xmlNode = xml.SelectSingleNode("root/Apps/AppList");
        XmlElement elm = xml.CreateElement("app");
        elm.SetAttribute("id", data.id);
        elm.SetAttribute("category", data.category);
        XmlElement name = xml.CreateElement("name");
        name.InnerText = data.name;
        elm.AppendChild(name);
        XmlElement posterURL = xml.CreateElement("posterURL");
        posterURL.InnerText = data.picture;
        elm.AppendChild(posterURL);
        XmlElement pkgname = xml.CreateElement("pkgname");
        pkgname.InnerText = data.pkgname;
        elm.AppendChild(pkgname);
        xmlNode.AppendChild(elm);
        xml.Save(ConfigFile);
    }

    public void RemoveApp(string id)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Apps/AppList").ChildNodes;
        int count = 1;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == id)
            {
                elm.ParentNode.RemoveChild(elm);
                i--;
                continue;
            }
            elm.SetAttribute("id", (count++).ToString());
        }
        xml.Save(ConfigFile);
    }

    public void UpdateApp(AppData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Apps/AppList").ChildNodes;
        int count = 1;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == data.id)
            {
                elm.SetAttribute("id", data.id);
                elm.SetAttribute("category", data.category);
                elm["name"].InnerText = data.name;
                elm["posterURL"].InnerText = data.picture;
                elm["pkgname"].InnerText = data.pkgname;
                break;
            }
        }
        xml.Save(ConfigFile);
    }

    public void EditApp(AppData app, bool add)
    {
        if (add)
        {
            app.id = (AppDatas.Count + 1).ToString();
            AppDatas.Add(app);
            AddApp(app);
            if (OnAddApp != null)
            {
                OnAddApp(app);
            }
        }
        else
        {
            UpdateApp(app);
            if (OnEditApp != null)
            {
                OnEditApp(app);
            }
        }
        //Server.Instance.SendData("config", string.Concat(AppFile, "|" + text));
    }

    public void DelApp(AppData app)
    {
        AppDatas.Remove(app);
        if (File.Exists(Path.Combine(AppPosterPath, app.picture)) && GetAppPicPathCount(app.picture) == 0) File.Delete(Path.Combine(AppPosterPath, app.picture));
        RemoveApp(app.id);
        //Server.Instance.SendData("config", string.Concat(AppFile, "|" + text));
    }

    public bool ExistApp(string name)
    {
        bool exist = false;
        for (int i = 0; i < AppDatas.Count; i++)
        {
            if (name.Equals(AppDatas[i].name))
            {
                exist = true;
                break;
            }
        }
        return exist;
    }

    public int GetAppIndex(string id)
    {
        for (int i = 0; i < AppDatas.Count; i++)
        {
            if (id == AppDatas[i].id)
            {
                return i;
            }
        }
        return 0;
    }

    public int GetAppMaxIndex()
    {
        return AppDatas.Count - 1;
    }

    public AppData GetAppData(int index)
    {
        return AppDatas[index];
    }

    public void ExchangeAppWithPre(int index)
    {
        AppData data = AppDatas[index];
        AppDatas[index] = AppDatas[index - 1];
        AppDatas[index - 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Apps/AppList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index - 1);
        elm1.ParentNode.InsertAfter(elm1, elm0);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public void ExchangeAppWithNext(int index)
    {
        AppData data = AppDatas[index];
        AppDatas[index] = AppDatas[index + 1];
        AppDatas[index + 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Apps/AppList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index + 1);
        elm0.ParentNode.InsertAfter(elm0, elm1);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public bool AppCategoryInUse(string category)
    {
        foreach (AppData data in AppDatas)
        {
            if (data.category.Equals(category)) return true;
        }
        return false;
    }

    public int GetAppPicPathCount(string url)
    {
        int count = 0;
        for (int i = 0; i < AppDatas.Count; i++)
        {
            if (url.Equals(AppDatas[i].picture))
            {
                count++; ;
            }
        }
        return count;
    }
    #endregion
    #region picture
    public void AddPicture(PictureData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNode xmlNode = xml.SelectSingleNode("root/Pictures/PictureList");
        XmlElement elm = xml.CreateElement("picture");
        elm.SetAttribute("id", data.id);
        elm.SetAttribute("category", data.category);
        elm.SetAttribute("mode", data.mode);
        xmlNode.AppendChild(elm);
        XmlElement name = xml.CreateElement("name");
        name.InnerText = data.name;
        elm.AppendChild(name);
        XmlElement posterURL = xml.CreateElement("posterURL");
        posterURL.InnerText = data.posterURL;
        elm.AppendChild(posterURL);
        XmlElement pictureURL = xml.CreateElement("pictureURL");
        pictureURL.InnerText = data.pictureURL;
        elm.AppendChild(pictureURL);
        //XmlElement type = xml.CreateElement("type");
        //type.InnerText = data.type;
        //elm.AppendChild(type);
        xmlNode.AppendChild(elm);
        xml.Save(ConfigFile);
    }

    public void RemovePicture(string id)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Pictures/PictureList").ChildNodes;
        int count = 1;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == id)
            {
                elm.ParentNode.RemoveChild(elm);
                i--;
                continue;
            }
            elm.SetAttribute("id", (count++).ToString());
        }
        xml.Save(ConfigFile);
    }

    public void UpdatePicture(PictureData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Pictures/PictureList").ChildNodes;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == data.id)
            {
                elm.SetAttribute("id", data.id);
                elm.SetAttribute("category", data.category);
                elm.SetAttribute("mode", data.mode);
                elm["name"].InnerText = data.name;
                //elm["type"].InnerText = data.type;
                elm["posterURL"].InnerText = data.posterURL;
                elm["pictureURL"].InnerText = data.pictureURL;
                break;
            }
        }
        xml.Save(ConfigFile);
    }

    public bool PictureCategoryInUse(string category)
    {
        foreach (PictureData data in PictureDatas)
        {
            if (data.category.Equals(category)) return true;
        }
        return false;
    }

    public void EditPicture(PictureData picture, bool add)
    {
        if (add)
        {
            picture.id = (PictureDatas.Count + 1).ToString();
            PictureDatas.Add(picture);
            AddPicture(picture);
            if (OnAddPicture != null)
            {
                OnAddPicture(picture);
            }
        }
        else
        {
            UpdatePicture(picture);
            if (OnEditPicture != null)
            {
                OnEditPicture(picture);
            }
        }
        //Server.Instance.SendData("config", string.Concat(PictureFile, "|" + text));
    }

    public void DelPicture(PictureData picture)
    {
        PictureDatas.Remove(picture);
        if (File.Exists(Path.Combine(PicturePosterPath, picture.posterURL)) && GetPictureCoverPathCount(picture.posterURL) == 0) File.Delete(Path.Combine(PicturePosterPath,picture.posterURL));
        if (File.Exists(Path.Combine(PicturePath, picture.pictureURL)) && GetPicturePhotoPathCount(picture.pictureURL) == 0) File.Delete(Path.Combine(PicturePath, picture.pictureURL));
        RemovePicture(picture.id);
        //Server.Instance.SendData("config", string.Concat(PictureFile, "|" + text));
    }

    public bool ExistPicture(string name)
    {
        bool exist = false;
        for (int i = 0; i < PictureDatas.Count; i++)
        {
            if (name.Equals(PictureDatas[i].name))
            {
                exist = true;
                break;
            }
        }
        return exist;
    }

    public int GetPictureIndex(string id)
    {
        for (int i = 0; i < PictureDatas.Count; i++)
        {
            if (id == PictureDatas[i].id)
            {
                return i;
            }
        }
        return 0;
    }

    public int GetPictureMaxIndex()
    {
        return PictureDatas.Count - 1;
    }

    public PictureData GetPictureData(int index)
    {
        return PictureDatas[index];
    }

    public void ExchangePictureWithPre(int index)
    {
        PictureData data = PictureDatas[index];
        PictureDatas[index] = PictureDatas[index - 1];
        PictureDatas[index - 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Pictures/PictureList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index - 1);
        elm1.ParentNode.InsertAfter(elm1, elm0);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public void ExchangePictureWithNext(int index)
    {
        PictureData data = PictureDatas[index];
        PictureDatas[index] = PictureDatas[index + 1];
        PictureDatas[index + 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Pictures/PictureList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index + 1);
        elm0.ParentNode.InsertAfter(elm0, elm1);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public int GetPictureCoverPathCount(string url)
    {
        int count = 0;
        for (int i = 0; i < PictureDatas.Count; i++)
        {
            if (url.Equals(PictureDatas[i].posterURL))
            {
                count++; ;
            }
        }
        return count;
    }

    public int GetPicturePhotoPathCount(string url)
    {
        int count = 0;
        for (int i = 0; i < PictureDatas.Count; i++)
        {
            if (url.Equals(PictureDatas[i].pictureURL))
            {
                count++; ;
            }
        }
        return count;
    }
    #endregion
    #region client
    public void AddClient(ClientData data)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNode xmlNode = xml.SelectSingleNode("root/Clients/ClientList");
        XmlElement elm = xml.CreateElement("client");
        elm.SetAttribute("id", data.id);
        elm.SetAttribute("category", data.category);
        elm.SetAttribute("sn", data.sn);
        xmlNode.AppendChild(elm);
        xml.Save(ConfigFile);
    }

    public void RemoveClient(string id)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Clients/ClientList").ChildNodes;
        for (int i = 0; i < list.Count; i++)
        {
            XmlElement elm = list.Item(i) as XmlElement;
            if (elm.GetAttribute("id") == id)
            {
                elm.ParentNode.RemoveChild(elm);
                break;;
            }
        }
        xml.Save(ConfigFile);
    }

    public void UpdateClient(ClientData data,int index)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Clients/ClientList").ChildNodes;
        int count = 1;
        if (index != -1)
        {
            XmlElement elm = list.Item(index) as XmlElement;
            elm.SetAttribute("id", data.id);
            elm.SetAttribute("category", data.category);
            elm.SetAttribute("sn", data.sn);

        }
        xml.Save(ConfigFile);
    }

    public void AddClients(List<ClientData> clients)
    {
        ClientDatas.AddRange(clients);
        if (OnAddClient != null)
        {
            OnAddClients(clients);
        }
        foreach (ClientData client in clients)
        {
            AddClient(client);
        }
    }

    public void EditClient(ClientData client, bool add,int index)
    {
        if (add)
        {
            ClientDatas.Add(client);
            AddClient(client);
            if (OnAddClient != null)
            {
                OnAddClient(client);
            }
        }
        else
        {
            UpdateClient(client,index);
            if (OnEditClient != null)
            {
                OnEditClient(client);
            }
        }
    }

    public void DelClient(ClientData client)
    {
        ClientDatas.Remove(client);
        RemoveClient(client.id);
    }


    public int GetClientIndex(string id)
    {
        for (int i = 0; i < ClientDatas.Count; i++)
        {
            if (id == ClientDatas[i].id)
            {
                return i;
            }
        }
        return 0;
    }

    public int GetClientMaxIndex()
    {
        return ClientDatas.Count - 1;
    }

    public void ExchangeClientWithPre(int index)
    {
        ClientData data = ClientDatas[index];
        ClientDatas[index] = ClientDatas[index - 1];
        ClientDatas[index - 1] = data;


        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Clients/ClientList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index - 1);
        elm1.ParentNode.InsertAfter(elm1, elm0);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public void ExchangeClientWithNext(int index)
    {
        ClientData data = ClientDatas[index];
        ClientDatas[index] = ClientDatas[index + 1];
        ClientDatas[index + 1] = data;

        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlNodeList list = xml.SelectSingleNode("root/Clients/ClientList").ChildNodes;
        XmlNode elm0 = list.Item(index);
        XmlNode elm1 = list.Item(index + 1);
        elm0.ParentNode.InsertAfter(elm0, elm1);
        string id0 = (elm0 as XmlElement).GetAttribute("id");
        string id1 = (elm1 as XmlElement).GetAttribute("id");
        (elm0 as XmlElement).SetAttribute("id", id1);
        (elm1 as XmlElement).SetAttribute("id", id0);
        xml.Save(ConfigFile);
    }

    public ClientData GetClientData(int index)
    {
        return ClientDatas[index];
    }

    public ClientData GetClientById(string id)
    {
        foreach (ClientData client in ClientDatas)
        {
            if (client.id == id) return client;
        }
        return null;
    }

    public ClientData GetLastClientById(string id)
    {
        for(int i = 0;i < ClientDatas.Count;i++)
        {
            ClientData client = ClientDatas[i];
            if (client.id == id) return ClientDatas[i-1];

        }
        return null;
    }

    public ClientData GetNextClientById(string id)
    {
        for (int i = 0; i < ClientDatas.Count; i++)
        {
            ClientData client = ClientDatas[i];
            if (client.id == id) return ClientDatas[i + 1];

        }
        return null;
    }

    public ClientData GetClientBySn(string sn)
    {
        foreach (ClientData client in ClientDatas)
        {
            if (client.sn == sn) return client;
        }
        return null;
    }
    public bool ClientCategoryInUse(string category)
    {
        foreach (ClientData data in ClientDatas)
        {
            if (data.category.Equals(category)) return true;
        }
        return false;
    }

    #endregion

    public List<string> GetCategories(DataType type)
    {
        SortedDictionary<string, string> categories = VideoCategories;
        switch (type)
        {
            case DataType.app:
                categories = AppCategories;
                break;
            case DataType.picture:
                categories = PictureCategories;
                break;
            case DataType.client:
                categories = ClientCategories;
                break;
        }
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, string> category in categories)
        {
            list.Add(category.Value);
        }
        return list;
    }

    public string GetCategoryId(DataType type, string value)
    {
        SortedDictionary<string, string> categories = VideoCategories;
        switch (type)
        {
            case DataType.app:
                categories = AppCategories;
                break;
            case DataType.picture:
                categories = PictureCategories;
                break;
            case DataType.client:
                categories = ClientCategories;
                break;
        }
        foreach (KeyValuePair<string, string> category in categories)
        {
            if (value == category.Value) return category.Key;
        }
        return "0";
    }
    public string GetCategory(DataType type, string id)
    {
        SortedDictionary<string, string> categories = VideoCategories;
        switch (type)
        {
            case DataType.app:
                categories = AppCategories;
                break;
            case DataType.picture:
                categories = PictureCategories;
                break;
            case DataType.client:
                categories = ClientCategories;
                break;
        }
        foreach (KeyValuePair<string, string> category in categories)
        {
            if (id == category.Key) return category.Value;
        }
        return "Ungrouped";
    }

    public string GetNewCategoryId(DataType type)
    {
        int newId = 1;
        SortedDictionary<string, string> categories = VideoCategories;
        switch (type)
        {
            case DataType.app:
                categories = AppCategories;
                break;
            case DataType.picture:
                categories = PictureCategories;
                break;
            case DataType.client:
                categories = ClientCategories;
                break;
        }
        foreach (KeyValuePair<string, string> category in categories)
        {
            if (System.Convert.ToInt32(category.Key) >= newId)
            {
                newId = System.Convert.ToInt32(category.Key) + 1;
            }
        }
        return newId.ToString();
    }

    public void UpdateCategories(DataType type, SortedDictionary<string, string> categories)
    {
        
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        string path = "";
        switch (type)
        {
            case DataType.Video:
                path = "root/Videos/VideoCategories";
                VideoCategories = categories;
                break;
            case DataType.app:
                path = "root/Apps/AppCategories";
                AppCategories = categories;
                break;
            case DataType.picture:
                path = "root/Pictures/PictureCategories";
                PictureCategories = categories;
                break;
            case DataType.client:
                path = "root/Clients/ClientCategories";
                ClientCategories = categories;
                break;
        }
        XmlNode list = xml.SelectSingleNode(path);
        list.RemoveAll();
        foreach (KeyValuePair<string, string> category in categories)
        {
            XmlElement elm = xml.CreateElement("category");
            elm.SetAttribute("index", category.Key);
            elm.SetAttribute("name", category.Value);
            list.AppendChild(elm);
        }
        xml.Save(ConfigFile);
        if (OnEditCategory != null) OnEditCategory();
    }

    public string AddCategory(DataType type, string category)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        string path = "";
        string id = GetNewCategoryId(type);
        switch (type)
        {
            case DataType.Video:
                path = "root/Videos/VideoCategories";
                VideoCategories[id] = category;
                break;
            case DataType.app:
                path = "root/Apps/AppCategories";
                AppCategories[id] = category;
                break;
            case DataType.picture:
                path = "root/Pictures/PictureCategories";
                PictureCategories[id] = category;
                break;
            case DataType.client:
                path = "root/Clients/ClientCategories";
                ClientCategories[id] = category;
                break;
        }
        XmlNode list = xml.SelectSingleNode(path);
        XmlElement elm = xml.CreateElement("category");
        elm.SetAttribute("index", id);
        elm.SetAttribute("name", category);
        list.AppendChild(elm);
        xml.Save(ConfigFile);
        return id;
    }

    #region net
    public void UpdateNetConfig(List<string> config)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(ConfigFile);
        XmlElement xmlNode = xml.SelectSingleNode("root/config") as XmlElement;
        xmlNode.SetAttribute("ssid", config[0]);
        xmlNode.SetAttribute("pswd", config[1]);
        xmlNode.SetAttribute("serverip", config[2]);
        xmlNode.SetAttribute("port", config[3]);
        xmlNode.SetAttribute("id", config[4]);
        xml.Save(ConfigFile);

        string[] keys = new string[] { "ssid", "pswd", "serverip", "port", "id" };
        string text = "";
        for (int i = 0; i < keys.Length; i++)
        {
            text += string.Format("{0}:{1}\r\n", keys[i], config[i]);
        }
        ConfigText = text;
        if (OnEditNet != null) OnEditNet();
        Server.Instance.SendData("config", string.Concat(ConfigPath, "|" + text));
    }
    #endregion
    #region volume
    public void SaveVolume()
    {
        PlayerPrefs.SetInt("volume",system_volume);
    }
    #endregion
    //public void SynchronousConfig(NetworkPlayer info)
    //{
    //    //string tarPath = Path.Combine(ConfigPath, VideoFile);
    //    //string text = File.ReadAllText(tarPath);
    //    //Server.Instance.SendData("config", string.Concat(VideoFile, "|" + text));
    //    //tarPath = Path.Combine(ConfigPath, AppFile);
    //    //text = File.ReadAllText(tarPath);
    //    //Server.Instance.SendData("config", string.Concat(AppFile, "|" + text));
    //    //tarPath = Path.Combine(ConfigPath, PictureFile);
    //    //text = File.ReadAllText(tarPath);
    //    //Server.Instance.SendData("config", string.Concat(PictureFile, "|" + text));
    //    string tarPath = Path.Combine(ConfigPath, ConfigPath).Replace("/","/");
    //    string text = File.ReadAllText(tarPath);
    //    Server.Instance.SendData(info, "config", string.Concat(ConfigPath, "|" + text));
    //}

    public string GetVideoType(string type)
    {
        string _type = "2D";
        switch (type)
        {
           
            case "1":
                _type = "3D_LR";
                break;
            case "2":
                _type = "360";
                break;
         
            case "7":
                _type = "3D_TB";
                break;
           

        }
        return _type;

    }

    public string GetVideoTypeIndex(string name)
    {
        string index = "0";
        switch (name)
        {
           
            case "3D_LR":
                index = "1";
                break;
            case "360":
                index = "2";
                break;
            case "3D_TB":
                index = "7";
                break;
         
        }
        return index;
    }
    //0  //2D
    //1  //3DLeft and right
    //7  //3DUp and down
    //2  //360°Panorama
    //5  //360°Left and right
    //3  //360°Up and down
    //10  //180°Panorama
    //13  //180°Left and right
    //11  //180°Up and down
    //15  //Panorama of fish eye
    //18  //Fish eye left and right
    //16  //Fish eye up and down

}

//Client data
public class ClientData
{
    public string id = "";
    public string sn = "";
    public string category = "";
    public bool accept =  true;
}

//System data
public class SystemData
{
    public string title = "";
    public string value = "";
}

//Video data
public class VideoData
{
    public string mode = "Local";
    public string category = "";
    public string id = "";
    public string name = "";
    public string picture = "";
    public string video = "";
    public string type = "";
    public string secret = "";
    public string pcSecret = "";
}

//Application data
public class AppData
{
    public string category = "";//App category
    public string id = "";//APP ID
    public string name = "";//App name
    public string picture = "";//App Icon
    public string pkgname = "";//Package name：com.xxx.xxx.xxx
}

//Application data
public class PictureData
{
    public string mode = "Local";
    public string category = "";//category
    public string id = "";//ID
    public string name = "";//name
    public string posterURL = "";//Icon
    public string pictureURL = "";//
    public string type = "";//
}

public enum DataType
{
    Video,
    app,
    picture,
    client
}
