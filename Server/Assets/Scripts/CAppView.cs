using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LitJson;
using System;

public class CAppView : MonoBehaviour
{

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
    int page, totalPage;
    int pageCount = 8;
    private List<AppData> mDataList = new List<AppData>();
    private List<string> CateList = new List<string>();
    [HideInInspector]
    public List<GameObject> mList = new List<GameObject>();
    ///Current app name
    public Text AlreadyPlayedApp;
    //Mask UI after starting application
    public GameObject gaZheZhaoApp;
    private GameObject gaAppTemp;
    ///Played time
    public Text AlreadyPlayedTime;
    private float appTime = 0f;
    private bool isOpenApp = false;
    private bool mLoadTexture;
    private Texture2D mErrorTex;
    void Start()
    {
      
        mErrorTex = Resources.Load("pic-error") as Texture2D;
    }

    void OnEnable()
    {
        if (!mLoadTexture) LoadTexture();
    }
    private void Update()
    {
        //timer
        if (isOpenApp)
        {
            appTime += Time.deltaTime;
            AlreadyPlayedTime.text = appTime.ToString("F2");
        }
    }
    private void InitDropdownOption()
    {
        CateList.Clear();
        Drd_CategoryList.ClearOptions();
        CateList.Add("All Groups");
        bool ungrouped = false;
        foreach (AppData data in mDataList)
        {
            if (data.category == "0") ungrouped = true;
            if (data.category != "0" && !CateList.Contains(DataManager.Instance.GetCategory(DataType.app, data.category))) CateList.Add(DataManager.Instance.GetCategory(DataType.app, data.category));
        }
        if (ungrouped) CateList.Add(DataManager.Instance.GetCategory(DataType.app, "0"));
        //Update dropdown
        Drd_CategoryList.AddOptions(CateList);
        if (Drd_CategoryList.value != 0)
            Drd_CategoryList.value = 0;
    }

    void SetPageApps()
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
                    obj.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnClickOpenApp(obj);
                    });
                }
                GameObject ga = mList[i % pageCount];
                AppData info = mDataList[i];
                ga.name = info.id;
                CSetAppData cSet = ga.GetComponent<CSetAppData>();
                cSet.SetText(info.name);
                cSet.Data = info;
                cSet.ShowIcon(false);
                cSet.DestroyIcon();
                ga.SetActive(true);
                if (gameObject.activeSelf) StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.AppPosterPath, info.picture), cSet));
            }
            else
            {
                if (mList.Count > i % pageCount)
                {
                    GameObject ga = mList[i % pageCount];
                    CSetAppData cSet = ga.GetComponent<CSetAppData>();
                    ga.SetActive(false);
                    cSet.DestroyIcon();
                }
            }
        }
        Resources.UnloadUnusedAssets();
    }

    public void LoadDataOver(List<AppData> appList)
    {
        mLoadTexture = false;
        mDataList.Clear();
        mDataList.AddRange(appList);
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        InitDropdownOption();
        SetPageApps();
        if (gameObject.activeSelf) mLoadTexture = true;
    }
    private void LoadTexture()
    {
        mLoadTexture = true;
        foreach (GameObject obj in mList)
        {
            CSetAppData data = obj.GetComponent<CSetAppData>();
            StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.AppPosterPath,data.Data.picture), data));
        }
    }
    private IEnumerator LoadTexture(string sF, CSetAppData data)
    {
        string str = Tools.GetUrl(sF);
        WWW www = new WWW(str);
        yield return www;
        if (www.isDone)
        {
            if (www.error == null || www.error == "")
            {
                Texture2D tex = www.texture;
                if (tex.width != 480 || tex.height != 270)
                {
                    tex = Tools.ReSetTextureSize(tex, 480, 270);
                    File.Delete(sF);
                    Tools.SaveTexture(tex, sF);
                }
                Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                data.mIcon.GetComponent<RectTransform>().sizeDelta = data.mSizeDelta;
                data.SetIcon(temp);
            }
            else
            {
                if (mErrorTex != null)
                {
                    Sprite temp = Sprite.Create(mErrorTex, new Rect(0, 0, mErrorTex.width, mErrorTex.height), new Vector2(0, 0));
                    data.mIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(mErrorTex.width, mErrorTex.height);
                    data.SetIcon(temp);
                    Debug.Log("LoadTexture:" + www.error);
                }
            }
        }
    }
    public void fnDropSelect(int nIndex)
    {
        string strSortValue = CateList[nIndex];
        mDataList.Clear();
        List<AppData> list = DataManager.Instance.AppDatas;
        for (int i = 0; i < list.Count; i++)
        {
            AppData data = list[i];
            if (strSortValue.Equals(DataManager.Instance.GetCategory(DataType.app, data.category)) || nIndex == 0)
            {
                mDataList.Add(data);
            }
        }
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageApps();
    }

    public void OnPrePage()
    {
        page--;
        SetPageApps();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnNextPage()
    {
        page++;
        SetPageApps();
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
            SetPageApps();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }
    /// <summary>
    ///Select app pop up
    /// </summary>
    /// <param name="sender"></param>
    //public void OnClickOpenApp(GameObject sender)
    //{
    //    if (Input.GetMouseButtonUp(1))
    //        return;

    //    CBaseData.ePlayType = PlayType.app;
    //    //Judge whether it is free mode
    //    string strModel = Server.Instance.fnGetServerStatus();
    //    if (strModel.Equals("Free"))
    //    {
    //        ToastView _toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
    //        _toast.SetText(I2.Loc.ScriptLocalization.Get("Switch to Network Mode"));
    //        return;
    //    }
    //    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
    //    toast.SetText(I2.Loc.ScriptLocalization.Get("Run app"));
    //    toast.OnClick = (index) =>
    //    {
    //        if (index == 0)
    //        {
    //            fnAppSurePlay(sender);
    //        }
    //    };
    //}
    //public void fnAppSurePlay(GameObject sender)
    //{
    //    AppData data = sender.GetComponent<CSetAppData>().Data;
    //    CBaseData.strPlayAppName = data.pkgname;
    //    CBaseData.strPlayAppTitle = data.title;
    //    //StartCoroutine("StartAppWait",sender);
    //    //gaAppTemp = sender;
    //    //appTime = 0;
    //    //isOpenApp = true;
    //    CLoadJson.Instance.SocketSend("gotoapp" + CBaseData.strPlayAppName);
    //    //rtmp://192.168.50.18:1935/live/room
    //    //Turn on the mask of application to shield other buttons
    //    //if (gaZheZhaoApp)
    //    //    gaZheZhaoApp.SetActive(true);
    //    CBaseData.strPlayVideoName = "";
    //    CBaseData.strPlayVideoTitle = "";
    //    CBaseData.strVideoType = "2D";
    //    CBaseData.strLinkModel = "Online";
    //    CPlayVideo.Instance.fnPlayApp();
    //}
    public void OnClickOpenApp(GameObject sender)
    {
        if (gaAppTemp == null)
        {
            if (Input.GetMouseButtonUp(1))
                return;
            string strModel = Server.Instance.fnGetServerStatus();
            if (strModel.Equals("Free"))
            {
                ToastView _toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                _toast.SetText("Switch to Network Mode");
                return;
            }
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Run app");
            toast.OnClick = (index) =>
            {
                if (index == 0)
                {
                    fnAppSurePlay(sender);
                }
            };
        }
        else if(gaAppTemp == sender)
        {
            if(sender.GetComponent<CSetAppData>().ActiveClose()) OnClickCloseApp(sender);
        }        
    }
    public void fnAppSurePlay(GameObject sender)
    {
        AppData data = sender.GetComponent<CSetAppData>().Data;
        CBaseData.strPlayAppName = data.pkgname;
        CBaseData.strPlayAppTitle = data.name;
        StartCoroutine("StartAppWait",sender);
        gaAppTemp = sender;
        appTime = 0;
        isOpenApp = true;
        CLoadJson.Instance.SocketSend("gotoapp" + CBaseData.strPlayAppName);
        if (gaZheZhaoApp)
            gaZheZhaoApp.SetActive(true);
    }

    private IEnumerator StartAppWait(GameObject sender)
    {
        yield return new WaitForSeconds(2);
        sender.GetComponent<CSetAppData>().ShowClose(true);
        AlreadyPlayedApp.text = CBaseData.strPlayAppTitle;
    }

    /// <summary>
    /// Confirm to stop running app
    /// </summary>
    public void fnStopAppSure(GameObject sender)
    {
        gaAppTemp = null;
        StopAppWait(sender);
        isOpenApp = false;
        CLoadJson.Instance.SocketSend("gotoappcom.picovr.broadcast");
        //Unmask
        if (gaZheZhaoApp)
            gaZheZhaoApp.SetActive(false);
    }

    private void StopAppWait(GameObject sender)
    {
        sender.GetComponent<CSetAppData>().ShowClose(false);
        Debug.Log("StopAppWait: " + CBaseData.strPlayAppName + "--" + CBaseData.strPlayAppTitle);
    }

    /// <summary>
    /// Select app close
    /// </summary>
    public void OnClickCloseApp(GameObject sender)
    {
        if (Input.GetMouseButtonUp(1))
            return;

        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Stop app");
        toast.OnClick = (index) =>
        {
            if (index == 0)
            {
                fnStopAppSure(sender);
            }
        };
    }
}
