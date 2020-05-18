using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class CategoryTemp : MonoBehaviour {

    [SerializeField]
    Text mIdText;
    [SerializeField]
    InputField mName;
    private int mIndex;
    private string mNameText;
    public UnityEngine.Events.UnityAction<GameObject,string> DelAction;
    public UnityEngine.Events.UnityAction<string,string> EditAction;
    public string Id
    {
        get; set;
    }

    public int index
    {
        get
        {
            return mIndex;
        }
        set
        {
            mIndex = value;
            mIdText.text = mIndex.ToString();
        }
    }

    public void SetName(string value)
    {
        mName.text = mNameText = value;
    }
    // Use this for initialization
    void Start () {

        transform.localScale = Vector3.one;
        mName.onEndEdit.AddListener(OnEndEdit);
    }

    public void OnDelClick()
    {
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Is it sure to delete the classification?");
        toast.OnClick = (idx) =>
        {
            if (idx == 0)
            {
                string name = mName.text;
                if (DelAction != null) DelAction(gameObject,Id);
            }
        };
    }

    private void OnEndEdit(string value)
    {
        if (string.IsNullOrEmpty(mName.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Classification can not be empty");
            SetName(mNameText);
            return;
        }
        string str = Tools.SplitNameByASCII(value);
        SetName(str);
        if (EditAction != null) EditAction(Id, str);
    }

    public void OnButtonPointEnter(Image image)
    {
        Texture2D tex = Resources.Load("button_rollOver") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnButtonPointExit(Image image)
    {
        Texture2D tex = Resources.Load("button") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
}
