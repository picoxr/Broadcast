using System.Collections;
using UnityEngine;
using System.IO;
public class Main : MonoBehaviour
{
    static public StatusBar statusBar;
    static public bool mainUI = false;
    private static float m_synchro = -1f;
    static public ClinetControl clinetControl;
    public static bool synchro
    {
        set
        {
            m_synchro = value ? 0 : -1;
        }
    }
    public static bool isfreeMode = true;

    public static bool freeMode
    {
        set
        {
        
     
            AndroidTools.EnableKey(value);
            if (Pvr_ControllerManager.controllerlink != null)
            {
                Pvr_ControllerManager.controllerlink.SwitchHomeKey(value);
            } 
        }
    }

    private string m_configError;

    private const float k_synchroDuration = 0.3f;


    private void Awake()
    {
        clinetControl = GameObject.Find("Meun").GetComponent<ClinetControl>();
        statusBar = GameObject.Find("StatusBar").GetComponent<StatusBar>();
        AndroidTools.Initialize(ref m_configError);
    }

    private void Start()
    {
#if !UNITY_EDITOR
        PicoUnityActivity.CallObjectMethod("MergeFile","");
#else
        RefreshFiles("");
#endif
        freeMode = isfreeMode;
        NetManager.network.onNetStateChanged += OnNetStateChanged;
        AndroidMsgReciver.instance.Init();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && NetManager.network != null && NetManager.network.isConnected)
        {
            StartCoroutine(OnClientResume());
        }
    }

    private IEnumerator OnClientResume()
    {
        yield return new WaitForSeconds(0.5f);
        NetManager.network.SendToServerMessage("resume", AndroidTools.serialNumber);
    }

    private IEnumerator OpenFreeMode()
    {
        const float seconds = 1f;
        yield return new WaitForSeconds(seconds);
        // Start free mode if server is unreachable in 'seconds' seconds.
        freeMode = isfreeMode;
    }

    private void Update()
    {
        if (NetManager.network != null && NetManager.network.isConnected)
        {
            UpdateSynchro();
        }
    }

    private void UpdateSynchro()
    {
        if (m_synchro >= 0)
        {
            m_synchro -= Time.deltaTime;
            if (m_synchro < 0)
            {
                m_synchro = k_synchroDuration;
                Vector3 angles = Pvr_UnitySDKManager.SDK.HeadPose.Orientation.eulerAngles;
                NetManager.network.SendToServerMessage("syncCamera", Utility.Concat(angles.x, angles.y, angles.z));
            }
        }
    }

    private void OnNetStateChanged(bool connected)
    {
        statusBar.UpdateServerConnection(connected);
        if (!connected && !isfreeMode)
        {
            isfreeMode = true;
            StartCoroutine(OpenFreeMode());
            if (!AndroidMsgReciver.allowChangeVolume)
            {
                AndroidMsgReciver.allowChangeVolume = true;
                PicoUnityActivity.CallObjectMethod("setVolumeKeyEnabled", true);
            }
        }
    }


    public void RefreshFiles(string value)
    {
        bool isconfig=false;
        string path = "/storage/emulated/0/pre_resource/broadcast/config/BroadcastConfig.xml";
        string externalSDCardDir = Path.Combine(AndroidTools.externalSDCardDir, "pre_resource/broadcast/config/BroadcastConfig.xml").Replace("file://", "");
        if (System.IO.File.Exists(path) || System.IO.File.Exists(externalSDCardDir) )
        {
           isconfig = true;
        }
        statusBar.UpdateConfigStatus(isconfig);
        statusBar.UpdateServerConnection(false);
    }
}