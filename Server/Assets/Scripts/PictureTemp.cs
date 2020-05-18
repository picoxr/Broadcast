using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureTemp : MonoBehaviour {

    [SerializeField]
    Text mIndex;
    [SerializeField]
    Text mName;
    [SerializeField]
    Text mCategory;
    [SerializeField]
    Image mImage;
    [SerializeField]
    Button mUpBtn;
    [SerializeField]
    Button mDownBtn;

    public UnityEngine.Events.UnityAction<PictureTemp> OnDelPicture;
    public UnityEngine.Events.UnityAction<PictureTemp> OnUpPicture;
    public UnityEngine.Events.UnityAction<PictureTemp> OnDownPicture;
    private PictureData mModel;
    public string Id
    {
        set
        {
            mModel.id = value;
            mIndex.text = value;
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

    public void SetData(PictureData model)
    {
        mModel = model;
        mIndex.text = model.id;
        mName.text = model.name;
        mCategory.text = DataManager.Instance.GetCategory(DataType.picture, model.category);
        DestroyIcon();
        mUpBtn.gameObject.SetActive(DataManager.Instance.GetPictureIndex(model.id) != 0);
        mDownBtn.gameObject.SetActive(DataManager.Instance.GetPictureIndex(model.id) != DataManager.Instance.GetPictureMaxIndex());
        StartCoroutine(LoadPictureSprite(System.IO.Path.Combine(DataManager.Instance.PicturePosterPath, model.posterURL)));
    }

    public void UpdateIndex(int index)
    {
        mUpBtn.gameObject.SetActive(index != 0);
        mDownBtn.gameObject.SetActive(index != DataManager.Instance.GetPictureMaxIndex());
    }

    public void OnEditClick()
    {
        PictureSetEdit edit = UIManager.Instance.PushUI(UI.PictureEdit).GetComponent<PictureSetEdit>();
        edit.SetData(mModel);
        edit.Add = false;
    }

    public void UpdateCategory()
    {
        mCategory.text = DataManager.Instance.GetCategory(DataType.picture, mModel.category);
    }

    public void OnDelClick()
    {
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Do you want to delete the panorama?");
        toast.OnClick = (idx) =>
        {
            if (idx == 0)
            {
                DataManager.Instance.DelPicture(mModel);
                if (OnDelPicture != null) OnDelPicture(this);
            }
        };
    }

    public void OnUpClick()
    {
        if (OnUpPicture != null) OnUpPicture(this);
    }

    public void OnDownClick()
    {
        if (OnDownPicture != null) OnDownPicture(this);
    }

    private IEnumerator LoadPictureSprite(string url)
    {
        WWW www = new WWW(Tools.GetUrl(url));
        yield return www;
        if (www.isDone && www.error == null)
        {
            mImage.gameObject.SetActive(true);
            Texture2D tex = www.texture;
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
