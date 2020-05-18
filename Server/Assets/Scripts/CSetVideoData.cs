using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSetVideoData : MonoBehaviour {

    public Text mText;
    public Image mType;
    public Image mIcon;
    public RawImage mRawImage;
    public Vector2 mSizeDelta;
    void Start()
    {
        mSizeDelta = mIcon.GetComponent<RectTransform>().sizeDelta;
    }
    public VideoData Data
    {
        get; set;
    }

    public void SetText(string value)
    {
        mText.text = value;
        Debug.Log("SetText:Tex:" + System.Text.Encoding.Default.GetBytes(value).Length);
    }
    public void SetType(Sprite value)
    {
        mType.sprite = value;
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
