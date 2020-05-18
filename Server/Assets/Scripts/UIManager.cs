using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum UI
{
    VideoSet,
    VideoEdit,
    VideoCategory,
    AppSet,
    AppEdit,
    AppCategory,
    PictureSet,
    PictureEdit,
    PictureCategory,
    ClientSet,
    ClientEdit,
    ClientCategory,
    NetSet,
    NetEdit,
    Toast,
    Copy,

};

public class UIManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> mUIs;
    [SerializeField]
    GameObject mEditBtns;
    [SerializeField]
    CLoadJson mLoadJson;
    List<UI> stack = new List<UI>();
    List<Transform> transforms = new List<Transform>();

    void Start()
    {
        Instance = this;
    }
    public static UIManager Instance;

    private GameObject GetUI(UI value)
    {
        return mUIs[(int)value];
    }

    public GameObject RunUI(UI value)
    {
        transforms.Clear();
        GameObject canvas = GameObject.Find("Canvas");
        for (int i = 0; i < canvas.transform.childCount; i++)
        {
            Transform child = canvas.transform.GetChild(i);
            if (child.name != "BackGround" && child.gameObject.activeSelf)
            {
                transforms.Add(child);
                child.gameObject.SetActive(false);
            }
        }
        mEditBtns.SetActive(true);
        GameObject go = GetUI(value);
        go.SetActive(true);
        stack.Add(value);
        return go;
    }

    public GameObject PushUI(UI value)
    {
        if (stack.Count > 0 && stack[stack.Count - 1] == value) return GetUI(value);
        stack.Add(value);
        GameObject go = GetUI(value);
        go.SetActive(true);
        return go;
    }

    public void PopUI()
    {
        GetUI(stack[stack.Count - 1]).SetActive(false);
        stack.RemoveAt(stack.Count - 1);
        if(stack.Count > 0) GetUI(stack[stack.Count - 1]).SetActive(true);

    }

    public GameObject ReplaceUI(UI value)
    {
        if (stack.Count > 0 && stack[stack.Count - 1] == value) return GetUI(value);
        GetUI(stack[stack.Count - 1]).SetActive(false);
        stack.RemoveAt(stack.Count - 1);
        stack.Add(value);
        GameObject go = GetUI(value);
        go.SetActive(true);
        return go;
    }

    public void PopAllUI()
    {
        mEditBtns.SetActive(false);
        for (int i = 0; i < stack.Count; i++)
        {
            GetUI(stack[i]).SetActive(false);
        }
        stack.Clear();
        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].gameObject.SetActive(true);
        }
        mLoadJson.RefreshList();
    }
}
