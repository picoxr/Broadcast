using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VideoTemp : MonoBehaviour
{

    [SerializeField]
    Text mId;
    [SerializeField]
    Text mName;
    [SerializeField]
    Text mCategory;
    [SerializeField]
    Text mType;
    [SerializeField]
    Image mImage;
    [SerializeField]
    Button mUpBtn;
    [SerializeField]
    Button mDownBtn;

    public UnityEngine.Events.UnityAction<VideoTemp> OnDelVideo;
    public UnityEngine.Events.UnityAction<VideoTemp> OnUpVideo;
    public UnityEngine.Events.UnityAction<VideoTemp> OnDownVideo;
    private VideoData mModel;
    public string Id
    {
        set
        {
            mModel.id = value;
            mId.text = value;
        }
        get { return mModel.id; }
    }
    // Use this for initialization
    void Start()
    {

        transform.localScale = Vector3.one;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetData(VideoData model)
    {
        mModel = model;
        mId.text = model.id;
        mName.text = model.name;
        mCategory.text = DataManager.Instance.GetCategory(DataType.Video, model.category);
        mType.text = DataManager.Instance.GetVideoType(model.type);
        DestroyIcon();
        StartCoroutine(LoadVideoSprite(System.IO.Path.Combine(DataManager.Instance.VideoPosterPath, model.picture)));
        mUpBtn.gameObject.SetActive(DataManager.Instance.GetVideoIndex(model.id) != 0);
        mDownBtn.gameObject.SetActive(DataManager.Instance.GetVideoIndex(model.id) != DataManager.Instance.GetVideoMaxIndex());
    }

    public void UpdateIndex(int index)
    {
        mUpBtn.gameObject.SetActive(index != 0);
        mDownBtn.gameObject.SetActive(index != DataManager.Instance.GetVideoMaxIndex());
    }

    public VideoData GetData()
    {
        return mModel;
    }

    public void OnEditClick()
    {
        VideoSetEdit edit = UIManager.Instance.PushUI(UI.VideoEdit).GetComponent<VideoSetEdit>();
        edit.SetData(mModel);
        edit.Add = false;
    }

    public void UpdateCategory()
    {
        mCategory.text = DataManager.Instance.GetCategory(DataType.Video, mModel.category);
    }

    public void OnDelClick()
    {
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Is it sure to delete the video?");
        toast.OnClick = (idx) =>
        {
            if (idx == 0)
            {
                DataManager.Instance.DelVideo(mModel);
                if (OnDelVideo != null) OnDelVideo(this);
            }
        };
    }

    public void OnUpClick()
    {
        if (OnUpVideo != null) OnUpVideo(this);
    }

    public void OnDownClick()
    {
        if (OnDownVideo != null) OnDownVideo(this);
    }

    private IEnumerator LoadVideoSprite(string url)
    {
        WWW www = new WWW(Tools.GetUrl(url));
        yield return www;
        if (www.isDone && www.error == null)
        {
            Texture2D tex = www.texture;
            mImage.gameObject.SetActive(true);
            if (tex.width != 480 || tex.height != 270)
            {
                tex = Tools.ReSetTextureSize(tex, 480, 270);
                System.IO.File.Delete(url);
                Tools.SaveTexture(tex, url);
            }
            Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            mImage.sprite = temp;
        }
    }

    public void DestroyIcon()
    {
        if (mImage.sprite != null)
        {
            DestroyImmediate(mImage.sprite.texture);
            DestroyImmediate(mImage.sprite);
            mImage.sprite = null;
        }
    }
}
