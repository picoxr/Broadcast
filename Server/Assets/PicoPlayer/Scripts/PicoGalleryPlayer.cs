using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PicoGalleryPlayer : MonoBehaviour
{
    public static string FileName = string.Empty;
    public static string PictureName = string.Empty;

    public GameObject FlatScreen;
    public GameObject PanoramaScreen;

    public GameObject PlayRoot;
    public Text VideoNameText;
    public Toggle FlatToggle;
    public Toggle PanaramaToggle;
    public Texture DummyTex;
    public GameObject ExceptionDialog;//ExceptionArea

    private GameObject mainCanvas = null;
    private Texture2D _leftTexture = null;
    private Texture2D _rightTexture = null;

    private Server m_Server;

    private bool _isPlaying = false;
    private bool _isRestart = false;

    private bool m_bActiveDrag = true;
    private bool m_bUpdate = true;
    private float m_fDragTime = 0.2f;
    private float m_fDeltaTime = 0.0f;

    private PlayerAPI.Video3DMode _video3DMode = PlayerAPI.Video3DMode.Off;
    private PlayerAPI.LocalOnline _localOnline = PlayerAPI.LocalOnline.Local;

    private IntPtr intPtr = IntPtr.Zero;

    public int _videoWidth = 0;
    public int _videoHeight = 0;

    //User list UI
    private Transform gaUserListUI;
    //private Transform trTemp;

    //Current timestamp
    private string GetTimeStamp
    {
        get
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }

    private void Awake()
    {
        m_Server = GameObject.Find("Manager").GetComponent<Server>();
        mainCanvas = GameObject.Find("Canvas");

        //gaUserListUI = GameObject.Find("CanvasUser");
        GameObject gaUserFather = GameObject.Find("MoverUserlist");
        gaUserListUI = PlayerUtils.Find(gaUserFather.transform, "CanvasUser");
        gaUserListUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        //FileName = "D:\\sample_old.mp4";
        //FileName = "http://c86f1omm.vod2.hongshiyun.net/target/hls/2017/07/31/559_20389806e93f4e1cb0a65a9138fcf561_10_1920x960.m3u8";
        mainCanvas.SetActive(false);
        VideoNameText.text = PictureName;
        _localOnline = PlayerAPI.LocalOnline.Local;
        StartPicture();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Application.Quit();
        }
        UpdateUI();
    }

    /// <summary>
    /// Load picture
    /// </summary>
    private void StartPicture()
    {
        if (!PlayerPrefs.HasKey(FileName))
            PlayerPrefs.SetInt(FileName, 1);
        //StartCoroutine(LoadPicture(FileName));
        LoadByIO(FileName);
        if (!m_Server.IsFree)
            m_Server.fnPlayPicture();
        StartCoroutine(UICanvas3sHidden());
        //if (_videoWidth > 0 && _videoWidth <= 4096 && _videoHeight > 0 && _videoHeight <= 4096)
        //{
        //    if (_leftTexture != null)
        //    {
        //        Destroy(_leftTexture);
        //        _leftTexture = null;
        //    }
        //    if (_rightTexture != null)
        //    {
        //        Destroy(_rightTexture);
        //        _rightTexture = null;
        //    }
        //    if (_leftTexture == null)
        //    {
        //        _leftTexture = new Texture2D(_videoWidth, _videoHeight, TextureFormat.BGRA32, false);
        //        _leftTexture.hideFlags = HideFlags.HideAndDontSave;
        //        _leftTexture.wrapMode = TextureWrapMode.Clamp;
        //        _leftTexture.filterMode = FilterMode.Trilinear;
        //        _leftTexture.anisoLevel = 9;
        //    }
        //    if (_rightTexture == null)
        //    {
        //        _rightTexture = new Texture2D(_videoWidth, _videoHeight, TextureFormat.BGRA32, false);
        //        _rightTexture.hideFlags = HideFlags.HideAndDontSave;
        //        _rightTexture.wrapMode = TextureWrapMode.Clamp;
        //        _rightTexture.filterMode = FilterMode.Trilinear;
        //        _rightTexture.anisoLevel = 9;
        //    }
        //    int i = PlayerAPI.SetTexturePointers(_session,
        //        _leftTexture != null ? _leftTexture.GetNativeTexturePtr() : IntPtr.Zero,
        //        _rightTexture != null ? _rightTexture.GetNativeTexturePtr() : IntPtr.Zero);
        //    if (i == 0)
        //    {
        //        Play();
        //        float va = PlayerAPI.VolumeDown(_session, 0);
        //        if (DataManager.system_volume > va)
        //        {
        //            PlayerAPI.VolumeUp(_session, (int)(DataManager.system_volume - va));
        //        }
        //        else if (DataManager.system_volume < va)
        //        {
        //            PlayerAPI.VolumeDown(_session, (int)(va - DataManager.system_volume));
        //        }

        //        //Send server instructions only in online mode
        //        if (!m_Server.IsFree)
        //            m_Server.fnPlayVideo();
        //        StartCoroutine(UICanvas3sHidden());
        //    }
        //}
    }

    /// <summary>
    /// video rendering
    /// </summary>
    private void UpdatePicture()
    {
        if (PanoramaScreen != null)
        {
            if (PanoramaScreen.GetComponent<MeshRenderer>() != null)
            {
                if (PanoramaScreen.GetComponent<MeshRenderer>().material.mainTexture != _leftTexture)
                {
                    PanoramaScreen.GetComponent<MeshRenderer>().material.mainTexture = _leftTexture;
                }
            }
        }
        if (FlatScreen != null)
        {
            if (FlatScreen.GetComponent<RawImage>() != null)
            {
                if (FlatScreen.GetComponent<RawImage>().texture != _leftTexture)
                {
                    FlatScreen.GetComponent<RawImage>().texture = _leftTexture;
                }
            }
        }
    }

    /// <summary>
    /// UI rendering
    /// </summary>
    private void UpdateUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("UpdateUI:Current touch on UI");
            transform.GetComponent<SimpleRotateSphere>().enabled = false;
        }
        else
        {
            //Debug.Log("UpdateUI:No touch on UI at present");
            transform.GetComponent<SimpleRotateSphere>().enabled = true;
        }
        if (_isRestart && !PlayRoot.activeSelf)
            PlayRoot.SetActive(true);
    }

    /// <summary>
    ///Destroy player
    /// </summary>
    private void OnDestroy()
    {
        PlayerPrefs.DeleteKey(FileName);
        FileName = string.Empty;
        PictureName = string.Empty;
        //Reclaim memory of texture2d
        DestroyImmediate(_leftTexture);
        GC.Collect();
        Debug.Log("OnDestroy:PicoMediaPlayer: OnDestroy!");
    }

    #region Operation method of broadcast control UI

    /// <summary>
    /// Switch panorama / normal player
    /// </summary>
    public void ChangeScreenToggle()
    {
        //Camera.main.transform.eulerAngles = Vector3.zero;
        if (PanaramaToggle.isOn)
        {
            FlatScreen.SetActive(false);
            PanoramaScreen.SetActive(true);
        }
        if (FlatToggle.isOn)
        {
            PanoramaScreen.SetActive(false);
            FlatScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Play control UI will be hidden if there is no operation for more than 3 seconds
    /// </summary>
    /// <returns></returns>
    private IEnumerator UICanvas3sHidden()
    {
        yield return new WaitForSeconds(3f);
        //if (!EventSystem.current.IsPointerOverGameObject())
        //    PlayRoot.SetActive(false);
    }

    /// <summary>
    /// Move the mouse to the bottom of the screen to display the broadcast control UI
    /// </summary>
    public void UICanvasVisible()
    {
        PlayRoot.SetActive(true);
    }

    /// <summary>
    /// Hide broadcast control UI
    /// </summary>
    public void UICanvasHidden()
    {
        //if (!IsRestart)
        StartCoroutine(UICanvas3sHidden());
    }

    /// <summary>
    ///Show client list
    /// </summary>
    public void ShowDeviceUserView()
    {
        if (!gaUserListUI.gameObject.activeSelf)
        {
            gaUserListUI.gameObject.SetActive(true);
        }
        else
        {
            gaUserListUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Exit picture player
    /// </summary>
    public void ExitButton()
    {
        //Send server instructions only in online mode
        if (!m_Server.IsFree)
            //Send stop command
            m_Server.fnStopPicture();

        Camera.main.transform.rotation = Quaternion.Euler(Vector3.zero);
        mainCanvas.SetActive(true);
        Destroy(gameObject);
    }

    /// <summary>
    /// Method called in case of video playing error, click to return to the main interface
    /// </summary>
    public void ExceptionReturnMenuBtnClick()
    {
        if (ExceptionDialog.activeSelf)
            ExceptionDialog.SetActive(false);
        ExitButton();
    }

    #endregion Operation method of broadcast control UI

    private IEnumerator LoadPicture(string path)
    {
        WWW www = new WWW(GetUrl(path));
        yield return www;
        if (www.isDone)
        {
            if (www.error == null || www.error == "")
            {
                _leftTexture = www.texture;
                //Reclaim the occupied memory of texture2d immediately
                DestroyImmediate(www.texture);
                UpdatePicture();
            }
        }
    }

    /// <summary>
    /// Load in io mode
    /// </summary>
    private void LoadByIO(string url)
    {
        //Create file read stream
        FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //Create file length buffer
        byte[] bytes = new byte[fileStream.Length];
        //read file
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //Release file read stream
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //Create texture
        _leftTexture = new Texture2D(1, 1);
        _leftTexture.LoadImage(bytes);
        UpdatePicture();
    }

    private string GetUrl(string path)
    {
        string url = null;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                if (path.Contains(@"jar:file://"))
                    url = path;
                else
                    url = @"file://" + path;
                break;
            default:
                url = @"file://" + path;
                break;
        }
        return url.Replace("\\","/");
    }
}