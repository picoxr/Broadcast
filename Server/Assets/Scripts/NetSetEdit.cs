using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;
using LitJson;

public class NetSetEdit : MonoBehaviour {

    [SerializeField]
    InputField mSsid;
    [SerializeField]
    InputField mPswd;
    [SerializeField]
    InputField mServerIp;
    [SerializeField]
    InputField mPort;
    [SerializeField]
    InputField mId;

    public void SetData(List<string> list)
    {
        if (list.Count == 0)
        {
            mSsid.text = mPswd.text = mServerIp.text = mPort.text = mId.text = "";
        }
        else {

            mSsid.text = list[0];
            mPswd.text = list[1];
            mServerIp.text = list[2];
            mPort.text = list[3];
            mId.text = list[4]; ;
        }
    }

    public void OnConfirmClick()
    {
        if (string.IsNullOrEmpty(mSsid.text))
        {
            return;
        }
        else if (string.IsNullOrEmpty(mPswd.text))
        {
            return;
        }
        else if (string.IsNullOrEmpty(mServerIp.text))
        {
            return;
        }
        if (string.IsNullOrEmpty(mPort.text))
        {
            mPort.text = "8081";
        }
        if (string.IsNullOrEmpty(mId.text))
        {
            mId.text = "0";
        }
        List<string> values = new List<string>();
        values.Add(mSsid.text);
        values.Add(mPswd.text);
        values.Add(mServerIp.text);
        values.Add(mPort.text);
        values.Add(mId.text);
        DataManager.Instance.UpdateNetConfig(values);
        UIManager.Instance.ReplaceUI(UI.NetSet);
    }

    public void OnCancleClick()
    {
        UIManager.Instance.ReplaceUI(UI.NetSet);
    }
    public void OnValueChange(InputField input)
    {
        string str = Tools.SplitNameByASCII(input.text);
        input.text = str;
    }
}
