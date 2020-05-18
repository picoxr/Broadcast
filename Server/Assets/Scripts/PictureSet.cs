using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PictureSet : MonoBehaviour {

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
    void Start() {

        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.PictureDatas.Count * 1.0f / pageCount), 1);
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform rect = Instantiate(mTemplate);
            rect.SetParent(mScroll.content);
            rect.gameObject.SetActive(false);
            PictureTemp temp = rect.GetComponent<PictureTemp>();
            temp.OnDelPicture = OnDelPicture;
            temp.OnUpPicture = OnUpPicture;
            temp.OnDownPicture = OnDownPicture;
        }
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPagePicture();
    }

    void OnEnable()
    {
        DataManager.OnAddPicture += OnAddPicture;
        DataManager.OnEditPicture += OnEditPicture;
        DataManager.OnEditCategory += OnEditCategory;

    }
    void OnDisable()
    {
        DataManager.OnAddPicture -= OnAddPicture;
        DataManager.OnEditPicture -= OnEditPicture;
        DataManager.OnEditCategory -= OnEditCategory;
    }

    private void SetPagePicture()
    {
        int count = DataManager.Instance.PictureDatas.Count < (page + 1) * pageCount ? DataManager.Instance.PictureDatas.Count : (page + 1) * pageCount;
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            PictureTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<PictureTemp>();
            temp.gameObject.SetActive(i < count);
        }
        mScroll.gameObject.SetActive(false);
        mScroll.gameObject.SetActive(true);
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            PictureTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<PictureTemp>();
            if (i < count)
            {
                PictureData pic = DataManager.Instance.PictureDatas[i];
                pic.id = (i + 1).ToString();
                temp.SetData(pic);
            }
            else
            {
                temp.DestroyIcon();
            }
        }
    }

    private void OnAddPicture(PictureData pic)
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.PictureDatas.Count * 1.0f / pageCount), 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
        Debug.Log("OnAddPicture:Scroll:" + mScroll.content.childCount + " " + DataManager.Instance.PictureDatas.Count);
        if (page == totalPage - 1)
        {
            PictureTemp temp = mScroll.content.GetChild((DataManager.Instance.PictureDatas.Count - 1) % pageCount).GetComponent<PictureTemp>();
            pic.id = DataManager.Instance.PictureDatas.Count.ToString();
            temp.gameObject.SetActive(true);
            mScroll.content.gameObject.SetActive(false);
            mScroll.content.gameObject.SetActive(true);
            temp.SetData(pic);
            if (DataManager.Instance.PictureDatas.Count % pageCount != 1)
            {
                PictureTemp _temp = mScroll.content.GetChild((DataManager.Instance.PictureDatas.Count - 2) % pageCount).GetComponent<PictureTemp>();
                _temp.UpdateIndex(DataManager.Instance.GetPictureIndex(_temp.Id));
            }
        }
        else if (page == totalPage - 2 && DataManager.Instance.PictureDatas.Count % pageCount == 1)
        {
            PictureTemp temp = mScroll.content.GetChild((DataManager.Instance.PictureDatas.Count - 2) % pageCount).GetComponent<PictureTemp>();
            temp.UpdateIndex(DataManager.Instance.GetPictureIndex(temp.Id));
        }
    }

    private void OnDelPicture(PictureTemp temp)
    {        
        if (DataManager.Instance.PictureDatas.Count % pageCount == 0 || page != totalPage - 1)
        {
            if (page == totalPage - 1 && page != 0) page--;
            totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.PictureDatas.Count * 1.0f / pageCount), 1);
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
            SetPagePicture();
        }
        else {
            temp.gameObject.SetActive(false);
            temp.transform.parent = null;
            temp.transform.parent = mScroll.content;
            for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
            {
                PictureTemp _temp = mScroll.content.GetChild(i % pageCount).GetComponent<PictureTemp>();
                if (_temp.gameObject.activeSelf)
                {
                    PictureData data = DataManager.Instance.PictureDatas[i];
                    data.id = (i + 1).ToString();
                    _temp.Id = data.id;
                    _temp.UpdateIndex(DataManager.Instance.GetPictureIndex(_temp.Id));
                }
            }
        }
    }

    private void OnUpPicture(PictureTemp temp)
    {
        int index = DataManager.Instance.GetPictureIndex(temp.Id);
        PictureData data = DataManager.Instance.GetPictureData(index - 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % pageCount != 0)
        {
            PictureTemp _temp = mScroll.content.GetChild(index% pageCount - 1).GetComponent<PictureTemp>();
            PictureData _data = DataManager.Instance.GetPictureData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index - 1);
        }
        DataManager.Instance.ExchangePictureWithPre(index);
    }

    private void OnDownPicture(PictureTemp temp)
    {
        int index = DataManager.Instance.GetPictureIndex(temp.Id);
        PictureData data = DataManager.Instance.GetPictureData(index + 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % (pageCount - 1) != 0 || index == 0)
        {
            PictureTemp _temp = mScroll.content.GetChild(index% pageCount + 1).GetComponent<PictureTemp>();
            PictureData _data = DataManager.Instance.GetPictureData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index+1);
        }
        DataManager.Instance.ExchangePictureWithNext(index);
    }
    private void OnEditPicture(PictureData picture)
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            PictureTemp temp = mScroll.content.GetChild(i).GetComponent<PictureTemp>();
            if (temp.Id == picture.id)
            {
                temp.SetData(picture);
                break;
            }
        }
    }

    public void OnEditCategory()
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            PictureTemp temp = mScroll.content.GetChild(i).GetComponent<PictureTemp>();
            if (temp.gameObject.activeSelf) temp.UpdateCategory();
        }
    }

    public void OnAddClick()
    {
        PictureSetEdit edit = UIManager.Instance.PushUI(UI.PictureEdit).GetComponent<PictureSetEdit>();
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
        int.TryParse(mPageInput.text, out _page);
        if (_page >= 1 && _page <= totalPage)
        {
            page = _page - 1;
            SetPagePicture();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }

    public void OnEditClick()
    {
       UIManager.Instance.PushUI(UI.PictureCategory);
    }

    public void OnPrePage()
    {
        page--;
        SetPagePicture();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }

    public void OnNextPage()
    {
        page++;
        SetPagePicture();
        mPreBtn.enabled = true;
        mNextBtn.enabled = page < (totalPage - 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
}
