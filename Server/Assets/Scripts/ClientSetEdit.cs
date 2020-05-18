using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;
using LitJson;

public class ClientSetEdit : MonoBehaviour {

    [SerializeField]
    InputField mId;
    [SerializeField]
    InputField mSn;
    [SerializeField]
    Dropdown mCategory;

    private ClientData Model;
    private int Index;
    public bool Add
    {
        get; set;
    }

    public void SetData(ClientData model,int _index = -1)
    {
        Index = _index;
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.client));
        if (model != null)
        {
            Model = model;
            mId.text = model.id;
            mSn.text = model.sn;
            int index = DataManager.Instance.GetCategories(DataType.client).IndexOf(DataManager.Instance.GetCategory(DataType.client, model.category));
            mCategory.value = index;
            //bool find = false;
            //foreach (string category in DataManager.Instance.ClientCategories)
            //{
            //    if (category == model.category)
            //    {
            //        find = true;
            //        break;
            //    }
            //}
            //mCategory.captionText.text = find ? model.category : DataManager.Instance.ClientCategories[0];
        }
        else {
            Model = null;
            mId.text = mSn.text = "";
        }
    }

    public void OnConfirmClick()
    {
        if (string.IsNullOrEmpty(mId.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The client ID can not be empty");
            return;
        }
        else if (string.IsNullOrEmpty(mSn.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The client SN can not be empty");
            return;
        }
        //else if (string.IsNullOrEmpty(mCategory.captionText.text))
        //{
        //    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        //    toast.SetText(I2.Loc.ScriptLocalization.Get("Classification can not be empty"));
        //    return;
        //}
        if (Add)
        {
            ClientData _client = DataManager.Instance.GetClientById(mId.text);
            if (_client != null)
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Client ID already exists");
                return;
            }
            _client = DataManager.Instance.GetClientBySn(mSn.text);
            if (_client != null)
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Client SN already exists");
                return;
            }
        }
        else {
            if (mId.text != Model.id)
            {
                ClientData _client = DataManager.Instance.GetClientById(mId.text);
                if (_client != null)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Client ID already exists");
                    return;
                }
            }
            if (mSn.text != Model.sn)
            {
                ClientData _client = DataManager.Instance.GetClientBySn(mSn.text);
                if (_client != null)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Client SN already exists");
                    return;
                }
            }
        }
        if (Add) { Model = new ClientData(); }
        Model.id = mId.text.Replace("\n", "").Replace("\r", "");
        Model.sn = mSn.text.Replace("\n", "").Replace("\r", "");
        Model.category = DataManager.Instance.GetCategoryId(DataType.client, mCategory.captionText.text);
        DataManager.Instance.EditClient(Model, Add,Index);
        UIManager.Instance.PopUI();
    }

    public void OnCancleClick()
    {
        UIManager.Instance.PopUI();
    }

    public void OnFreshCategory()
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.client));
    }
    public void OnValueChanged(InputField input)
    {
        if (input == mId)
        {
            string str = Tools.SplitNameByASCII(input.text.Trim(),4);
            input.text = str;
        }
        else {
            string str = Tools.SplitNameByASCII(input.text.Trim());
            input.text = str;
        }
    }
}
