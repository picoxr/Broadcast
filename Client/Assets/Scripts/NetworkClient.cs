using UnityEngine;

public class NetworkClient : MonoBehaviour
{
    private bool m_connected;

    public delegate void OnNetStateChanged(bool connected);

    public delegate void OnNetworkCommand(string command, string value);

    public OnNetworkCommand onNetworkCommand;
    public OnNetStateChanged onNetStateChanged;
    public bool isConnected
    {
        get { return m_connected; }
    }

    private NetworkView networkView;

    public void SendToServerMessage(string command, string parameter)
    {
        networkView.RPC("RequestMessage", RPCMode.Server, command, parameter);
    }

    void Awake()
    {
        networkView = gameObject.AddComponent<NetworkView>();
        InvokeRepeating("CheckConnection", 2, 1.5f);
    }

    private void CheckConnection()
    {
        if (m_connected != (Network.peerType == NetworkPeerType.Client))
        {
            NetStateChanged(!m_connected);
        }

        Debug.Log("Checkconnection:" + Network.peerType + AndroidTools.serverIp + "   " + AndroidTools.serverPort);
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            NetworkConnectionError err = Network.Connect(AndroidTools.serverIp, AndroidTools.serverPort);
        }
    }

    private void OnConnectedToServer()
    {
        NetStateChanged(true);
    }

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (info == NetworkDisconnection.LostConnection)
        {
            NetStateChanged(false);
        }
    }

    private void NetStateChanged(bool connected)
    {
        m_connected = connected;
        if(onNetStateChanged != null) onNetStateChanged(connected);

        string sn = AndroidTools.serialNumber;
        print(sn + (m_connected ? " connected to server." : " disconnected from server."));

        if (connected)
        {
            int status = 0;
            PicoUnityActivity.CallObjectMethod<int>(ref status, "getSensorStatus");
            SendToServerMessage("connect", Utility.Concat(sn,status));
        }
    }

    [RPC]
    private void RequestMessage(string command, string parameter, NetworkMessageInfo info)
    {
        string message = command;
        if (!string.IsNullOrEmpty(parameter))
        {
            message += ", " + parameter;
        }

        print("Message: " + message);

        onNetworkCommand(command, parameter);
    }
}