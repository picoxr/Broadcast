using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AppSet : MonoBehaviour {

    [SerializeField]
    RectTransform mTemplate;
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
    int page = 0;
    int pageCount = 5;
    int totalPage;
    // Use this for initialization
    void Start()
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.AppDatas.Count * 1.0f / pageCount), 1);
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform rect = Instantiate(mTemplate);
            rect.SetParent(mScroll.content);
            rect.gameObject.SetActive(false);
            AppTemp temp = rect.GetComponent<AppTemp>();
            temp.OnDelApp = OnDelApp;
            temp.OnUpApp = OnUpApp;
            temp.OnDownApp = OnDownApp;
        }
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageApp();
        
    }

    void OnEnable()
    {
        DataManager.OnAddApp += OnAddApp;
        DataManager.OnEditApp += OnEditApp;
        DataManager.OnEditCategory += OnEditCategory;
    }
    void OnDisable()
    {
        DataManager.OnAddApp -= OnAddApp;
        DataManager.OnEditApp -= OnEditApp;
        DataManager.OnEditCategory -= OnEditCategory;
    }
    private void SetPageApp()
    {
        int count = DataManager.Instance.AppDatas.Count < (page + 1) * pageCount ? DataManager.Instance.AppDatas.Count : (page + 1) * pageCount;
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            AppTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<AppTemp>();
            temp.gameObject.SetActive(i < count);
        }
        mScroll.gameObject.SetActive(false);
        mScroll.gameObject.SetActive(true);
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            AppTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<AppTemp>();
            if (i < count)
            {
                AppData app = DataManager.Instance.AppDatas[i];
                app.id = (i + 1).ToString();
                temp.SetData(app);
            }
            else
            {
                temp.DestroyIcon();
            }
        }
    }

    private void OnAddApp(AppData app)
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.AppDatas.Count * 1.0f / pageCount), 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
        if (page == totalPage - 1)
        {
            AppTemp temp = mScroll.content.GetChild((DataManager.Instance.AppDatas.Count - 1) % pageCount).GetComponent<AppTemp>();
            app.id = DataManager.Instance.AppDatas.Count.ToString();
            temp.gameObject.SetActive(true);
            mScroll.content.gameObject.SetActive(false);
            mScroll.content.gameObject.SetActive(true);
            temp.SetData(app);
            if (DataManager.Instance.AppDatas.Count % pageCount != 1)
            {
                AppTemp _temp = mScroll.content.GetChild((DataManager.Instance.AppDatas.Count - 2) % pageCount).GetComponent<AppTemp>();
                _temp.UpdateIndex(DataManager.Instance.GetAppIndex(_temp.Id));
            }
        }
        else if (page == totalPage - 2 && DataManager.Instance.AppDatas.Count % pageCount == 1)
        {
            AppTemp temp = mScroll.content.GetChild((DataManager.Instance.AppDatas.Count - 2) % pageCount).GetComponent<AppTemp>();
            temp.UpdateIndex(DataManager.Instance.GetAppIndex(temp.Id));
        }
    }

    private void OnEditApp(AppData app)
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            AppTemp temp = mScroll.content.GetChild(i).GetComponent<AppTemp>();
            if (temp.Id == app.id)
            {
                temp.SetData(app);
                break;
            }
        }
    }

    private void OnEditCategory()
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            AppTemp temp = mScroll.content.GetChild(i).GetComponent<AppTemp>();
            if(temp.gameObject.activeSelf) temp.UpdateCategory();
        }
    }

    private void OnDelApp(AppTemp temp)
    {
        if (DataManager.Instance.AppDatas.Count % pageCount == 0 || page != totalPage - 1)
        {
            if (page == totalPage - 1 && page != 0) page--;
            totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.AppDatas.Count * 1.0f / pageCount), 1);
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
            SetPageApp();
        }
        else
        {
            temp.gameObject.SetActive(false);
            temp.transform.parent = null;
            temp.transform.parent = mScroll.content;
            for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
            {
                AppTemp _temp = mScroll.content.GetChild(i % pageCount).GetComponent<AppTemp>();
                if (_temp.gameObject.activeSelf)
                {
                    AppData data = DataManager.Instance.AppDatas[i];
                    data.id = (i + 1).ToString();
                    _temp.Id = data.id;
                    _temp.UpdateIndex(DataManager.Instance.GetAppIndex(_temp.Id));
                }
            }
        }
    }

    private void OnUpApp(AppTemp temp)
    {
        int index = DataManager.Instance.GetAppIndex(temp.Id);
        AppData data = DataManager.Instance.GetAppData(index - 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % pageCount != 0)
        {
            AppTemp _temp = mScroll.content.GetChild(index% pageCount - 1).GetComponent<AppTemp>();
            AppData _data = DataManager.Instance.GetAppData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index-1);
        }
        DataManager.Instance.ExchangeAppWithPre(index);
    }

    private void OnDownApp(AppTemp temp)
    {
        int index = DataManager.Instance.GetAppIndex(temp.Id);
        AppData data = DataManager.Instance.GetAppData(index + 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % (pageCount - 1) != 0 || index == 0)
        {
            AppTemp _temp = mScroll.content.GetChild(index% pageCount + 1).GetComponent<AppTemp>();
            AppData _data = DataManager.Instance.GetAppData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index + 1);
        }
        DataManager.Instance.ExchangeAppWithNext(index);
    }

    public void OnAddClick()
    {
        AppSetEdit edit = UIManager.Instance.PushUI(UI.AppEdit).GetComponent<AppSetEdit>();
        edit.Add = true;
        edit.SetData(null);
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
        int.TryParse(mPageInput.text,out _page);
        if (_page >= 1 && _page <= totalPage)
        {
            page = _page - 1;
            SetPageApp();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }

    public void OnEditClick()
    {
        UIManager.Instance.PushUI(UI.AppCategory);
    }
    public void OnPrePage()
    {
        page--;
        SetPageApp();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
    public void OnNextPage()
    {
        page++;
        SetPageApp();
        mPreBtn.enabled = true;
        mNextBtn.enabled = page < (totalPage - 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
}