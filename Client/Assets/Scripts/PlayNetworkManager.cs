using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNetworkManager : MonoBehaviour
{

    private static PlayNetworkManager _instance;
    private static PlayNetworkManager _instanceBit;
    private static readonly object locker = new object();
    private Transform _uiParent;

    [SerializeField]
    //private UILabel label;


    public static PlayNetworkManager GetInstance()
    {
        lock (locker)
        {
                if (_instance == null)
                {
                    GameObject obj = Resources.Load("UIPrefab/PlayNetworkManager") as GameObject;
                    GameObject go = Instantiate(obj);
                    _instance = go.GetComponent<PlayNetworkManager>();
                    //_instance._uiParent = UIRoot.list[0].transform;
                    go.transform.parent = _instance._uiParent;
                    go.transform.localPosition = new Vector3(0f, 100f, 0f);
                    go.transform.localScale = Vector3.one;
                    go.SetActive(false);
                }
                return _instance;
        }
    }


    public void OnNativeStateAction(int nativeState)
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //label.text = "Network exception, please check the network";
            //label.text = Localization.Get("Network_Retry");
            gameObject.SetActive(true);
            if (IsInvoking()) CancelInvoke();
            InvokeRepeating("Networking", 1f, 2f);
            Invoke("HideAlertView", 3.0f);
        }
        else
        {
            Debug.Log("OnNativeStateAction:The value of onNative： " + nativeState);
        }
    }


    public void ShowNetWorkNoConnect(bool isConnect)
    {
        //label.text = Localization.Get("Network_Retry");
        gameObject.SetActive(!isConnect);
        if (!isConnect)
        {
           
            Invoke("HideAlertView",3.0f);
        }   
    }

    public void HandleAlertView()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            //Invoke("ShowAgain", 2.1f);
        }
    }

    public void HideAlertView()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    void ShowAgain()
    {
        gameObject.SetActive(true);
    }
    private void Networking()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("Networking:Connect to the network, replay");
          
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}