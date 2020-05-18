using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class AppCategoryEdit : MonoBehaviour {

    [SerializeField]
    InputField mInput;
    [SerializeField]
    ScrollRect mRect;
    [SerializeField]
    RectTransform mTemplate;

    private SortedDictionary<string, string> mCategories;

    void OnEnable() {

        mCategories = DataManager.Instance.AppCategories;
        mInput.text = "";
        for (int i = 0; i < mRect.content.childCount; i++)
        {
            mRect.content.GetChild(i).gameObject.SetActive(false);
        }
        int count = 1;
        foreach (KeyValuePair<string,string> category in mCategories)
        {
            AddCategory(category.Value, count++,category.Key);
        }
        mRect.verticalScrollbar.value = 0f;
        mRect.content.anchoredPosition = new Vector2(0, 0);
    }

    private void AddCategory(string value, int index,string id)
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

    //private void OnEndEdit(string value)
    //{
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        return;
    //    }
    //    else if (Categories.Contains(value))
    //    {
    //        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
    //        toast.SetText("OnEndEdit:Category already exists");
    //        return;
    //    }
    //    Categories.Add(value);
    //    AddCategory(value, mRect.content.childCount);
    //}

    public void OnDelAction(GameObject obj, string id)
    {
        bool inUse = DataManager.Instance.AppCategoryInUse(mCategories[id]);
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

    public void OnEditAction(string id,string value)
    {
        mCategories[id] = value;
    }

    public void OnConfirmClick()
    {
        DataManager.Instance.UpdateCategories(DataType.app,mCategories);
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
            string newId = DataManager.Instance.GetNewCategoryId(DataType.app);
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
