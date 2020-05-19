using UnityEngine;

public class AndroidMessageHandler : MonoBehaviour
{
    private void Update()
    {
        if (NetManager.network == null)
        {
            return;
        }

        if (Status.needSyncBattery && NetManager.network.isConnected)
        {
            NetManager.network.SendToServerMessage("battery", Utility.Concat(AndroidTools.serialNumber, Status.batteryLevel, Status.isCharging ? 1 : 0));
            Status.needSyncBattery = false;
        }
    }

    public void Android_SendTimeToUnity(string time)
    {
        Status.time = time;
        Main.statusBar.UpdateTime();
    }

    public void Android_SendBatteryInfo(string info)
    {
        string[] arr = info.Split('#');
        Status.isCharging = int.Parse(arr[0]) == 2;
        Status.batteryNumber = int.Parse(arr[1]);
        Main.statusBar.UpdateBatteryStatus(Status.isCharging, Status.batteryLevel);
    }

    public void Android_SendWifiConnectedToUnity(string connected)
    {
        Status.wifiStatus.isConnected = (connected == "1");
        Main.statusBar.UpdateWifiStatus(Status.wifiStatus);
    }

    public void Android_SendWifiStatusToUnity(string state)
    {
        Status.wifiStatus.switchStatus = (WifiSwitchStatus)int.Parse(state);
        Main.statusBar.UpdateWifiStatus(Status.wifiStatus);
    }

    public void Android_SendWifiLevelToUnity(string level)
    {
        Status.wifiStatus.level = int.Parse(level);
        Main.statusBar.UpdateWifiStatus(Status.wifiStatus);
    }

    public void Android_SendSensorStatusChange(string value)
    {
        if (!string.IsNullOrEmpty(AndroidTools.serialNumber))
        {
            NetManager.network.SendToServerMessage("sensor", Utility.Concat(AndroidTools.serialNumber, value));
        }
    }
}