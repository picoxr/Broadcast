using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class VideoCategoryEdit : MonoBehaviour {

    [SerializeField]
    InputField mInput;
    [SerializeField]
    ScrollRect mRect;
    [SerializeField]
    RectTransform mTemplate;

    private SortedDictionary<string, string> mCategories;
    //private VerticalLayoutGroup mGroup;
    void OnEnable() {


        mCategories = DataManager.Instance.VideoCategories;
        mInput.text = "";
        for (int i = 0; i < mRect.content.childCount; i++)
        {
            mRect.content.GetChild(i).gameObject.SetActive(false);
        }
        int count = 1;
        foreach (KeyValuePair<string, string> category in mCategories)
        {
            AddCategory(category.Value, count++, category.Key);
        }
        mRect.verticalScrollbar.value = 0f;
        mRect.content.anchoredPosition = new Vector2(0, 0);
        //mRect.content.rect.height = Categories.Count * mGroup.spacing;
    }

    private void AddCategory(string value, int index, string id)
    {
        RectTransform rect = null;
        if (mRect.content.childCount > index)
        {
            rect = mRect.content.GetChild(index).GetComponent<RectTransform>();
        }
        else
        {
            rect = Instantiate(mTemplate);
        }
        rect.SetParent(mRect.content);
        rect.gameObject.SetActive(true);
        CategoryTemp tem = rect.GetComponent<CategoryTemp>();
        tem.index = index;
        tem.Id = id;
        tem.SetName(value);
        tem.DelAction = OnDelAction;
        tem.EditAction = OnEditAction;
    }
    //void CenterToSelected(RectTransform target)
    //{
    //    Vector3 maskCenterPos = mRect.viewport.position + (Vector3)mRect.viewport.rect.center;
    //    Vector3 itemCenterPos = target.position;
    //    Vector3 difference = maskCenterPos - itemCenterPos;
    //    difference.z = 0;
    //    Vector3 newPos = mRect.content.position + difference;
    //    DOTween.To(() => mRect.content.position, x => mRect.content.position = x, newPos, 5);
    //}

    //private void OnEndEdit(string value)
    //{
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        return;
    //    }
    //    else if (Categories.Contains(value))
    //    {
    //        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
    //        toast.SetText("Category already exists");
    //        return;
    //    }
    //    Categories.Add(value);
    //    AddCategory(value, mRect.content.childCount);
    //}

    public void OnDelAction(GameObject obj, string id)
    {
        bool inUse = DataManager.Instance.VideoCategoryInUse(id);
        if (!inUse)
        {
            DestroyImmediate(obj);
            mCategories.Remove(id);
            for (int i = 0; i < mRect.content.childCount; i++)
            {
                CategoryTemp temp = mRect.content.GetChild(i).GetComponent<CategoryTemp>();
                temp.index = i + 1;
            }
        }
        else
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The classification is available and can not be deleted");
        }
    }

    public void OnEditAction(string id, string value)
    {
        mCategories[id] = value;
    }

    public void OnConfirmClick()
    {
        DataManager.Instance.UpdateCategories(DataType.Video, mCategories);
        UIManager.Instance.PopUI();
    }
    public void OnCancelClick()
    {
        UIManager.Instance.PopUI();
    }
    public void OnAddClick()
    {
        if (string.IsNullOrEmpty(mInput.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Classification can not be empty");
        }
        else if (mCategories.ContainsValue(mInput.text) || mInput.text.Equals("Ungrouped") || mInput.text.Equals("All Groups"))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("This type has already existed");
        }
        else
        {
            string newId = DataManager.Instance.GetNewCategoryId(DataType.Video);
            AddCategory(mInput.text, mCategories.Count + 1, newId);
            mCategories[newId] = mInput.text;
            mInput.text = "";
            Invoke("ScrollToEnd", 0.1f);
        }
    }
    void ScrollToEnd()
    {
        if (mRect.content.childCount > 2)
        {
            mRect.verticalScrollbar.value = 1.0f;
            mRect.content.anchoredPosition = new Vector2(0, (mRect.content.childCount - 2) * 40);
        }
    }
    public void OnValueChanged()
    {
        string str = Tools.SplitNameByASCII(mInput.text.Trim());
        mInput.text = str;
    }
}
