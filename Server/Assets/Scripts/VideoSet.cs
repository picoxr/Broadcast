using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VideoSet : MonoBehaviour {

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

        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.VideoDatas.Count * 1.0f / pageCount), 1);
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform rect = Instantiate(mTemplate);
            rect.SetParent(mScroll.content);
            rect.gameObject.SetActive(false);
            VideoTemp temp = rect.GetComponent<VideoTemp>();
            temp.OnDelVideo = OnDelVideo;
            temp.OnUpVideo = OnUpVideo;
            temp.OnDownVideo = OnDownVideo;

        }
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}",page + 1, totalPage);
        SetPageVideo();
    }

    void OnEnable()
    {
        DataManager.OnAddVideo += OnAddVideo;
        DataManager.OnEditVideo += OnEditVideo;
        DataManager.OnEditCategory += OnEditCategory;

    }
    void OnDisable()
    {
        DataManager.OnAddVideo -= OnAddVideo;
        DataManager.OnEditVideo -= OnEditVideo;
        DataManager.OnEditCategory -= OnEditCategory;
    }

    private void SetPageVideo()
    {
        int count = DataManager.Instance.VideoDatas.Count < (page + 1) * pageCount ? DataManager.Instance.VideoDatas.Count : (page + 1) * pageCount;
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            VideoTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<VideoTemp>();
            temp.gameObject.SetActive(i < count);
        }
        mScroll.gameObject.SetActive(false);
        mScroll.gameObject.SetActive(true);
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            VideoTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<VideoTemp>();
            if (i < count)
            {
                VideoData video = DataManager.Instance.VideoDatas[i];
                video.id = (i + 1).ToString();
                temp.SetData(video);
            }
            else
            {
                temp.DestroyIcon();
            }
        }
    }

    private void OnAddVideo(VideoData video)
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.VideoDatas.Count * 1.0f / pageCount), 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
        if (page == totalPage - 1)
        {
            VideoTemp temp = mScroll.content.GetChild((DataManager.Instance.VideoDatas.Count - 1) % pageCount).GetComponent<VideoTemp>();
            video.id = DataManager.Instance.VideoDatas.Count.ToString();
            temp.gameObject.SetActive(true);
            mScroll.content.gameObject.SetActive(false);
            mScroll.content.gameObject.SetActive(true);
            temp.SetData(video);
            if (DataManager.Instance.VideoDatas.Count % pageCount != 1)
            {
                VideoTemp _temp = mScroll.content.GetChild((DataManager.Instance.VideoDatas.Count - 2) % pageCount).GetComponent<VideoTemp>();
                _temp.UpdateIndex(DataManager.Instance.GetVideoIndex(_temp.Id));
            }
        }
        else if (page == totalPage - 2 && DataManager.Instance.VideoDatas.Count % pageCount == 1)
        {
            VideoTemp temp = mScroll.content.GetChild((DataManager.Instance.VideoDatas.Count - 2) % pageCount).GetComponent<VideoTemp>();
            temp.UpdateIndex(DataManager.Instance.GetVideoIndex(temp.Id));
        }
    }

    private void OnDelVideo(VideoTemp temp)
    {
        if (DataManager.Instance.VideoDatas.Count % pageCount == 0 || page != totalPage - 1)
        {
            if (page == totalPage - 1 && page != 0) page--;
            totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.VideoDatas.Count * 1.0f / pageCount), 1);
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
            SetPageVideo();
        }
        else {

            temp.gameObject.SetActive(false);
            temp.transform.parent = null;
            temp.transform.parent = mScroll.content;
            for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
            {
                VideoTemp _temp = mScroll.content.GetChild(i % pageCount).GetComponent<VideoTemp>();
                if (_temp.gameObject.activeSelf)
                {
                    VideoData data = DataManager.Instance.VideoDatas[i];
                    data.id = (i + 1).ToString();
                    _temp.Id = data.id;
                    _temp.UpdateIndex(DataManager.Instance.GetVideoIndex(_temp.Id));
                }
            }
        }
    }

    private void OnUpVideo(VideoTemp temp)
    {
        int index = DataManager.Instance.GetVideoIndex(temp.Id);
        VideoData data = DataManager.Instance.GetVideoData(index - 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % pageCount != 0)
        {
            VideoTemp _temp = mScroll.content.GetChild(index% pageCount - 1).GetComponent<VideoTemp>();
            VideoData _data = DataManager.Instance.GetVideoData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index -1 );
        }
        DataManager.Instance.ExchangeVideoWithPre(index);
    }

    private void OnDownVideo(VideoTemp temp)
    {
        int index = DataManager.Instance.GetVideoIndex(temp.Id);
        VideoData data = DataManager.Instance.GetVideoData(index + 1);
        string id = data.id;
        data.id = temp.Id;
        temp.SetData(data);
        temp.UpdateIndex(index);
        if (index % (pageCount - 1) != 0 || index == 0)
        {
            VideoTemp _temp = mScroll.content.GetChild(index% pageCount + 1).GetComponent<VideoTemp>();
            VideoData _data = DataManager.Instance.GetVideoData(index);
            _data.id = id;
            _temp.SetData(_data);
            _temp.UpdateIndex(index + 1);
        }
        DataManager.Instance.ExchangeVideoWithNext(index);
    }

    public void OnEditClick()
    {
        UIManager.Instance.PushUI(UI.VideoCategory);
    }

    private void OnEditVideo(VideoData video)
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            VideoTemp temp = mScroll.content.GetChild(i).GetComponent<VideoTemp>();
            if (temp.Id == video.id)
            {
                temp.SetData(video);
                break;
            }
        }
    }

    private void OnEditCategory()
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            VideoTemp temp = mScroll.content.GetChild(i).GetComponent<VideoTemp>();
            if (temp.gameObject.activeSelf) temp.UpdateCategory();
        }
    }

    public void OnAddClick()
    {
        VideoSetEdit edit = UIManager.Instance.PushUI(UI.VideoEdit).GetComponent<VideoSetEdit>();
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
            SetPageVideo();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }

    public void OnPrePage()
    {
        page--;
        SetPageVideo();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }

    public void OnNextPage()
    {
        page++;
        SetPageVideo();
        mPreBtn.enabled = true;
        mNextBtn.enabled = page < (totalPage - 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }
}
