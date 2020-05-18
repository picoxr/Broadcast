using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSetClientData : MonoBehaviour {

    public Text texID, texIP;

    public Image imNetWork;

    public Toggle togAcceptOrder;

    public Sprite[] sprNet;

    //Status of the client  Ready,Playing,Pause,Free
    public Sprite[] sprStatue;

    public Image imStatus;

    //Icon array of client power
    public Sprite[] sprDianliang;

    //Client power
    public Image imDianLiang;

    //Charging cycle state
    private bool IsBatting = false;
    public float fTime = 0.25f;
    private int nIdex = 0;
    private ClientData mData;

    void OnEnable()
    {
        if(mData != null) togAcceptOrder.isOn = mData.accept;
    }

    public void fnSetData(ClientData data)
    {
        mData = data;
        togAcceptOrder.isOn = data.accept;
    }

    public void fnSetID(string strID)
    {
        texID.text = strID;
    }

    public void fnSetIP(string strIP)
    {
        texIP.text = strIP;
    }

    public string fnCategory
    {
        get; set;
    }
    //Set client status
    public void fnSetStu(string strStu)
    {
       switch(strStu)
        {
            case "Ready":
                imStatus.sprite = sprStatue[0];
                break;
            case "Playing":
            case "PlayGallery":
                imStatus.sprite = sprStatue[1];
                break;
            case "Pause":
                imStatus.sprite = sprStatue[2];
                break;
            case "Free":
                imStatus.sprite = sprStatue[3];
                break;
            default:
                imStatus.sprite = sprStatue[0];
                break;
        }
    }

    public void fnSetImage(bool IsConnect,string sensor = "")
    {
        if (IsConnect)
        {
            imNetWork.sprite = sensor == "0" ? sprNet[1] : sprNet[2];
        }
        else
        {
            imNetWork.sprite = sprNet[0];
            fnSetElectric(5,false);
        }
    }

    //Get whether the client accepts the server instruction
    public bool fnGetIsOreder()
    {
        return mData.accept;
        //Debug.Log("fnGetIsOreder:togAcceptOrder:" + togAcceptOrder.isOn);
        //return togAcceptOrder.isOn;
    }

    //Set whether clients accept server instructions
    public void fnSetToggleClient(bool IsAccept)
    {
        togAcceptOrder.isOn = IsAccept;
        mData.accept = IsAccept;
    }
    public void ToggleValueChange()
    {
        mData.accept = togAcceptOrder.isOn;
    }

    //Set the power of the client
    public void fnSetElectric(int nElec, bool IsBattey)
    {
        if(!IsBattey)
        {
            IsBatting = false;
            CancelInvoke("fnBatteying");
            switch (nElec)
            {
                case (1):
                    imDianLiang.sprite = sprDianliang[1];
                    break;
                case (2):
                    imDianLiang.sprite = sprDianliang[2];
                    break;
                case (3):
                    imDianLiang.sprite = sprDianliang[3];
                    break;
                case (4):
                    imDianLiang.sprite = sprDianliang[4];
                    break;
                case (5):
                    imDianLiang.sprite = sprDianliang[5];
                    break;
                default:
                    imDianLiang.sprite = sprDianliang[0];
                    break;
            }
            //if (nElec < 5)
            //    imDianLiang.sprite = sprDianliang[0];
            //else if (nElec >= 5 && nElec < 25)
            //    imDianLiang.sprite = sprDianliang[1];
            //else if (nElec >= 25 && nElec < 45)
            //    imDianLiang.sprite = sprDianliang[2];
            //else if (nElec >= 45 && nElec < 65)
            //    imDianLiang.sprite = sprDianliang[3];
            //else if (nElec >= 65 && nElec < 85)
            //    imDianLiang.sprite = sprDianliang[4];
            //else if (nElec >= 85)
            //    imDianLiang.sprite = sprDianliang[5];
        }
        else
        {
            IsBatting = true;
            nIdex = 0;
            Invoke("fnBatteying", 0.0f);
        }    
    }

    private void fnBatteying()
    {
        imDianLiang.sprite = sprDianliang[nIdex];
        nIdex++;
        if (nIdex >= sprDianliang.Length)
            nIdex = 0;

        if (IsBatting)
            Invoke("fnBatteying", fTime);
    }
}
