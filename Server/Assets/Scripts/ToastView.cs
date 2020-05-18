using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastView : MonoBehaviour {

    public UnityEngine.Events.UnityAction<int> OnClick;

    [SerializeField]
    Text mText;
	// Use this for initialization
	void OnEnable () {
        OnClick = null;
    }

    public void SetText(string text)
    {
        mText.text = text;
    }

    public void OnConfirmClick()
    {
        UIManager.Instance.PopUI();
        if (OnClick != null) OnClick(0);
    }

    public void OnCancleClick()
    {
        UIManager.Instance.PopUI();
        if (OnClick != null) OnClick(1);
    }
}
