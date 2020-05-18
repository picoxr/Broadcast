using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using LitJson;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using MergeConfig;

public class CLoadJson : MonoBehaviour
{
    #region Parameter setting
    public static CLoadJson Instance;
    public const string SYSTEM_VOLUME = "SYSTEM_VOLUME";
    public string _folder = "./";
    public string _Videofilename = "", _ClientFileName = "", _Appfilename = "", _Picfilename = "";
    public bool _useStreamingAssetsPath;

    //Client UI object
    public GameObject gaClientUI;

    //Client UI parent
    public GameObject gaClientUIFather;

    //The client UI which is used for video playback
    public GameObject gaClientUIPlayFather;

    //Video class UI parent node
    public GameObject gaVideoUIFather;

    //Application class UI parent node
    public GameObject gaAppUIFather;

    //Picture class UI parent node
    public GameObject gaPicUIFather;

    //Client for video playback
    private List<GameObject> gaClientListPlaying = new List<GameObject>();

    //Video screenshot
    //List<Sprite> sprVideoList = new List<Sprite>();

    //Wrong screenshot
    public Sprite sprError;

    //User number drop-down list
    public Dropdown Drd_IDList;

    private List<string> UserIdList = new List<string>();

    //Company logo picture
    public Image imLogo;

    public string strLogoName = "logo.png";

    //Is it a type selection
    //private bool IsSortSelect = false;

    //Client select all and invert selection status
    private bool IsAll = true;

    //Number of last synced client
    private string strLastSynSn = "";

    /////Played time
    //public Text AlreadyPlayedTime;

    //private float appTime = 0f;
    //private bool isOpenApp = false;

    public Sprite[] spVideoType;
    //List<clientStatus> clientList = new List<clientStatus>();

    //Gif path
    private string loadingGifPath = "/loading_loop.gif";

    //Store the image from the parsing gif
    public static List<Texture2D> gifFrames = null;

    #endregion Parameter setting

    #region UDP Socket Broadcast

    //UDP Socket
    public int Port = 7975;

    public int IntervalTime = 20;
    private Socket socket; //Target socket
    //SocketServer server;
    private IPEndPoint ipEndPoint; //Listening port
    private byte[] sendData; //Data sent, must be bytes
    private Thread socketThread; //Connection thread

    //Initialize server
    private void InitSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //ipEndPoint = new IPEndPoint(IPAddress.Broadcast, Port);
        //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        //Start a thread connection, required, or the main thread is stuck
        socketThread = new Thread(new ThreadStart(SocketReceive));
        socketThread.Start();

        //server = new SocketServer(6000, 1000);
        //server.Start();
    }

    public void SocketSend(string sendStr)
    {
        Thread.Sleep(IntervalTime);
        //Clear send cache
        sendData = new byte[1024];
        //Data type conversion
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //Send to specified client
        //socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEndPoint);
        //Debug.Log(sendData.Length);
        foreach (var info in GetComponent<Server>().clientList)
        {
            //Accept server instructions or not
            bool IsAccept = fnGetClientControl(info.sn);
            if (IsAccept)
            {
                Debug.Log("SocketSend:"+info.sn + "---" + IPAddress.Parse(info.netPlay.ipAddress) + "---" + Port);
                ipEndPoint = new IPEndPoint(IPAddress.Parse(info.netPlay.ipAddress), Port);
                Debug.Log(" SocketSend:Socket:" + socket.Connected);
                //Send to specified client
                socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEndPoint);
            }
        }
    }

    //Server receive
    private void SocketReceive()
    {
    }

    //Connection closed
    private void SocketQuit()
    {
        //Close thread
        if (socketThread != null)
        {
            socketThread.Interrupt();
            socketThread.Abort();
        }
        //Last close socket
        if (socket != null)
            socket.Close();
        print("SocketQuit:disconnect");

        //server.Close();
    }

    private void OnApplicationQuit()
    {
        SocketSend("OnApplicationQuit:tohome");
        SocketQuit();
    }

    #endregion UDP Socket Broadcast

    private void Awake()
    {
        Instance = this;
        gifFrames = new List<Texture2D>();
        //Loading GIF animation loaded when loading video or applying icons
        System.Drawing.Image gifImage = System.Drawing.Image.FromFile(Application.streamingAssetsPath + loadingGifPath);
        System.Drawing.Imaging.FrameDimension dimension = new System.Drawing.Imaging.FrameDimension(gifImage.FrameDimensionsList[0]);
        int frameCount = gifImage.GetFrameCount(dimension);
        for (int i = 0; i < frameCount; i++)
        {
            gifImage.SelectActiveFrame(dimension, i);
            System.Drawing.Bitmap frame = new System.Drawing.Bitmap(gifImage.Width, gifImage.Height);
            System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, System.Drawing.Point.Empty);
            Texture2D frameTexture = new Texture2D(frame.Width, frame.Height);
            for (int x = 0; x < frame.Width; x++)
                for (int y = 0; y < frame.Height; y++)
                {
                    System.Drawing.Color sourceColor = frame.GetPixel(x, y);
                    frameTexture.SetPixel(x, frame.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A)); // for some reason, x is flipped
                }
            frameTexture.Apply();
            gifFrames.Add(frameTexture);
        }
    }

    private void Start()
    {
        Screen.SetResolution(1280, 720, false);
        CBaseData.strPlayVideoName = "";
        CBaseData.strPlayVideoTitle = "";
        CBaseData.strVideoType = "";
        CBaseData.strPlayPictureName = "";
        CBaseData.strPlayPictureTitle = "";
        CBaseData.strPlayPictureCategory = "";

        if (!File.Exists(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, DataManager.Instance.ConfigFile))) 
        {
            MergeConfigFile config = new MergeConfigFile();
            config.Init(System.IO.Path.Combine(System.IO.Directory.GetParent(Application.dataPath).FullName, "pre_resource"));
        }
        if (!Directory.Exists(_folder))
        {
            Directory.CreateDirectory(_folder);
        }
        Debug.Log("Start:Load xml");
        DataManager.Instance.CreateDirectory();
        XmlManager.Instance.LoadXml();
        DataManager.Instance.Init();
        fnLoadVideoDataOver();
        fnLoadAppDataOver();
        fnLoadPicDataOver();
        fnLoadClientDataOver();
        //Read company logo
        StartCoroutine(LoadLogoTexture(_folder + "/broadcast/LogoTex/", strLogoName));
        //Start socket
        InitSocket();
        InitDropdownOption();
    }

    #region Path processing

    /// <summary>
    /// Get local path address
    /// </summary>
    private string GetUrl(string path)
    {
        string url = null;
        try
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.Android:
                    if (path.Contains(@"jar:file://"))
                        url = path;
                    else
                        url = @"file://" + path;
                    break;

                case RuntimePlatform.OSXEditor:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.WindowsEditor:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.OSXPlayer:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.WindowsPlayer:
                    url = @"file://" + path;
                    break;
            }

            return url.Replace("\\","/");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    /// <summary>
    /// Get program external address
    /// </summary>
    public string GetFilePath(string sFolder, string sFileName)
    {
        string filePath = Path.Combine(sFolder, sFileName);

        if (_useStreamingAssetsPath)
        {
            filePath = Path.Combine(Application.streamingAssetsPath, filePath);
        }
        // If we're running outside of the editor we may need to resolve the relative path
        // as the working-directory may not be that of the application EXE.
        else if (!Application.isEditor && !Path.IsPathRooted(filePath))
        {
            string rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            filePath = Path.Combine(rootPath, filePath);
        }
        return filePath.Replace("\\","/");
    }

    #endregion Path processing

    #region Client processing

    /// <summary>
    /// Client data processing
    /// </summary>
    private void fnLoadClientDataOver()
    {
        CClientView view = gaClientUIFather.GetComponent<CClientView>();
        view.LoadDataOver(DataManager.Instance.ClientDatas);
        foreach (var info in DataManager.Instance.ClientDatas)
        {
            if (gaClientUI)
            {
                //GameObject ga = Instantiate(gaClientUI, gaClientUI.transform.position, gaClientUI.transform.rotation);
                //ga.name = info.id;
                //gaClientList.Add(ga);
                //if (gaClientUIFather)
                //    ga.transform.SetParent(gaClientUIFather.transform, false);

                ////Data assignment
                //CSetClientData cSetClient = ga.GetComponent<CSetClientData>();
                //cSetClient.fnSetID(info.id);
                //cSetClient.fnSetIP(info.sn);
                //cSetClient.fnCategory = info.category;
                //cSetClient.fnSetStu(PlayStatus.Free.ToString());
                bool connect = false;
                string sensor = "0";
                for (int i = 0; i < Server.Instance.clientList.Count; i++)
                {
                    clientStatus status = Server.Instance.clientList[i];
                    if (status.sn == info.sn)
                    {
                        connect = true;
                        sensor = status.sensor;
                        break;
                    }
                }
                //cSetClient.fnSetImage(connect, sensor);
                //if (connect)
                //{
                //    cSetClient.fnSetStu(server.IsFree ? PlayStatus.Free.ToString() : PlayStatus.Ready.ToString());
                //}
                //else
                //{
                //    cSetClient.fnSetStu(PlayStatus.Free.ToString());
                //}
                //Video playback use
                GameObject gaPlay = Instantiate(gaClientUI, gaClientUI.transform.position, gaClientUI.transform.rotation);
                gaPlay.name = info.id;
                gaClientListPlaying.Add(gaPlay);
                gaPlay.transform.SetParent(gaClientUIPlayFather.transform, false);

                //Data assignment
                CSetClientData cSetClientPlay = gaPlay.GetComponent<CSetClientData>();
                cSetClientPlay.fnSetData(info);
                cSetClientPlay.fnSetID(info.id);
                cSetClientPlay.fnSetIP(info.sn);
                cSetClientPlay.fnCategory = info.category;
                cSetClientPlay.fnSetImage(connect, sensor);
                if (connect)
                {
                    cSetClientPlay.fnSetStu(Server.Instance.IsFree ? PlayStatus.Free.ToString() : PlayStatus.Ready.ToString());
                }
                else {

                    cSetClientPlay.fnSetStu(PlayStatus.Free.ToString());
                }
                //Add ID list
                UserIdList.Add(info.id);
            }
        }
        //Get total number of clients
        CBaseData.nMaxClient = DataManager.Instance.ClientDatas.Count;
        //Update dropdown
        Drd_IDList.AddOptions(UserIdList);
        fnSetClietSynInit();
    }

    #endregion Client processing

    #region Video list processing
    /// <summary>
    /// Video data processing
    /// </summary>
    private void fnLoadVideoDataOver()
    {
        CVideoView view = gaVideoUIFather.GetComponent<CVideoView>();
        view.LoadDataOver(DataManager.Instance.VideoDatas);
    }


    #endregion Video list processing

    #region Application list processing

    /// <summary>
    /// Application side data processing
    /// </summary>
    private void fnLoadAppDataOver()
    {
        CAppView view = gaAppUIFather.GetComponent<CAppView>();
        view.LoadDataOver(DataManager.Instance.AppDatas);
    }
    #endregion Application list processing

    #region Picture list processing

    private void fnLoadPicDataOver()
    {
        CPictureView view = gaPicUIFather.GetComponent<CPictureView>();
        view.LoadDataOver(DataManager.Instance.PictureDatas);
    }
    #endregion Image processing

    #region Dynamically load company logo

    private IEnumerator LoadLogoTexture(string sF, string strSortName)
    {
        string str = GetUrl(GetFilePath(sF, strLogoName));
        WWW www = new WWW(str);
        yield return www;
        if (www.isDone)
        {
            if (www.error == null || www.error == "")
            {
                Texture2D tex = www.texture;
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                if (imLogo)
                    imLogo.sprite = temp;
            }
            else
            {
                Texture2D tex = (Texture2D)Resources.Load("logo");
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                if (imLogo)
                    imLogo.sprite = temp;
                Debug.Log("LoadLogoTexture:" + www.error);
            }
        }
    }

    #endregion Dynamically load company logo

    #region Client synchronization selection

    /// <summary>
    /// Selection of user ID synchronization
    /// </summary>
    /// <param name="nIndex"></param>
    public void fnDropUserIdSelect(int nIndex)
    {
        //Direct return in free mode
        if (Server.Instance.fnGetServerStatus().Equals("Free"))
            return;

        //Debug.Log(" fnDropUserIdSelect:Start synchronizing clients .... " + nIndex);
        //Stop synchronization
        if (nIndex == 0)
            Server.Instance.fnStopOrStartSynchroClient(strLastSynSn, false);
        else
        {
            //Stop synchronizing last object
            if (!strLastSynSn.Equals(""))
                Server.Instance.fnStopOrStartSynchroClient(strLastSynSn, false);

            //Debug.Log(" fnDropUserIdSelect:Sync client .... " + nIndex);
            if (nIndex != 0) {
                ClientData info = DataManager.Instance.ClientDatas[nIndex - 1];
                strLastSynSn = info.sn;
                CBaseData.strSynClientSN = info.sn;
                Server.Instance.fnStopOrStartSynchroClient(info.sn, true);
            }
            //foreach (var info in DataManager.Instance.ClientDatas)
            //{
            //    //Debug.Log("fnDropUserIdSelect:Sync client sequence number .... " + int.Parse(info.id));
            //    //Start synchronizing corresponding clients
            //    if (nIndex == int.Parse(info.id))
            //    {
            //        Debug.Log(""fnDropUserIdSelect:Sync client .... " + info.sn);
            //        strLastSynSn = info.sn;
            //        CBaseData.strSynClientSN = info.sn;
            //        Server.Instance.fnStopOrStartSynchroClient(info.sn, true);
            //    }
            //}
        }
    }

    #endregion Client synchronization selection

    #region Client UI status change display

    /// <summary>
    /// After the client connects, set the client status display according to the SN number
    /// </summary>
    /// <param name="strSN"></param>
    public void fnSetClientStatus(string strSN, bool IsConnect,string sensor= "")
    {
        string id = fnGetClientID(strSN);
        if (!id.Equals(""))
        {
            CClientView view = gaClientUIFather.GetComponent<CClientView>();
            foreach (var ga in view.mList)
            {
                if (ga.name.Equals(id))
                {
                    ga.GetComponent<CSetClientData>().fnSetImage(IsConnect, sensor);
                    break;
                }
            }
            foreach (var ga in gaClientListPlaying)
            {
                if (ga.name.Equals(id))
                {
                    ga.GetComponent<CSetClientData>().fnSetImage(IsConnect, sensor);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Return the number of the client according to the SN number
    /// </summary>
    /// <param name="strSN"></param>
    /// <returns></returns>
    private string fnGetClientID(string strSN)
    {
        foreach (var info in DataManager.Instance.ClientDatas)
        {
            if (info.sn.Equals(strSN))
                return info.id;
        }
        return "";
    }

    /// <summary>
    /// Obtain whether the client accepts the server instruction according to the SN number
    /// </summary>
    /// <param name="strSN"></param>
    /// <returns></returns>
    public bool fnGetClientControl(string strSN)
    {
        //string id = fnGetClientID(strSN);
        //if (!id.Equals(""))
        //{
        //    CClientView view = gaClientUIFather.GetComponent<CClientView>();
        //    foreach (var ga in view.mList)
        //    {
        //        if (ga.name.Equals(id))
        //        {
        //            return ga.GetComponent<CSetClientData>().fnGetIsOreder();
        //        }
        //    }
        //}
        List<ClientData> list = DataManager.Instance.ClientDatas;
        for (int i = 0; i < list.Count; i++)
        {
            ClientData data = list[i];
            if (data.sn == strSN) return data.accept;
        }
        return false;
    }

    /// <summary>
    /// Set the status information of the client according to the SN number
    /// </summary>
    /// <param name="strSN"></param>
    /// <param name="strStatus"></param>
    public void fnSetClientMessage(string strSN, string strStatus)
    {
        string id = fnGetClientID(strSN);
        if (!id.Equals(""))
        {
            CClientView view = gaClientUIFather.GetComponent<CClientView>();
            foreach (var ga in view.mList)
            {
                if (ga.name.Equals(id))
                {
                    ga.GetComponent<CSetClientData>().fnSetStu(strStatus);
                    break;
                }
            }
            foreach (var ga in gaClientListPlaying)
            {
                if (ga.name.Equals(id))
                {
                    //Debug.Log(" fnSetClientMessage:Update client Message .... ID =  " + id + " " + strStatus);
                    ga.GetComponent<CSetClientData>().fnSetStu(strStatus);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Set the power status information of the client according to the SN number
    /// </summary>
    /// <param name="strSN"></param>
    /// <param name="strStatus"></param>
    public void fnSetClientElectric(string strSN, string strStatus, bool IsBattering)
    {
        string id = fnGetClientID(strSN);
        if (!id.Equals(""))
        {
            CClientView view = gaClientUIFather.GetComponent<CClientView>();
            foreach (var ga in view.mList)
            {
                if (ga.name.Equals(id))
                {
                    ga.GetComponent<CSetClientData>().fnSetElectric(int.Parse(strStatus), IsBattering);
                    break;
                }
            }
            foreach (var ga in gaClientListPlaying)
            {
                if (ga.name.Equals(id))
                {
                    ga.GetComponent<CSetClientData>().fnSetElectric(int.Parse(strStatus), IsBattering);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Select all or deselect all clients
    /// </summary>
    /// <param name="IsControl"></param>
    public void fnSetClientToggle()
    {
        IsAll = !IsAll;
        CClientView view = gaClientUIFather.GetComponent<CClientView>();
        foreach (var ga in view.mList)
        {
            ga.GetComponent<CSetClientData>().fnSetToggleClient(IsAll);
        }
        foreach (var ga in gaClientListPlaying)
        {
            ga.GetComponent<CSetClientData>().fnSetToggleClient(IsAll);
        }
    }

    /// <summary>
    /// Set client synchronization ID initialization
    /// </summary>
    public void fnSetClietSynInit()
    {
        Drd_IDList.value = 0;
    }

    /// <summary>
    /// Server issues all shutdown instructions
    /// </summary>
    public void fnOffClientAll()
    {
        SocketSend("close");

        foreach (var info in GetComponent<Server>().clientList)
        {
            //Accept server instructions or not
            bool IsAccept = fnGetClientControl(info.sn);
            if (IsAccept)
            {
                //Set the client's state to the initial state
                GetComponent<CLoadJson>().fnSetClientStatus(info.sn, false);
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, PlayStatus.Free.ToString());
            }
        }
    }
    public void fnRebootClientAll()
    {
        SocketSend("reboot");

        foreach (var info in GetComponent<Server>().clientList)
        {
            //Accept server instructions or not
            bool IsAccept = fnGetClientControl(info.sn);
            if (IsAccept)
            {
                //Set the client's state to the initial state
                GetComponent<CLoadJson>().fnSetClientStatus(info.sn, false);
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, PlayStatus.Free.ToString());
            }
        }
    }


    #endregion Client UI status change display

    #region Refresh video app client list

    /// <summary>
    /// Refresh video app client list
    /// </summary>
    public void RefreshList()
    {
        Drd_IDList.ClearOptions();
        UserIdList.Clear();
        InitDropdownOption();

        foreach (GameObject obj in gaClientListPlaying) Destroy(obj);
        gaClientListPlaying.Clear();
        //XmlManager.Instance.LoadXml();
        //DataManager.Instance.Init();
        fnLoadVideoDataOver();
        fnLoadAppDataOver();
        fnLoadPicDataOver();
        fnLoadClientDataOver();
    }

    /// <summary>
    /// Every time you refresh the scroll bar, it will return to its original position
    /// </summary>
    /// <param name="go"></param>
    private void defaultScrollBarPosition(GameObject go)
    {
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, 0, 0);
    }

    #endregion Refresh video app client list
    private void InitDropdownOption()
    {
        UserIdList.Add("None");
    }

    private void OnDestroy()
    {
        DataManager.Instance.SaveVolume();
        Debug.Log("OnDestroy:Write system data to JSON");
    }
}
