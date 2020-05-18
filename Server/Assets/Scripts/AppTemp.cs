using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppTemp : MonoBehaviour {

    [SerializeField]
    Text mId;
    [SerializeField]
    Text mName;
    [SerializeField]
    Text mPackage;
    [SerializeField]
    Text mCategory;
    [SerializeField]
    Image mImage;
    [SerializeField]
    Button mUpBtn;
    [SerializeField]
    Button mDownBtn;

    public UnityEngine.Events.UnityAction<AppTemp> OnDelApp;
    public UnityEngine.Events.UnityAction<AppTemp> OnUpApp;
    public UnityEngine.Events.UnityAction<AppTemp> OnDownApp;
    private AppData mModel;
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

    public void SetData(AppData model)
    {
        mModel = model;
        mId.text = model.id;
        mName.text = model.name;
        mPackage.text = model.pkgname;
        mCategory.text = DataManager.Instance.GetCategory(DataType.app,model.category);
        DestroyIcon();
        mUpBtn.gameObject.SetActive(DataManager.Instance.GetAppIndex(model.id) != 0);
        mDownBtn.gameObject.SetActive(DataManager.Instance.GetAppIndex(model.id) != DataManager.Instance.GetAppMaxIndex());
        StartCoroutine(LoadAppSprite(System.IO.Path.Combine(DataManager.Instance.AppPosterPath, model.picture)));
    }

    public void UpdateCategory()
    {
        mCategory.text = DataManager.Instance.GetCategory(DataType.app, mModel.category);
    }

    public void UpdateIndex(int index)
    {
        mUpBtn.gameObject.SetActive(index != 0);
        mDownBtn.gameObject.SetActive(index != DataManager.Instance.GetAppMaxIndex());
    }

    public void OnEditClick()
    {
        AppSetEdit edit = UIManager.Instance.PushUI(UI.AppEdit).GetComponent<AppSetEdit>();
        edit.SetData(mModel);
        edit.Add = false;
    }

    public void OnDelClick()
    {
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Is it sure to delete the application?");
        toast.OnClick = (idx) =>
        {
            if (idx == 0)
            {
                DataManager.Instance.DelApp(mModel);
                if (OnDelApp != null) OnDelApp(this);
            }
        };
    }

    private IEnumerator LoadAppSprite(string url)
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

    public void OnUpClick()
    {
        if (OnUpApp != null) OnUpApp(this);
    }

    public void OnDownClick()
    {
        if (OnDownApp != null) OnDownApp(this);
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
