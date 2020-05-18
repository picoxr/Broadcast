using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CQuitUserlist : MonoBehaviour {

    public GameObject gaUserlist;
    [SerializeField]
    Toggle VolumeToggle;
    [SerializeField]
    Slider VolumeSlider;
    [SerializeField]
    GameObject[] mVolumeObj;
    void OnEnable()
    {
        Server.ChangeModel += OnModelChange;
        Server.ChangeVolume += OnChangeVolume;

        //if (Server.Instance.ChangeVolume != null)
        //    Server.Instance.ChangeVolume((int)VolumeSlider.value);
        //VolumeSlider.gameObject.SetActive(!Server.Instance.IsFree);
        //if (!Server.Instance.IsFree)
        //{
        //    VolumeToggle.isOn = Server.Instance.ClientVolume == -1;
        //    if (Server.Instance.ClientVolume != -1) VolumeSlider.value = Server.Instance.ClientVolume;
        //}
    }

    void OnDisable()
    {
        Server.ChangeModel -= OnModelChange;
        Server.ChangeVolume -= OnChangeVolume;
    }

    private void OnModelChange(bool isFree)
    {
        VolumeToggle.gameObject.SetActive(!isFree);
        VolumeSlider.gameObject.SetActive(!isFree);
    }

    private void OnChangeVolume(int value)
    {
        VolumeToggle.isOn = value == -1;
        if (!VolumeToggle.isOn)
        {
            VolumeSlider.value = value;
        }
    }

    public void fnShutPlane()
    {
        if(gaUserlist)
        gaUserlist.SetActive(false);
    }
    public void VolumeToggleChanged()
    {
        if (VolumeToggle.isOn)
        {
            VolumeToggle.GetComponent<Image>().enabled = false;
            mVolumeObj[0].SetActive(true);
            mVolumeObj[1].SetActive(false);
            mVolumeObj[2].SetActive(false);
            Server.Instance.fnSetVolume(-1);
        }
        else
        {
            if (VolumeSlider.value == 0)
            {
                VolumeToggle.GetComponent<Image>().enabled = false;
                mVolumeObj[0].SetActive(false);
                mVolumeObj[1].SetActive(true);
                mVolumeObj[2].SetActive(false);
            }
            else if (VolumeSlider.value > 0 && VolumeSlider.value <= 50)
            {
                VolumeToggle.GetComponent<Image>().enabled = false;
                mVolumeObj[0].SetActive(false);
                mVolumeObj[1].SetActive(false);
                mVolumeObj[2].SetActive(true);
            }
            else if (VolumeSlider.value > 50 && VolumeSlider.value <= 100)
            {
                VolumeToggle.GetComponent<Image>().enabled = true;
                mVolumeObj[0].SetActive(false);
                mVolumeObj[1].SetActive(false);
                mVolumeObj[2].SetActive(false);
            }
            Server.Instance.fnSetVolume((int)VolumeSlider.value);
        }
    }


    public void VolumeSliderChanged()
    {
        VolumeToggle.isOn = false;
        if (VolumeSlider.value == 0)
        {
            VolumeToggle.GetComponent<Image>().enabled = false;
            mVolumeObj[0].SetActive(false);
            mVolumeObj[1].SetActive(true);
            mVolumeObj[2].SetActive(false);
        }
        else if (VolumeSlider.value > 0 && VolumeSlider.value <= 50)
        {
            VolumeToggle.GetComponent<Image>().enabled = false;
            mVolumeObj[0].SetActive(false);
            mVolumeObj[1].SetActive(false);
            mVolumeObj[2].SetActive(true);
        }
        else if (VolumeSlider.value > 50 && VolumeSlider.value <= 100)
        {
            VolumeToggle.GetComponent<Image>().enabled = true;
            mVolumeObj[0].SetActive(false);
            mVolumeObj[1].SetActive(false);
            mVolumeObj[2].SetActive(false);
        }
        Server.Instance.fnSetVolume((int)VolumeSlider.value);
    }
}
