using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : MonoBehaviour {


    static public NetworkClient network;
    private NetCommandHandler m_handler;
    static public bool block = false;
    // Use this for initialization

    void Awake()
    {
        network = GetComponent<NetworkClient>();
        m_handler = new NetCommandHandler();
        network.onNetworkCommand = OnNetworkCommand;
    }

    private void OnNetworkCommand(string command, string parameter)
    {
        StartCoroutine(WaitCommand(command, parameter));
    }

    IEnumerator WaitCommand(string command, string parameter)
    {
        yield return new WaitForEndOfFrame();
        while (block)
        {
            yield return new WaitForEndOfFrame();
        }
        m_handler.OnCommand(command, parameter);
    }
}
