using UnityEngine;
using UnityEngine.UI;
public class StatusBar : MonoBehaviour
{
    public Image wifi;
    public Image server;
    public Image config;
    public Image battery;
    public Text timeNumber;
    public Text showMessage;
    private string path= "FileExplorer/";
    private void Start()
    {
        Refresh();
    }
    private void Update()
    {
        UpdateTime();
    }

    public void Refresh()
    {
        string wifiStatus = string.Empty;
        if (AndroidTools.GetWifiStatus(ref wifiStatus))
        {
            string[] arguments = wifiStatus.Split('#');
            Status.wifiStatus.isConnected = int.Parse(arguments[0]) != 0;
            Status.wifiStatus.switchStatus = (WifiSwitchStatus)int.Parse(arguments[1]);
            Status.wifiStatus.level = int.Parse(arguments[2]);
            UpdateWifiStatus(Status.wifiStatus);
        }

        string batteryStatus = string.Empty;
        if (AndroidTools.GetBatteryStatus(ref batteryStatus))
        {
            string[] arguments = batteryStatus.Split('#');
            Status.isCharging = int.Parse(arguments[0]) == 2;
            Status.batteryNumber = int.Parse(arguments[1]);
            UpdateBatteryStatus(Status.isCharging, Status.batteryLevel);
        }

        string time = string.Empty;
        if (AndroidTools.GetTime(ref time))
        {
            Status.time = time;
            UpdateTime();
        }
        Debug.Log("Refresh:UpdateServerConnection:1");
        UpdateServerConnection(NetManager.network.isConnected);
        Debug.Log("Refresh:UpdateServerConnection:2");
    }

    public void UpdateTime()
    {

        timeNumber.text = System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
    }

    public void UpdateWifiStatus(WifiStatus status)
    {
        if (status.switchStatus == WifiSwitchStatus.WIFI_STATE_ENABLED)
        {
            if (!status.isConnected)
            {
                wifi.sprite =Resources.Load(path+ "WF_07",typeof(Sprite))as Sprite;
            }
            else
            {
                wifi.sprite =Resources.Load(path+ "WF_0" + (2 + status.level),typeof (Sprite))as Sprite;
            }
        }
        else
        {
            wifi.sprite = Resources.Load(path + "WF_01", typeof(Sprite)) as Sprite;
        }
    }

    public void UpdateServerConnection(bool connected)
    {
        string sprite = connected ? "PC icon_02" : "PC icon_01";
        server.sprite =Resources.Load(path+sprite ,typeof (Sprite))as Sprite  ;
    }

    public void UpdateConfigStatus(bool ok)
    {
        string sprite = ok ? "File icon_02" : "File icon_01";
        config.sprite = Resources.Load(path + sprite, typeof(Sprite)) as Sprite;
        if (!ok)
        {
            showMessage.text = "Config not exist";
        }
     
    }

    public void UpdateBatteryStatus(bool charging, int level)
    {
        if (charging)
        {
            battery.sprite= Resources.Load(path + "DC_01", typeof(Sprite)) as Sprite;
        }
        else
        {
            battery.sprite = Resources.Load(path + "DC_0" + (1 + level), typeof(Sprite)) as Sprite;
        }
    }
}