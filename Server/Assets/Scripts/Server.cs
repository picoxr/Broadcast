using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class clientStatus
{
    public string sn;
    public NetworkPlayer netPlay;
    public string sensor;
}

public class Server : MonoBehaviour
{

    public static Server Instance;
    PlayStatus currentStatus = PlayStatus.Free;
    float alreadyPlayedTime = 0.0f;
    int port = 8081;
    NetworkView networkView;
    int length = 0;
    public List<clientStatus> clientList = new List<clientStatus>();

    //UI:Number of clients 
    public Text texClientCount;

    //Free mode or not
    public Text texFree;
    public GameObject imFree, imIdel;
    public bool IsFree = true;
    public static Action<bool> ChangeModel;
    public static Action<int> ChangeVolume;
    public int ClientVolume = 50;
    //Whether to turn on synchronization
    private bool IsSynCamera = false;

    //Use camera to synchronizing status of target
    private Quaternion vecCameraPos;

    //Historical data dictionary
    public string History = "history.csv";
    private Dictionary<int, List<string>> data = null;

    private List<int> OutSyncList = new List<int>();

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        networkView = gameObject.AddComponent<NetworkView>();

        if (IsFree)
        {
            currentStatus = PlayStatus.Free;
            imFree.SetActive(true);
            imIdel.SetActive(false);
            //texFree.text = "standby mode";
        }
        else
        {
            currentStatus = PlayStatus.Ready;
            imFree.SetActive(false);
            imIdel.SetActive(true);
            //texFree.text = "Free mode";
        }

        //Turn on server automatically
        fnStartServer();

        //Load historical data
        StartCoroutine(LoadCSVDataLoading());
    }

    void FixedUpdate()
    {
        length = Network.connections.Length;
        texClientCount.text = ": " + length + "/" + CBaseData.nMaxClient;

        //If it's synchronous
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    //Get current screen resolution
        //    Resolution[] resolutions = Screen.resolutions;
        //    //Set current resolution
        //    Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);

        //    Screen.fullScreen = true;  //Set to full screen,
        //}
        //else if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    Screen.SetResolution(1280, 720, false);
        //}
        //else if (Screen.width == Screen.currentResolution.width && !Screen.fullScreen)
        //{
        //    //Get current screen resolution
        //    Resolution[] resolutions = Screen.resolutions;
        //    //Set current resolution
        //    Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);

        //    Screen.fullScreen = true;  //Set to full screen
        //}
    }

    /// <summary>
    /// Start Server
    /// </summary>
    public void fnStartServer()
    {
        bool useNat = !Network.HavePublicAddress();
        Network.InitializeServer(1000, port, false);
    }

    /// <summary>
    /// Disconnect server
    /// </summary>
    public void fnStopServer()
    {
        Network.Disconnect();
        alreadyPlayedTime = 0;
        currentStatus = PlayStatus.Free;
    }

    /// <summary>
    /// Play video
    /// </summary>
    public void fnStartVideo(string videoType, string videoName)
    {
        string strOrder = videoType;
        currentStatus = PlayStatus.Pause;
        Debug.Log("fnPlayVideo:" + clientList.Count);
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
            {
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                networkView.RPC("RequestMessage", info.netPlay, strOrder,videoName);
            }
        }
    }

    /// <summary>
    ///  Play video and get video's current status
    /// </summary>
    public void fnPlayVideo(long playTime = 0)
    {
        playTime = (long)Math.Round(playTime * 1.0f/10000);
        string strOrder = "playing";
        if (currentStatus == PlayStatus.Ready)
            strOrder = "playing";
        else if (currentStatus == PlayStatus.Pause)
        {
            strOrder = "continue";
            alreadyPlayedTime = playTime;
        }
        currentStatus = PlayStatus.Playing;
        Debug.Log("fnPlayVideo:" + clientList.Count + " time:" + playTime +" order:" + strOrder);
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            Debug.Log("fnPlayVideo:" + info.sn + IsAccept);
            if (IsAccept)
            {
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                networkView.RPC("RequestMessage", info.netPlay, strOrder, CBaseData.strPlayVideoTitle + ":" + alreadyPlayedTime.ToString() + ":" + CBaseData.strVideoType + ":" + CBaseData.strLinkModel);
            }
        }
    }

    public void fnPlayPlayingVideo(long playTime = 0)
    {
        playTime = (long)Math.Round(playTime * 1.0f / 10000);
        string strOrder = "playing";
        currentStatus = PlayStatus.Playing;
        Debug.Log(" fnPlayPlayingVideo:" + clientList.Count + " time:" + playTime + " order:" + strOrder);
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            Debug.Log(" fnPlayPlayingVideo:" + info.sn + IsAccept);
            if (IsAccept)
            {
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                networkView.RPC("RequestMessage", info.netPlay, strOrder, CBaseData.strPlayVideoTitle + ":" + alreadyPlayedTime.ToString() + ":" + CBaseData.strVideoType + ":" + CBaseData.strLinkModel);
            }
        }
    }

    /// <summary>
    /// Pause video
    /// </summary>
    public void fnPauseVideo()
    {
        currentStatus = PlayStatus.Pause;
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
            {
                  networkView.RPC("RequestMessage", info.netPlay, "Pause", "");
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
            }
        }

    }
    /// <summary>
    ///  Continue playing
    /// </summary>
    public void fnCountinueVideo()
    {
        Debug.Log("fnCountinueVideo:Countiue");
        currentStatus = PlayStatus.Playing;
        foreach (var info in clientList)
        {
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
            {
                Debug.Log("fnCountinueVideo:Countiue");
                networkView.RPC("RequestMessage", info.netPlay, "Continue", "");
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
            }
        }

    }

    /// <summary>
    /// Video reset or stop
    /// </summary>
    public void fnStopVideo()
    {
        currentStatus = PlayStatus.Ready;
        alreadyPlayedTime = 0;
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
            {
                 networkView.RPC("RequestMessage", info.netPlay, "Stop", " ");
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
            }

            //Is synchronization enabled? If Synchronization enabled, it needs to be closed
            if (IsSynCamera)
            {
                fnStopOrStartSynchroClient(CBaseData.strSynClientSN, false);
                GetComponent<CLoadJson>().fnSetClietSynInit();
            }
        }

    }

    /// <summary>
    /// Play picture
    /// </summary>
    public void fnPlayPicture()
    {
        currentStatus = PlayStatus.PlayGallery;
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            Debug.Log("fnPlayPicture:" + info.sn + IsAccept);
            if (IsAccept)
            {
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                networkView.RPC("RequestMessage", info.netPlay, "playGallery", CBaseData.strPlayPictureTitle);
            }
        }
    }
    /// <summary>
    /// Stop picture
    /// </summary>
    public void fnStopPicture()
    {
        currentStatus = PlayStatus.Ready;
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
            {
                networkView.RPC("RequestMessage", info.netPlay, "reset", "");
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
            }

            //Is synchronization enabled? If Synchronization enabled, it needs to be closed
            if (IsSynCamera)
            {
                fnStopOrStartSynchroClient(CBaseData.strSynClientSN, false);
                GetComponent<CLoadJson>().fnSetClietSynInit();
            }
        }
    }

    /// <summary>
    /// Set free mode
    /// </summary>
    public void fnSetClientFreeOrIdle()
    {
        IsFree = !IsFree;

        if (IsFree)
        {
            currentStatus = PlayStatus.Free;
            //texFree.text = "standby mode";
            imFree.SetActive(true);
            imIdel.SetActive(false);
        }
        else
        {
            CLoadJson.Instance.SocketSend("tohome");
            currentStatus = PlayStatus.Ready;
            //texFree.text = "Free mode";
            imFree.SetActive(false);
            imIdel.SetActive(true);
        }
        if (ChangeModel != null) ChangeModel(IsFree); 
        alreadyPlayedTime = 0;
        Debug.Log("fnSetClientFreeOrIdle:clientList:" + clientList.Count);
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            Debug.Log("fnSetClientFreeOrIdle:clientList:" + info.sn + " isAccept:" + IsAccept);
            if (IsAccept)
            {
                if (IsFree)
                {
                    networkView.RPC("RequestMessage", info.netPlay, "free", "");
                    //Set the status of the client
                    GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                }
                else
                {
                    networkView.RPC("RequestMessage", info.netPlay, "ready", "");
                    //Set the status of the client
                    GetComponent<CLoadJson>().fnSetClientMessage(info.sn, currentStatus.ToString());
                    networkView.RPC("RequestMessage", info.netPlay, "volume", ClientVolume.ToString());

                }
            }
        }

    }

    /// <summary>
    /// Synchronize or stop clients
    /// </summary>
    /// <param name="strSn"></param>
    /// <param name="IsSynchro"></param>
    public void fnStopOrStartSynchroClient(string strSn, bool IsSynchro)
    {
        //Debug.Log("fnStopOrStartSynchroClient:Server sends synchronization instruction11111111.... " + strSn);
        IsSynCamera = IsSynchro;
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept && strSn.Equals(info.sn))
            {
                //Debug.Log("fnStopOrStartSynchroClient:Server sends synchronization instruction22222222222.... " + info.sn);
                //Synchronization instruction
                networkView.RPC("RequestMessage", info.netPlay, "synchro", IsSynchro.ToString());
            }
        }

    }

    /// <summary>
    /// Server seek to client
    /// </summary>
    public void fnServerSeekToClient()
    {
        foreach (var info in clientList)
        {
            //Accept server instructions or not
            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
            if (IsAccept)
                //Seek instruction
                networkView.RPC("RequestMessage", info.netPlay, "seek", CBaseData.strSeekTime);
        }
    }

    public void fnSetVolume(int volume)
    {
        if (!IsFree) {
            ClientVolume = volume;
            foreach (var info in clientList)
            {
                //Accept server instructions or not
                bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(info.sn);
                if (IsAccept)
                {
                    networkView.RPC("RequestMessage", info.netPlay, "volume", volume.ToString());
                }
            }
        }
    }

    ///// <summary>
    ///// Server issues all shutdown instructions
    ///// </summary>
    //public void fnOffClientAll()
    //{
    //    networkView.RPC("RequestMessage", RPCMode.All, "close", "");

    //    foreach (var info in clientList)
    //    {
    //        //Set the client's state to the initial state
    //        GetComponent<CLoadJson>().fnSetClientStatus(info.sn, false);
    //        GetComponent<CLoadJson>().fnSetClientMessage(info.sn, PlayStatus.Free.ToString());
    //    }
    //}


    /// <summary>When a player is connected</summary>
	void OnPlayerConnected(NetworkPlayer player)
    {
        //string command = "ready";
        //switch (currentStatus)
        //{
        //    case PlayStatus.Playing:
        //        command = "playing";
        //        break;
        //    case PlayStatus.Pause:
        //        command = "playing-pause";
        //        break;
        //    case PlayStatus.Free:
        //        command = "free";
        //        break;
        //    default:
        //        command = "ready";
        //        break;
        //}

        //networkView.RPC("RequestMessage",
        //            player,
        //            command,
        //            CBaseData.strPlayVideoName + ":" + alreadyPlayedTime.ToString());
    }


    /// <summary>When a player is disconnected</summary>
    void OnPlayerDisconnected(NetworkPlayer netWorkPlayer)
    {
        Debug.Log("OnPlayerDisconnected ..... " + netWorkPlayer.ipAddress + " " + netWorkPlayer.guid);
        for (int i = clientList.Count - 1; i >= 0; i--)
        {
            if (clientList[i].netPlay.ipAddress.Equals(netWorkPlayer.ipAddress) && clientList[i].netPlay.guid.Equals(netWorkPlayer.guid))
            {
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientStatus(clientList[i].sn, false);
                GetComponent<CLoadJson>().fnSetClientMessage(clientList[i].sn, PlayStatus.Free.ToString());
                 //Delete clientlist
                clientList.Remove(clientList[i]);
            }
        }
    }

    [RPC]
    void RequestMessage(string command, string value, NetworkMessageInfo info)
    {
        Debug.Log("RequestMessage:command:" + command + "   value:" + value + " " + info.sender.guid);

        //The client sends its own Sn number to the server after connecting
        switch (command)
        {
            case "connect":
                { 
                    string[] str = value.Split('|');
                    //Check for presence
                    for (int i = clientList.Count - 1; i >= 0; i--)
                    {
                        if (clientList[i].sn.Equals(str[0]))
                        {
                            clientList.RemoveAt(i);
                            break;
                        }
                    }
                    clientStatus client = new clientStatus();
                    client.netPlay = info.sender;
                    client.sn = str[0];
                    client.sensor = str[1];
                    clientList.Add(client);
                    Debug.Log("RequestMessage:Add new user ..... " + client.sn + " ip: " + client.netPlay.ipAddress);
                    //Set the status of the client
                    GetComponent<CLoadJson>().fnSetClientStatus(str[0], true,str[1]);
                    //Accept server instructions or not
                    bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(str[0]);
                    if (IsAccept)
                    {
                        fnSetConnectClientStatus(str[0], info.sender);
                        //DataManager.Instance.SynchronousConfig(info.sender);
                    }
                    if (!IsFree && IsAccept)
                    {
                        networkView.RPC("RequestMessage", info.sender, "volume", ClientVolume.ToString());
                    }
                }
                break;

            case "syncCamera":
                if (IsSynCamera)
                {
                    string[] rotations = value.Split('|');

                    
                    //Vector3 vect3 = new Vector3(float.Parse(rotations[0]), float.Parse(rotations[1]) + 90.0f, float.Parse(rotations[2]));
                    Vector3 vect3 = new Vector3(float.Parse(rotations[0]), float.Parse(rotations[1]), float.Parse(rotations[2]));
                    vecCameraPos = Quaternion.Euler(vect3);

                    //Vector3 vect3 = new Vector3(float.Parse(rotations[0]), float.Parse(rotations[1]), float.Parse(rotations[2]));
                    //Camera.main.transform.rotation = Quaternion.Euler(vect3);
                }
                break;

            //Client re request progress
            case "resume":
                {
                    for (int i = clientList.Count - 1; i >= 0; i--)
                    {
                        if (clientList[i].sn.Equals(value))
                        {
                            bool IsAccept = GetComponent<CLoadJson>().fnGetClientControl(value);
                            if (IsAccept) fnSetConnectClientStatus(value, info.sender);
                            break;
                        }
                    }
                }
                break;

            //Client sends power information
            case "battery":
                string[] electric = value.Split('|');
                int n = int.Parse(electric[2]);
                bool IsB = (n == 0) ? false : true;
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientElectric(electric[0], electric[1], IsB);
                break;

            //Client sends playback record
            case "history":
                LoadSaveCSVData.SaveCSVData(value, LoadSaveCSVData.GetFilepath(History), data);
                break;
            case "sensor":
                string[] sensor = value.Split('|');
                GetComponent<CLoadJson>().fnSetClientStatus(sensor[0], true, sensor[1]);
                for (int i = clientList.Count - 1; i >= 0; i--)
                {
                    if (clientList[i].sn.Equals(sensor[0]))
                    {
                        clientList[i].sensor = sensor[1];
                        break;
                    }
                }
                break;
            case "duration":
                long duration = 0;
                long.TryParse(value,out duration);
              
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Command returned by the server after the client connects
    /// </summary>
    /// <param name="player"></param> 
    void fnSetConnectClientStatus(string strSN, NetworkPlayer player)
    {
        string command = "free";
        long fSeek = 0;
        switch (currentStatus)
        {
            case PlayStatus.Playing:
                command = "playing";
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(strSN, currentStatus.ToString());
               
              
                Debug.Log("fnSetConnectClientStatus:" + fSeek);
                networkView.RPC("RequestMessage",player,command,CBaseData.strPlayVideoTitle + ":" + fSeek.ToString() + ":" + CBaseData.strVideoType + ":" + CBaseData.strLinkModel);
                break;
            case PlayStatus.Pause:
                command = "playing-pause";
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(strSN, currentStatus.ToString());
          
              
                networkView.RPC("RequestMessage", player, command, CBaseData.strPlayVideoTitle + ":" + fSeek.ToString() + ":" + CBaseData.strVideoType + ":" + CBaseData.strLinkModel);
                break;
            case PlayStatus.Free:
                command = "free";
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(strSN, currentStatus.ToString());
                networkView.RPC("RequestMessage", player, command,"");
                break;
            case PlayStatus.PlayGallery:
                command = "playGallery";
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(strSN, currentStatus.ToString());
                networkView.RPC("RequestMessage", player, command, CBaseData.strPlayPictureTitle + ":" + CBaseData.strPlayPictureCategory);
                break;
            default:
                command = "ready";
                //Set the status of the client
                GetComponent<CLoadJson>().fnSetClientMessage(strSN, currentStatus.ToString());
                networkView.RPC("RequestMessage", player, command,"");
                break;
        }
        //GetComponent<CLoadJson>().SocketSend("gotoapp" + CBaseData.strPlayAppName);
    }


    /// <summary>Send data to client</summary>
    public void SendData(string command, string value)
    {
        if (command != null)
        {
            if (command.Equals("play"))
            {
                currentStatus = PlayStatus.Playing;
                alreadyPlayedTime = 0;
            }
            else if (command.Equals("pause"))
            {
                currentStatus = PlayStatus.Pause;
            }
            else if (command.Equals("stop"))
            {
                currentStatus = PlayStatus.Ready;
                alreadyPlayedTime = 0;
            }
            else if (command.Equals("free"))
            {
                currentStatus = PlayStatus.Free;
                alreadyPlayedTime = 0;
            }
        }
        networkView.RPC("RequestMessage", RPCMode.All, command, value);
        //Debug.LogError("SendData:commone:" + command + " value:" + value);
    }

    public void SendData(NetworkPlayer player,string command, string value)
    {
        networkView.RPC("RequestMessage", player, command, value);
    }

    /// <summary>Get the number of clients online</summary>
    public int GetOnlineCount
    {
        get
        {
            length = Network.connections.Length;
            return length;
        }
    }

    //Returns the current server status
    public string fnGetServerStatus()
    {
        return currentStatus.ToString();
    }

    //Server exit
    public void fnServerQuit()
    {
        fnStopVideo();
        fnStopServer();
        Invoke("fnQuit", 0.2f);
    }

    void fnQuit()
    {
        Application.Quit();
    }
    /// <summary>  
    /// /Read documents in CSV format  
    /// </summary>  
    /// <returns></returns>  
    private IEnumerator LoadCSVData()
    {
        DateTime startTime = DateTime.Now;
        Debug.Log("LoadCSVData:Start reading data");
        LoadSaveCSVData loadcsv = gameObject.AddComponent<LoadSaveCSVData>();
        //Read skill sheet 
        yield return StartCoroutine(loadcsv.LoadCSV_toDirectory(History, (csvdata) => { data = csvdata; }));
        Debug.Log("LoadCSVData:Read history table cost:" + (DateTime.Now - startTime).TotalMilliseconds + "ms");
    }
    private IEnumerator LoadCSVDataLoading()
    {
        yield return StartCoroutine(LoadCSVData());
    }

    public void AddOutSyncSn(int sn)
    {
        OutSyncList.Add(sn);
    }

    public void RemoveOutSyncSn(int sn)
    {
        if (OutSyncList.Contains(sn)) OutSyncList.Remove(sn);
    }

    public bool OutSyncContain(int sn)
    {
        return OutSyncList.Contains(sn);
    }
}



public enum PlayStatus
{
    Ready,
    Playing,
    Pause,
    Free,
    PlayGallery,
}
