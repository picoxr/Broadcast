using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class ClientSet : MonoBehaviour {

    [SerializeField]
    RectTransform mTemplate;
    [SerializeField]
    ScrollRect mScroll;
    [SerializeField]
    Button mPreBtn;
    [SerializeField]
    Button mNextBtn;
    [SerializeField]
    Text mPage;
    [SerializeField]
    InputField mPageInput;
    int page = 0;
    int pageCount = 10;
    int totalPage;
    // Use this for initialization
    void Start() {

        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.ClientDatas.Count * 1.0f / pageCount), 1);
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform rect = Instantiate(mTemplate);
            rect.SetParent(mScroll.content);
            rect.gameObject.SetActive(false);
            ClientTemp temp = rect.GetComponent<ClientTemp>();
            temp.OnDelClient = OnDelClient;
            temp.OnUpClient = OnUpClient;
            temp.OnDownClient = OnDownClient;
        }
        mPreBtn.enabled = false;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        SetPageClient();
    }

    void OnEnable()
    {
        DataManager.OnAddClient += OnAddClient;
        DataManager.OnEditClient += OnEditClient;
        DataManager.OnAddClients += OnAddClients;
        DataManager.OnEditCategory += OnEditCategory;
    }
    void OnDisable()
    {
        DataManager.OnAddClient -= OnAddClient;
        DataManager.OnEditClient -= OnEditClient;
        DataManager.OnAddClients -= OnAddClients;
        DataManager.OnEditCategory -= OnEditCategory;
    }

    private void SetPageClient()
    {
        int count = DataManager.Instance.ClientDatas.Count < (page + 1) * pageCount ? DataManager.Instance.ClientDatas.Count : (page + 1) * pageCount;
        for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
        {
            ClientTemp temp = mScroll.content.GetChild(i % pageCount).GetComponent<ClientTemp>();
            if (i < count)
            {
                ClientData client = DataManager.Instance.ClientDatas[i];
                temp.gameObject.SetActive(true);
                temp.SetData(client);
                temp.Index = i;
            }
            else
            {
                temp.gameObject.SetActive(false);
            }
        }
        mScroll.gameObject.SetActive(false);
        mScroll.gameObject.SetActive(true);
    }

    private void OnAddClient(ClientData client)
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.ClientDatas.Count * 1.0f / pageCount), 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
        if (page == totalPage - 1)
        {
            ClientTemp temp = mScroll.content.GetChild((DataManager.Instance.ClientDatas.Count - 1) % pageCount).GetComponent<ClientTemp>();
            temp.gameObject.SetActive(true);
            mScroll.content.gameObject.SetActive(false);
            mScroll.content.gameObject.SetActive(true);
            temp.SetData(client);
            temp.Index = DataManager.Instance.ClientDatas.Count - 1;
            if (DataManager.Instance.ClientDatas.Count % pageCount != 1)
            {
                ClientTemp _temp = mScroll.content.GetChild((DataManager.Instance.ClientDatas.Count - 2) % pageCount).GetComponent<ClientTemp>();
                _temp.UpdateIndex(DataManager.Instance.GetClientIndex(_temp.Id));
            }
        }
        else if (page == totalPage - 2 && DataManager.Instance.ClientDatas.Count % pageCount == 1)
        {
            ClientTemp temp = mScroll.content.GetChild((DataManager.Instance.ClientDatas.Count - 2) % pageCount).GetComponent<ClientTemp>();
            temp.UpdateIndex(DataManager.Instance.GetClientIndex(temp.Id));
        }
    }

    private void OnEditClient(ClientData client)
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            ClientTemp temp = mScroll.content.GetChild(i).GetComponent<ClientTemp>();
            if (temp.Id == client.id)
            {
                temp.SetData(client);
                break;
            }
        }
    }

    private void OnDelClient(ClientTemp temp)
    {
        if (DataManager.Instance.ClientDatas.Count % pageCount == 0 || page != totalPage - 1)
        {
            if (page == totalPage - 1 && page != 0) page--;
            totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.ClientDatas.Count * 1.0f / pageCount), 1);
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
            SetPageClient();
        }
        else {
            temp.gameObject.SetActive(false);
            temp.transform.parent = null;
            temp.transform.parent = mScroll.content;
            for (int i = pageCount * page; i < (page + 1) * pageCount; i++)
            {
                ClientTemp _temp = mScroll.content.GetChild(i % pageCount).GetComponent<ClientTemp>();
                if (_temp.gameObject.activeSelf)
                {
                    _temp.UpdateIndex(DataManager.Instance.GetClientIndex(_temp.Id));
                    _temp.Index = i;
                }
            }
        }
    }

    public void OnEditCategory()
    {
        for (int i = 0; i < mScroll.content.childCount; i++)
        {
            ClientTemp temp = mScroll.content.GetChild(i).GetComponent<ClientTemp>();
            if (temp.gameObject.activeSelf) temp.UpdateCategory();
        }
    }

    private void OnUpClient(ClientTemp temp)
    {
        int index = DataManager.Instance.GetClientIndex(temp.Id);
        temp.SetData(DataManager.Instance.GetClientData(index - 1));
        temp.UpdateIndex(index);
        if (index % pageCount != 0)
        {
            ClientTemp _temp = mScroll.content.GetChild(index% pageCount - 1).GetComponent<ClientTemp>();
            _temp.SetData(DataManager.Instance.GetClientData(index));
            _temp.UpdateIndex(index-1);
        }
        DataManager.Instance.ExchangeClientWithPre(index);
    }

    private void OnDownClient(ClientTemp temp)
    {
        int index = DataManager.Instance.GetClientIndex(temp.Id);
        temp.SetData(DataManager.Instance.GetClientData(index + 1));
        temp.UpdateIndex(index);
        if (index % (pageCount-1) != 0 || index == 0)
        {
            ClientTemp _temp = mScroll.content.GetChild(index% pageCount + 1).GetComponent<ClientTemp>();
            _temp.SetData(DataManager.Instance.GetClientData(index));
            _temp.UpdateIndex(index + 1);
        }
        DataManager.Instance.ExchangeClientWithNext(index);
    }

    private void OnAddClients(List<ClientData> _clients)
    {
        totalPage = Math.Max(Mathf.CeilToInt(DataManager.Instance.ClientDatas.Count * 1.0f / pageCount), 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1 && page < (totalPage - 1);
        SetPageClient();
        mScroll.content.gameObject.SetActive(false);
        mScroll.content.gameObject.SetActive(true);
    }

    public void OnAddClick()
    {
        ClientSetEdit edit = UIManager.Instance.PushUI(UI.ClientEdit).GetComponent<ClientSetEdit>();
        edit.Add = true;
        edit.SetData(null);
    }

    public void OnPageClick()
    {
        mPage.gameObject.SetActive(false);
        mPageInput.gameObject.SetActive(true);
        mPageInput.ActivateInputField();
    }

    public void OnEndEdit()
    {
        mPage.gameObject.SetActive(true);
        mPageInput.gameObject.SetActive(false);
        int _page = 1;
        int.TryParse(mPageInput.text, out _page);
        if (_page >= 1 && _page <= totalPage)
        {
            page = _page - 1;
            SetPageClient();
            mPreBtn.enabled = page != 0;
            mNextBtn.enabled = totalPage > 1;
            mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
            mPageInput.text = "";
        }
    }

    public void OnEditClick()
    {
        UIManager.Instance.PushUI(UI.ClientCategory);
    }

    public void OnPrePage()
    {
        page--;
        SetPageClient();
        mPreBtn.enabled = page != 0;
        mNextBtn.enabled = totalPage > 1;
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }

    public void OnNextPage()
    {
        page++;
        SetPageClient();
        mPreBtn.enabled = true;
        mNextBtn.enabled = page < (totalPage - 1);
        mPage.text = string.Format("{0}/{1}", page + 1, totalPage);
    }

    public void OnPutInClick()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "All Files\0*.*\0\0";
        ofn.filter = "File(*.csv)\0*.csv";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = string.IsNullOrEmpty(DataManager.Instance.DefaultPath) ? Application.dataPath : DataManager.Instance.DefaultPath;
        //path = path.Replace('/', '\\');
        //Default path  
        ofn.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Upload file";

        ofn.defExt = "CSV";//Show the type of file  
                           //Note that the following items do not have to be all selected, but 0x00000008 items should not be missing  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            if (!System.IO.File.Exists(ofn.file))
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Files incorrect");
                return;
            }
            Debug.Log("OnPutInClick:" + "Selected file with full path: {0}" + ofn.file);
            //TextAsset binAsset = Resources.Load(ofn.file, typeof(TextAsset)) as TextAsset;
            //Debug.Log(binAsset.text);
            StartCoroutine(LoadWWW("file://" + ofn.file));
            DataManager.Instance.DefaultPath = System.IO.Path.GetDirectoryName(ofn.file);
        }
    }

    IEnumerator LoadWWW(string filepath)
    {
        WWW www = new WWW(filepath.Replace("\\", "/"));

        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            List<List<string>> list = readcsv(www.text,System.Text.Encoding.UTF8);
            List<ClientData> clients = new List<ClientData>();
            foreach (List<string> strs in list)
            {
                if (strs.Count < 2)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("There is something wrong with the device configuration table");
                    yield break;
                }
                else if (string.IsNullOrEmpty(strs[0]))
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("The client ID can not be empty");
                    yield break;
                }
                else if (string.IsNullOrEmpty(strs[1]))
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("The client SN can not be empty");
                    yield break;
                }
                else if (strs[0].Length > 4)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("The Client ID Length than 4");
                    yield break;
                }
                ClientData client = new ClientData();
                client.id = strs[0];
                client.sn = strs[1];
                foreach (ClientData data in clients)
                {
                    if (data.id == client.id)
                    {
                        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                        toast.SetText(string.Format("<color=red>ID:{0}</color> ",data.id) + "Please check and modify the duplicated device SNID");
                        yield break;
                    }
                    else if(data.sn == client.sn){

                        ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                        toast.SetText(string.Format("<color=red>SN:{0}</color> ", data.sn) + "Please check and modify the duplicated device SNID");
                        yield break;
                    }
                }
                int _id = -1;
                int.TryParse(strs[0],out _id);
                if (_id == -1)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Please enter the correct device ID");
                    yield break;
                }
                if (strs.Count > 2)
                {
                    if (!DataManager.Instance.GetCategories(DataType.client).Contains(strs[2]))
                    {
                        //List<string> categories = new List<string>();
                        //categories.AddRange(DataManager.Instance.GetCategories(DataType.client));
                        //categories.Add(strs[2]);
                        string id = DataManager.Instance.AddCategory(DataType.client, strs[2]);
                        client.category = id;
                    }
                    else {

                        client.category = DataManager.Instance.GetCategoryId(DataType.client,strs[2]); ;

                    }
                }
                clients.Add(client);
                ClientData _client = DataManager.Instance.GetClientById(client.id);
                if (_client != null)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Client ID" + client.id + "Already existed");
                    yield break;
                }
                _client = DataManager.Instance.GetClientBySn(client.sn);
                if (_client != null)
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Client SN" + client.sn + "Already existed");
                    yield break;
                }
            }
            DataManager.Instance.AddClients(clients);
        }
    }

    public static List<List<string>> readcsv(string strin, System.Text.Encoding encoding)
    {
        List<List<string>> ret = new List<List<string>>();

        strin = strin.Replace("\r", "");
        string[] lines = strin.Split('\n');

        //if (lines.Length > 0)
        //{
        //    byte[] byt = encoding.GetBytes(lines[0]);
        //    if (byt.Length >= 3 &&
        //        byt[0] == 0xEF &&
        //        byt[1] == 0xBB &&
        //        byt[2] == 0xBF)
        //    {
        //        lines[0] = encoding.GetString(byt, 3, byt.Length - 3);
        //    }
        //}

        for (int i = 0; i < lines.Length; i++)
        {
            if(!string.IsNullOrEmpty(lines[i])) ret.Add(new List<string>(lines[i].Split(',')));
        }
        return ret;
    }
}
