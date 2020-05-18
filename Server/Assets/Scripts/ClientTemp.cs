using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientTemp : MonoBehaviour {

    [SerializeField]
    Text mId;
    [SerializeField]
    Text mName;
    [SerializeField]
    Text mCategory;
    [SerializeField]
    Button mUpBtn;
    [SerializeField]
    Button mDownBtn;
    public UnityEngine.Events.UnityAction<ClientTemp> OnDelClient;
    public UnityEngine.Events.UnityAction<ClientTemp> OnUpClient;
    public UnityEngine.Events.UnityAction<ClientTemp> OnDownClient;
    private ClientData mModel;
    // Use this for initialization

    public int Index
    {
        get; set;
    }
    public string Id
    {
        set
        {
            mModel.id = value;
            mId.text = value;
        }
        get { return mModel.id; }
    }
    void Start()
    {
        transform.localScale = Vector3.one;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetData(ClientData model)
    {
        mModel = model;
        mId.text = model.id;
        mName.text = model.sn;
        mCategory.text = DataManager.Instance.GetCategory(DataType.client, model.category);
        mUpBtn.gameObject.SetActive(DataManager.Instance.GetClientIndex(model.id) != 0);
        mDownBtn.gameObject.SetActive(DataManager.Instance.GetClientIndex(model.id) != DataManager.Instance.GetClientMaxIndex());
    }

    public void UpdateIndex(int index)
    {
        mUpBtn.gameObject.SetActive(index != 0);
        mDownBtn.gameObject.SetActive(index != DataManager.Instance.GetClientMaxIndex());
    }

    public void UpdateCategory()
    {
        mCategory.text = DataManager.Instance.GetCategory(DataType.client, mModel.category);
    }

    public void OnEditClick()
    {
        ClientSetEdit edit = UIManager.Instance.PushUI(UI.ClientEdit).GetComponent<ClientSetEdit>();
        edit.SetData(mModel,Index);
        edit.Add = false;
    }

    public void OnDelClick()
    {
        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        toast.SetText("Is it sure to delete the client?");
        toast.OnClick = (idx) =>
        {
            if (idx == 0)
            {
                DataManager.Instance.DelClient(mModel);
                if (OnDelClient != null) OnDelClient(this);
            }
        };
    }

    public void OnUpClick()
    {
        if (OnUpClient != null) OnUpClient(this);
    }

    public void OnDownClick()
    {
        if (OnDownClient != null) OnDownClient(this);
    }
}
