using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LitJson;
using System;
using UnityEngine.EventSystems;
public class CVideoView : MonoBehaviour {

    //Video class UI object
    public GameObject gaUI;
    public Sprite[] spVideoType;
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
    Dropdown Drd_TypeList;
    private List<string> CateList = new List<string>();
    private List<string> TypeList = new List<string>();
    string strSortValue, strTypeValue;
    int sortIndex = 0, typeIndex = 0;
    int page, totalPage;
    int pageCount = 8;
    private bool mLoadTexture;
    //string strVideoPath = "pre_resource" + "\\CoverPictures\\";
    private List<VideoData> mDataList = new List<VideoData>();
    [HideInInspector]
    public List<GameObject> mList = new List<GameObject>();
    private Texture2D mErrorTex;
    void Start()
    {
        //CLanguage.OnLanguageChange += InitDropdownOption;
        mErrorTex = Resources.Load("pic-error") as Texture2D;
    }
    void OnEnable()
    {

        if(!mLoadTexture) LoadTexture();
    }
    private void InitDropdownOption()
    {
        CateList.Clear();
        TypeList.Clear();
        Drd_CategoryList.ClearOptions();
        Drd_TypeList.ClearOptions();
        CateList.Add("All Groups");
        TypeList.Add("All Formats");
        bool ungrouped = false;
        foreach (VideoData data in mDataList)
        {
            if (data.category == "0") ungrouped = true;
            if (data.category != "0" && !CateList.Contains(DataManager.Instance.GetCategory(DataType.Video, data.category))) CateList.Add(DataManager.Instance.GetCategory(DataType.Video,data.category));
            if (!TypeList.Contains(DataManager.Instance.GetVideoType(data.type))) TypeList.Add(DataManager.Instance.GetVideoType(data.type));
        }
        if(ungrouped) CateList.Add(DataManager.Instance.GetCategory(DataType.Video, "0"));
        strSortValue = CateList[0];
        strTypeValue = TypeList[0];
        Drd_CategoryList.AddOptions(CateList);
        Drd_TypeList.AddOptions(TypeList);
        if (Drd_CategoryList.value != 0)
            Drd_CategoryList.value = 0;
        if (Drd_TypeList.value != 0)
            Drd_TypeList.value = 0;
    }

    void SetPageVideos()
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
                        OnClickVideo(obj);
                    });
                }
                GameObject ga = mList[i % pageCount];
                CSetVideoData cSet = ga.GetComponent<CSetVideoData>();
                VideoData info = mDataList[i];
                ga.name = info.id;
                cSet.Data = info;
                cSet.SetText(info.name);
                cSet.ShowIcon(false);
                cSet.DestroyIcon();
     
                switch (DataManager.Instance.GetVideoType(info.type))
                {
                  
                    case "3D_LR":
                        cSet.SetType(spVideoType[1]);
                        break;
                    case "3D_TB":
                        cSet.SetType(spVideoType[2]);
                        break;
                    case "360":
                        cSet.SetType(spVideoType[7]);
                        break;
                }
                ga.SetActive(true);
                if (gameObject.activeSelf) StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.VideoPosterPath,info.picture), cSet));
            }
            else
            {
                if (mList.Count > i % pageCount)
                {
                    GameObject ga = mList[i % pageCount];
                    CSetVideoData cSet = ga.GetComponent<CSetVideoData>();
                    ga.SetActive(false);
                    cSet.DestroyIcon();
                }
            }
        }
        Resources.UnloadUnusedAssets();
    }

    public void LoadDataOver(List<VideoData> videoList)
    {
        mLoadTexture = false;
        mDataList.Clear();
        mDataList.AddRange(videoList);
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        InitDropdownOption();
        SetPageVideos();
        if (gameObject.activeSelf) mLoadTexture = true;
    }

    private void LoadTexture()
    {
        mLoadTexture = true;
        foreach (GameObject obj in mList)
        {
            CSetVideoData data = obj.GetComponent<CSetVideoData>();
            StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.VideoPosterPath,data.Data.picture), data));
        }
    }

    private IEnumerator LoadTexture(string sF, CSetVideoData data)
    {
        Debug.Log("LoadTexture:Loadtex:" + sF);
        string str = Tools.GetUrl(sF);
        Debug.Log("LoadTexture:Loadtex:" + str);
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
                    Tools.SaveTexture(tex,sF);
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
                    Debug.Log("LoadTexture:"+www.error);
                }
            }
        }
    }

    public void fnDropSelect(int nIndex)
    { 
        strSortValue = CateList[nIndex];
        sortIndex = nIndex;
        mDataList.Clear();
        List<VideoData> list = DataManager.Instance.VideoDatas;
        for (int i = 0; i < list.Count; i++)
        {
            VideoData data = list[i];
            if ((strSortValue.Equals(DataManager.Instance.GetCategory(DataType.Video,data.category)) || sortIndex == 0) && (strTypeValue.Equals(DataManager.Instance.GetVideoType(data.type)) || typeIndex == 0))
            {
                mDataList.Add(data);
            }
        }
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageVideos();
    }
    
    public void fnDropTypeSelect(int nIndex)
    {
        strTypeValue = TypeList[nIndex];
        typeIndex = nIndex;
        mDataList.Clear();
        List<VideoData> list = DataManager.Instance.VideoDatas;
        for (int i = 0; i < list.Count; i++)
        {
            VideoData data = list[i];
            if ((strSortValue.Equals(DataManager.Instance.GetCategory(DataType.Video,data.category)) || sortIndex == 0) && (strTypeValue.Equals(DataManager.Instance.GetVideoType(data.type)) || typeIndex == 0))
            {
                mDataList.Add(data);
            }
        }
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageVideos();
    }

    public void OnPrePage()
    {
        page--;
        SetPageVideos();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnNextPage()
    {
        page++;
        SetPageVideos();
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
            SetPageVideos();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }

    public void OnClickVideo(GameObject sender)
    {
        CBaseData.ePlayType = PlayType.gallery;
        VideoData data = sender.GetComponent<CSetVideoData>().Data;
        CBaseData.strPlayVideoName = Path.Combine(DataManager.Instance.VideoPath, data.video);
        //CBaseData.strPlayVideoTitle = data.name.Replace("\n", "").Replace("\r", "");
        Debug.Log("OnClickVideo"+6666666 +data.name);
        string strModel = Server.Instance.fnGetServerStatus();
        string str = "";
        if (strModel.Equals("Free"))
        {
            //str = I2.Loc.ScriptLocalization.Get("play the video without synchronizing the client");
            str = "play the video without synchronizing the client";
        }
        else
        {
            //str = I2.Loc.ScriptLocalization.Get("synchronize the client to play the panorama");
            str = "synchronize the client to play the Video";
        }
        CPlayVideo cplay = GetComponent<CPlayVideo>();
        // CPlayPicture cplay = GetComponent<CPlayPicture>();
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText(str);
        toast.OnClick = (index) =>
        {
            Debug.Log("OnClickVideo:"+index);
            if (index == 0)
            {
                cplay.fnPlayVideo(data.type,data.name);
            }
            else
            {
                cplay.fnCancelPlay();
            }
        };
    }

    public void OnPointEnter(Image image)
    {
        image.color = Color.white;
        Texture2D tex = Resources.Load("dropdown_roll") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnPointExit(Image image)
    {
        image.color = new Color(241/255.0f, 241 / 255.0f, 241 / 255.0f);
        Texture2D tex = Resources.Load("dropdown_sprite") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnSelect(Image image)
    {
        image.color = Color.white;
        Texture2D tex = Resources.Load("dropdown_active") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
}