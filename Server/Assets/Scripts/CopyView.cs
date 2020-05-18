using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyView : MonoBehaviour {
    [SerializeField]
    Slider mSlider;
    [SerializeField]
    Text mPercent;
    void OnEnable()
    {
        mSlider.value = 0;
        mPercent.text = "0%";
        InvokeRepeating("OnUpdateCopy", 0, 0.5f);
    }
    public void SetSlider(int value)
    {
        if (value > 100) value = 100;
        mSlider.value = value;
        mPercent.text = value + "%";
    }

    void OnUpdateCopy()
    {
        SetSlider(Tools.Instance.percent);
        if (Tools.Instance.percent >= 100)
        {
            CancelInvoke("OnUpdateCopy");
            Invoke("CopyOver", 1.0f);
        }
    }
    void CopyOver()
    {
        UIManager.Instance.PopUI();
    }
}
