using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LitJson;
using System;

public class CPictureView : MonoBehaviour {

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
    private bool mLoadTexture;
    private List<PictureData> mDataList = new List<PictureData>();
    private List<string> CateList = new List<string>();
    [HideInInspector]
    public List<GameObject> mList = new List<GameObject>();
    //private bool update = false;
    private Texture2D mErrorTex;
    void Start()
    {
        //CLanguage.OnLanguageChange += InitDropdownOption;
        mErrorTex = Resources.Load("pic-error") as Texture2D;
    }
    void OnEnable()
    {
        if (!mLoadTexture) LoadTexture();
    }

    private void InitDropdownOption()
    {
        CateList.Clear();
        Drd_CategoryList.ClearOptions();
        CateList.Add("All Groups");
        bool ungrouped = false;
        foreach (PictureData data in mDataList)
        {
            if (data.category == "0") ungrouped = true;
            if (data.category != "0" && !CateList.Contains(DataManager.Instance.GetCategory(DataType.picture, data.category))) CateList.Add(DataManager.Instance.GetCategory(DataType.picture, data.category));
        }
        if (ungrouped) CateList.Add(DataManager.Instance.GetCategory(DataType.picture, "0"));
        //Update dropdown
        Drd_CategoryList.AddOptions(CateList);
        if (Drd_CategoryList.value != 0)
            Drd_CategoryList.value = 0;
    }

    void SetPagePictures()
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
                    obj.GetComponent<Button>().onClick.AddListener(delegate
                    {

                        OnClickOpenPic(obj);
                    });
                }
                GameObject ga = mList[i % pageCount];
                PictureData info = mDataList[i];
                ga.name = info.id;
                CSetPicData cSet = ga.GetComponent<CSetPicData>();
                cSet.Data = info;
                cSet.SetText(info.name);
                cSet.ShowIcon(false);
                cSet.DestroyIcon();
                ga.SetActive(true);
                if (gameObject.activeSelf) StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.PicturePosterPath,info.posterURL), cSet));
            }
            else
            {
                if (mList.Count > i % pageCount)
                {
                    GameObject ga = mList[i % pageCount];
                    CSetPicData cSet = ga.GetComponent<CSetPicData>();
                    ga.SetActive(false);
                    cSet.DestroyIcon();
                }
            }
        }
        Resources.UnloadUnusedAssets();
    }

    public void LoadDataOver(List<PictureData> list)
    {
        mLoadTexture = false;
        mDataList.Clear();
        mDataList.AddRange(list);
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        InitDropdownOption();
        SetPagePictures();
        if (gameObject.activeSelf) mLoadTexture = true;
    }
    private void LoadTexture()
    {
        mLoadTexture = true;
        foreach (GameObject obj in mList)
        {
            CSetPicData data = obj.GetComponent<CSetPicData>();
            StartCoroutine(LoadTexture(Path.Combine(DataManager.Instance.PicturePosterPath,data.Data.posterURL), data));
        }
    }
    private IEnumerator LoadTexture(string sF, CSetPicData data)
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
                    Debug.Log("LoadTexture:"+www.error);
                }
            }
        }
    }
    public void fnDropSelect(int nIndex)
    {
        string strSortValue = CateList[nIndex];
        mDataList.Clear();
        List<PictureData> list = DataManager.Instance.PictureDatas;
        for (int i = 0; i < list.Count; i++)
        {
            PictureData data = list[i];
            if (strSortValue.Equals(DataManager.Instance.GetCategory(DataType.picture, data.category)) || nIndex == 0)
            {
                mDataList.Add(data);
            }
        }
        page = 0;
        totalPage = Math.Max(Mathf.CeilToInt(mDataList.Count * 1.0f / pageCount), 1);
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPagePictures();
    }

    public void OnPrePage()
    {
        page--;
        SetPagePictures();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnNextPage()
    {
        page++;
        SetPagePictures();
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
            SetPagePictures();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }
    public void OnClickOpenPic(GameObject sender)
    {
        CBaseData.ePlayType = PlayType.gallery;
        PictureData data = sender.GetComponent<CSetPicData>().Data;
        CBaseData.strPlayPictureName = Path.Combine(DataManager.Instance.PicturePath,data.pictureURL); ;
        CBaseData.strPlayPictureTitle = data.name.Replace("\n", "").Replace("\r", ""); ;
        CBaseData.strPlayPictureCategory = data.category;
        //Judge whether it is free mode
        string strModel = Server.Instance.fnGetServerStatus();
        string str = "";
        if (strModel.Equals("Free"))
        {
            str ="play the panorama without synchronizing the client";
        }
        else
        {
            str ="synchronize the client to play the panorama";
        }
        CPlayPicture cplay = GetComponent<CPlayPicture>();
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText(str);
        toast.OnClick = (index) =>
        {
            if (index == 0)
            {
                cplay.fnPlayPicture();
            }
            else
            {
                cplay.fnCancelPlay();
            }
        };
    }
}
