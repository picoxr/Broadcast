using UnityEngine;

public enum WifiSwitchStatus
{
    WIFI_STATE_UNKNOWN = 4,
    WIFI_STATE_ENABLED = 3,
    WIFI_STATE_ENABLING = 2,
    WIFI_STATE_DISABLED = 1,
    WIFI_STATE_DISABLING = 0,
}

public struct WifiStatus
{
    public bool isConnected;
    public WifiSwitchStatus switchStatus;
    public int level;
}

public static class Status
{
    static public string time = "00:00";
    static public WifiStatus wifiStatus;

    static public bool isCharging
    {
        get { return m_isCharging; }
        set
        {
            if (m_isCharging == value) { return; }
            m_isCharging = value;
            needSyncBattery = true;
        }
    }

    static public bool needSyncBattery { get; set; }

    static public int batteryNumber
    {
        get
        {
            return m_batteryNumber;
        }
        set
        {
            if (m_batteryNumber == value)
            {
                return;
            }

            m_batteryNumber = value;
            int level = Mathf.Clamp(value / 20 + 1, 1, 5);

            needSyncBattery = true;
            m_batteryLevel = level;
        }
    }

    static public int batteryLevel
    {
        get
        {
            return m_batteryLevel;
        }
    }

    private static int m_batteryNumber = 0;
    private static int m_batteryLevel = 0;
    private static bool m_isCharging = false;
}