using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LitJson;
using System;

public class CClientView : MonoBehaviour {

    public GameObject gaUI;
    [SerializeField]
    ScrollRect mScroll;
    [SerializeField]
    Button mPreBtn;
    [SerializeField]
    Button mNextBtn;
    [SerializeField]
    Text mPage;
    [SerializeField]
    InputField mPageInput;
    [SerializeField]
    Dropdown Drd_CategoryList;
    [SerializeField]
    Toggle VolumeToggle;
    [SerializeField]
    Slider VolumeSlider;
    [SerializeField]
    GameObject[] mVolumeObj;
    int page, totalPage;
    int pageCount = 35;
    private List<ClientData> mDataList = new List<ClientData>();
    private List<string> CateList = new List<string>();
    [HideInInspector]
    public List<GameObject> mList = new List<GameObject>();
    private bool update = false;
    void Start()
    {
        //CLanguage.OnLanguageChange += InitDropdownOption;
    }
    void OnEnable()
    {
        Server.ChangeModel += OnModelChange;
        VolumeToggle.gameObject.SetActive(!Server.Instance.IsFree);
        VolumeSlider.gameObject.SetActive(!Server.Instance.IsFree);
        if (!Server.Instance.IsFree)
        {
            VolumeToggle.isOn = Server.Instance.ClientVolume == -1;
            if (Server.Instance.ClientVolume != -1) VolumeSlider.value = Server.Instance.ClientVolume;
        }
    }

    void OnDisable()
    {
        Server.ChangeModel -= OnModelChange;
    }

    private void OnModelChange(bool isFree)
    {
        VolumeToggle.gameObject.SetActive(!isFree);
        VolumeSlider.gameObject.SetActive(!isFree);
    }

    private void InitDropdownOption()
    {
        CateList.Clear();
        Drd_CategoryList.ClearOptions();
        CateList.Add("All Groups");
        bool ungrouped = false;
        foreach (ClientData data in mDataList)
        {
            if (data.category == "0") ungrouped = true;
            if (data.category != "0" && !CateList.Contains(DataManager.Instance.GetCategory(DataType.client, data.category))) CateList.Add(DataManager.Instance.GetCategory(DataType.client, data.category));
        }
        if (ungrouped) CateList.Add(DataManager.Instance.GetCategory(DataType.client, "0"));
        //Update dropdown
        Drd_CategoryList.AddOptions(CateList);
        if (Drd_CategoryList.value != 0)
            Drd_CategoryList.value = 0;
    }

    void SetPageClients()
    {
        int count = mDataList.Count < (page + 1) * pageCount ? mDataList.Count : (page + 1) * pageCount;
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            if (i < count)
            {
                if (mList.Count <= i % pageCount)
                {
                    GameObject obj = Instantiate(gaUI, gaUI.transform.position, gaUI.transform.rotation);
                    obj.transform.SetParent(mScroll.content.transform, false);
                    mList.Add(obj);
                }
                GameObject ga = mList[i % pageCount];
                ClientData info = mDataList[i];
                ga.name = info.id;
                CSetClientData cSet = ga.GetComponent<CSetClientData>();
                cSet.fnSetData(info);
                cSet.fnSetID(info.id);
                cSet.fnSetIP(info.sn);
                cSet.fnCategory = info.category;
                cSet.fnSetStu(PlayStatus.Free.ToString());
                bool connect = false;
                string sensor = "0";
                for (int j = 0; j < Server.Instance.clientList.Count; j++)
                {
                    clientStatus status = Server.Instance.clientList[j];
                    if (status.sn == info.sn)
                    {
                        connect = true;
                        sensor = status.sensor;
                        break;
                    }
                }
                cSet.fnSetImage(connect, sensor);
                if (connect)
                {
                    cSet.fnSetStu(Server.Instance.IsFree ? PlayStatus.Free.ToString() : PlayStatus.Ready.ToString());
                }
                else
                {
                    cSet.fnSetStu(PlayStatus.Free.ToString());
                }
                ga.SetActive(true);
            }
            else
            {
                if (mList.Count > i % pageCount)
                {
                    mList[i % pageCount].SetActive(false);
                }
            }
        }
    }

    public void LoadDataOver(List<ClientData> list)
    {
        update = true;
        mDataList.Clear();
        mDataList.AddRange(list);
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        InitDropdownOption();
        SetPageClients();
    }
    public void fnDropSelect(int nIndex)
    { 
        string strSortValue = CateList[nIndex];
        mDataList.Clear();
        List<ClientData> list = DataManager.Instance.ClientDatas;
        for (int i = 0; i < list.Count; i++)
        {
            ClientData data = list[i];
            if (strSortValue.Equals(DataManager.Instance.GetCategory(DataType.client, data.category)) || nIndex == 0)
            {
                mDataList.Add(data);
            }
        }
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageClients();
    }

    public void OnPrePage()
    {
        page--;
        SetPageClients();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnNextPage()
    {
        page++;
        SetPageClients();
        mPreBtn.enabled = true;
        mNextBtn.enabled = page < (totalPage - 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnPageClick()
    {
        mPage.gameObject.SetActive(false);
        mPageInput.gameObject.SetActive(true);
        mPageInput.ActivateInputField();
    }

    public void OnEndEdit()
    {
        mPage.gameObject.SetActive(true);
        mPageInput.gameObject.SetActive(false);
        int _page = 1;
        int.TryParse(mPageInput.text, out _page);
        if (_page >= 1 && _page <= totalPage)
        {
            page = _page - 1;
            SetPageClients();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
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
            if (Server.ChangeVolume != null)
                Server.ChangeVolume(-1);
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
            if (Server.ChangeVolume != null)
                Server.ChangeVolume((int)VolumeSlider.value);
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
        if (Server.ChangeVolume != null)
            Server.ChangeVolume((int)VolumeSlider.value);
    }
}
