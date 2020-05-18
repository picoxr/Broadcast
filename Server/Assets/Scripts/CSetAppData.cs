using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSetAppData : MonoBehaviour {

    public Text mText;
    public RawImage mRawImage;
    public Image mIcon;
    public GameObject mClose;
    public Vector2 mSizeDelta;
    void Start()
    {
        mSizeDelta = mIcon.GetComponent<RectTransform>().sizeDelta;
    }
    public AppData Data
    {
        get; set;
    }
    public void SetText(string value)
    {
        mText.text = value;
    }
    public void SetIcon(Sprite value)
    {
        mIcon.sprite = value;
        ShowIcon(true);
    }

    public void ShowIcon(bool value)
    {
        mRawImage.gameObject.SetActive(!value);
        mIcon.gameObject.SetActive(value);
    }
    public void ShowClose(bool value)
    {
        mClose.SetActive(value);
    }
    public bool ActiveClose()
    {
        return mClose.activeSelf;
    }

    public void DestroyIcon()
    {
        mIcon.sprite = null;
        //if (mIcon.sprite != null && mIcon.sprite.texture != null)
        //{
        //    DestroyImmediate(mIcon.sprite.texture);
        //    DestroyImmediate(mIcon.sprite);
        //}
    }
}
