using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;
using LitJson;

public class VideoSetEdit : MonoBehaviour
{

    [SerializeField]
    InputField mName;
    [SerializeField]
    Dropdown mCategory;
    [SerializeField]
    Dropdown mType;
    [SerializeField]
    Text mUploadVideoText;
    [SerializeField]
    Text mUploadImgText;
    [SerializeField]
    Text mUploadSecretText;
    [SerializeField]
    Text mUploadPCSecretText;
    [SerializeField]
    Dropdown mVType;
    [SerializeField]
    InputField mLink;
    [SerializeField]
    GameObject mSecretUpload;
    [SerializeField]
    GameObject mPCSecretUpload;
    //[SerializeField]
    //GameObject mEncryptToggle;
    //GameObject mEncryptSelect;
    //[SerializeField]

    private VideoData Model;
    private string mVideoPath, mImgPath,mSecretPath,mPCSecretPath;
    private CopyView mCopyView;
    private bool mLocal = true;
    private string secret = "";
    public bool Add
    {
        get; set;
    }
    public void SetData(VideoData model)
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.Video));
        mType.ClearOptions();
        mType.AddOptions(DataManager.Instance.VideoTypes);
        mVType.ClearOptions();
        mVType.AddOptions(new List<string> { "local", "Online"/*, "Live"*/ });
        if (model != null)
        {
            Model = model;
            mName.text = model.name;
            //mCategory.captionText.text = model.category;
            //mType.captionText.text = model.type;
            mLocal = model.mode.Equals("Local");
            SetVideoPath(model.video, mLocal);
            SetImgPath(model.picture);
            int index = DataManager.Instance.GetCategories(DataType.Video).IndexOf(DataManager.Instance.GetCategory(DataType.Video, model.category));
            mCategory.value = index;
            index = DataManager.Instance.VideoTypes.IndexOf(DataManager.Instance.GetVideoType(model.type));
            mType.value = index;
            mUploadVideoText.transform.parent.gameObject.SetActive(mLocal);
            mLink.transform.parent.gameObject.SetActive(!mLocal);
            mSecretUpload.SetActive(mLocal && secret == "Secret1");
            mPCSecretUpload.SetActive(mLocal && secret == "Secret1");
            if (mLocal)
            {
                mLink.text = "";
                mVType.value = 0;
                if (string.IsNullOrEmpty(model.secret))
                {
                    SetSecretPath("Unselected encrypt file");
                }
                else {
                    SetSecretPath(model.secret);
                }
                if (string.IsNullOrEmpty(model.pcSecret))
                {
                    SetPCSecretPath("Unselected encrypt file");
                }
                else
                {
                    SetPCSecretPath(model.pcSecret);
                }
            }
            else {

                mUploadVideoText.text = "Unselected video file";
                mVideoPath = "Unselected video file";
                if (model.mode.Equals("Online"))
                {
                    mVType.value = 1;
                }
                //else if (model.mode.Equals("Live"))
                //{
                //    mVType.value = 2;
                //}
            }
        }
        else
        {
            Model = null;
            mLocal = true;
            mVType.value = 0;
            mName.text = "";
            mLink.text = "";
            mUploadVideoText.transform.parent.gameObject.SetActive(true);
            mLink.transform.parent.gameObject.SetActive(false);
            mSecretUpload.SetActive(false);
            mPCSecretUpload.SetActive(false);
            SetVideoPath("Unselected video file");
            SetImgPath("Unselected thumbnail file");
            SetSecretPath("Unselected encrypt file");
            SetPCSecretPath("Unselected encrypt file");
        }
    }

    public void OnUploadVideoClick()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "All Files\0*.*\0\0";
        ofn.filter = "(*.MP4*.3GP*.3GP2*.MPV*.AVI*.ts*.m2ts*.webm*.WMV*.DivX*.vr1)\0*.MP4;*.3GP;*.3GP2;*.MPV;*.AVI;*.ts;*.m2ts;*.webm;*.WMV;*.DivX;*.vr1";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = string.IsNullOrEmpty(DataManager.Instance.DefaultPath) ? Application.dataPath : DataManager.Instance.DefaultPath;
        //path = path.Replace('/', '\\');
        //Default path 
        ofn.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Upload video";

        ofn.defExt = "MP4";//Show the type of file  
                           //Note that the following items do not have to be all selected, but 0x00000008 items should not be missing
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            if (!File.Exists(ofn.file))
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Files incorrect");
                return;
            }
            Debug.Log("OnUploadVideoClick:" + "Selected file with full path: {0}" + ofn.file);
            SetVideoPath(ofn.file);
            DataManager.Instance.DefaultPath = Path.GetDirectoryName(ofn.file);
            mSecretUpload.SetActive(secret == "Secret1");
            mPCSecretUpload.SetActive(secret == "Secret1");
            if (secret != "Secret1")
            {
                SetSecretPath("Unselected encrypt file");
                SetPCSecretPath("Unselected encrypt file");
                if (secret.Contains("Secret"))
                {
                    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                    toast.SetText("Encryption scheme");
                }
            }
        }
    }

    public void OnUploadImgClick()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "All Files\0*.*\0\0";
        ofn.filter = "(*.jpg*.png)\0*.jpg;*.png";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = string.IsNullOrEmpty(DataManager.Instance.DefaultPath) ? Application.dataPath : DataManager.Instance.DefaultPath;
        //path = path.Replace('/', '\\');
        //Default path 
        ofn.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Upload a picture";

        ofn.defExt = "JPG";//Show the type of file  
                           //Note that the following items do not have to be all selected, but 0x00000008 items should not be missing 
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            if (!File.Exists(ofn.file))
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Files incorrect");
                return;
            }
            Debug.Log( "OnUploadImgClick:Selected file with full path: {0}" + ofn.file);
            SetImgPath(ofn.file);
            DataManager.Instance.DefaultPath = Path.GetDirectoryName(ofn.file);
        }
    }


    public void OnUploadSecretClick()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "All Files\0*.*\0\0";
        //ofn.filter = "(*.jpg*.png)\0*.jpg;*.png";

        ofn.file = new string(new char[256]);

        //ofn.maxFile = ofn.file.Length;
        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = string.IsNullOrEmpty(DataManager.Instance.DefaultPath) ? Application.dataPath : DataManager.Instance.DefaultPath;
        //path = path.Replace('/', '\\');
        //Default path
        ofn.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Upload encrypted file";

        //ofn.defExt = "JPG";//Show the type of file
        //Note that the following items do not have to be all selected, but 0x00000008 items should not be missing  
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            string ex = Path.GetExtension(ofn.file);
            if (!File.Exists(ofn.file) || ex != "")
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Files incorrect");
                return;
            }
            Debug.Log("OnUploadSecretClick:Selected file with full path: {0}" + ofn.file);
            SetSecretPath(ofn.file);
            DataManager.Instance.DefaultPath = Path.GetDirectoryName(ofn.file);
        }
    }


    public void OnUploadPCSecretClick()
    {
        OpenFileName ofn = new OpenFileName();

        ofn.structSize = Marshal.SizeOf(ofn);

        //ofn.filter = "All Files\0*.*\0\0";
        //ofn.filter = "(*.jpg*.png)\0*.jpg;*.png";

        ofn.file = new string(new char[256]);

        ofn.maxFile = ofn.file.Length;

        ofn.fileTitle = new string(new char[64]);

        ofn.maxFileTitle = ofn.fileTitle.Length;
        string path = string.IsNullOrEmpty(DataManager.Instance.DefaultPath) ? Application.dataPath : DataManager.Instance.DefaultPath;
        //path = path.Replace('/', '\\');
        //Default path  
        ofn.initialDir = path;
        //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
        ofn.title = "Upload encrypted file";

        //ofn.defExt = "JPG";//Show the type of file  
        //Note that the following items do not have to be all selected, but 0x00000008 items should not be missing 
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

        if (WindowDll.GetOpenFileName(ofn))
        {
            string ex = Path.GetExtension(ofn.file);
            if (!File.Exists(ofn.file) || ex != "")
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Files incorrect");
                return;
            }
            Debug.Log("OnUploadPCSecretClick:Selected file with full path: {0}" + ofn.file);
            SetPCSecretPath(ofn.file);
            DataManager.Instance.DefaultPath = Path.GetDirectoryName(ofn.file);
        }
    }

    public void OnConfirmClick()
    {
        if (string.IsNullOrEmpty(mName.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The name cannot be empty");
            return;
        }
        //else if (string.IsNullOrEmpty(mCategory.captionText.text))
        //{
        //    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        //    toast.SetText(I2.Loc.ScriptLocalization.Get("Classification can not be empty"));
        //    return;
        //}
        else if (string.IsNullOrEmpty(mType.captionText.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Video format cannot be empty");
            return;
        }
        if (DataManager.Instance.ExistVideo(mName.text))
        {
            if (Add)
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Video already exist");
                return;
            }
            else if (mName.text != Model.name)
            {

                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Video already exist");
                return;
            }
        }
        string picturePath = "";
        string videoPath = "";
        string secretPath = "";
        string PCSecretPath = "";
        bool copyVideo = false;
        bool copyPic = false;
        bool copySecret = false;
        bool copyPCSecret = false;
        if (Add)
        {
            Model = new VideoData();
        }
        else
        {
            picturePath = Model.picture;
            videoPath = Model.video;
            secretPath = Model.secret;
            PCSecretPath = Model.pcSecret;
        }
        if (mImgPath.Equals("Unselected thumbnail file"))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Unselected thumbnail file");
            return;
        }
        else if (File.Exists(mImgPath))
        {
            if (Path.IsPathRooted(mImgPath))
            {
                if (!Add && File.Exists(Path.Combine(DataManager.Instance.VideoPosterPath, Model.picture)) && DataManager.Instance.GetVideoPicPathCount(Model.picture) == 1) File.Delete(Path.Combine(DataManager.Instance.VideoPosterPath, Model.picture));
                string fTarPath = Path.Combine(DataManager.Instance.VideoPosterPath, Path.GetFileName(mImgPath));
                if (!File.Exists(fTarPath))
                {
                    using (FileStream image = new FileStream(mImgPath, FileMode.Open))
                    {
                        System.Drawing.Image gifImage = System.Drawing.Image.FromStream(image);
                        if (gifImage.Width > 2000 || gifImage.Height > 2000)
                        {
                            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                            toast.SetText("Over 2000");
                            return;
                        }
                    }
                    copyPic = true;
                }
                picturePath = fTarPath;
            }
        }
        //    string fTarPath = DataManager.ConverPath + "\\" + mName.text + Path.GetExtension(mImgPath);
        //    copyPic = true;
        //    if (File.Exists(Model.picture)) File.Delete(Model.picture);
        //    picturePath = fTarPath;
        //}
        //else if (!Add && mName.text != Model.name)
        //{
        //    string fTarPath = DataManager.ConverPath + "\\" + mName.text + Path.GetExtension(mImgPath);
        //    if (File.Exists(Model.picture)) File.Move(Model.picture, fTarPath);
        //    picturePath = fTarPath;
        //}
        //string fTarPath = DataManager.ConverPath + "\\" + mName.text + Path.GetExtension(mUploadImgText.text);
        //if (!File.Exists(fTarPath))
        //{
        //copyPic = true;
        //}
        //else if(Add){

        //    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        //    toast.SetText(I2.Loc.ScriptLocalization.Get("The thumbnail file already exists"));
        //    return;
        //}
        //if (File.Exists(Model.picture)) File.Delete(Model.picture);
        //picturePath = fTarPath;
        else if (string.IsNullOrEmpty(Model.picture))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The thumbnail file does not exist");
            return;
        }
        if (mLocal)
        {
            if (mVideoPath == "Unselected video file")
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Unselected video file");
                return;
            }
            else if (File.Exists(mVideoPath))
            {
                if (Path.IsPathRooted(mVideoPath))
                {
                    if (!Add && File.Exists(Path.Combine(DataManager.Instance.VideoPath, Model.video)) && DataManager.Instance.GetVideoPathCount(Model.video) == 1) File.Delete(Path.Combine(DataManager.Instance.VideoPath, Model.video));
                    string fTarPath = Path.Combine(DataManager.Instance.VideoPath, Path.GetFileName(mVideoPath));
                    if (!File.Exists(fTarPath))
                    {
                        copyVideo = true;
                    }
                    videoPath = fTarPath;
                }
                //else if (!Add && (mName.text != Model.name || mPType.value.ToString() != Model.ptype))
                //{
                //    string fTarPath = DataManager.VideoPath + "\\" + Path.GetFileName(mVideoPath);
                //    if (File.Exists(Model.video)) File.Move(Model.video, fTarPath);
                //    videoPath = fTarPath;
                //}
            }
            else if (string.IsNullOrEmpty(Model.video))
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("No video");
                return;
            }
            if (secret == "Secret1")
            {
                if (File.Exists(mSecretPath))
                {
                    if (Path.IsPathRooted(mSecretPath))
                    {
                        string fTarPath = Path.Combine(DataManager.Instance.VideoPath, Path.GetFileName(mSecretPath));
                        copySecret = true;
                        secretPath = fTarPath;
                    }
                }
                if (File.Exists(mPCSecretPath))
                {
                    if (Path.IsPathRooted(mPCSecretPath))
                    {
                        string fTarPath = Path.Combine(DataManager.Instance.PCSecretPath,Path.GetFileName(mPCSecretPath));
                        copyPCSecret = true;
                        PCSecretPath = fTarPath;
                    }
                }
            }
            else {

                secretPath = "";
                PCSecretPath = "";
            }
        }
        else
        {
            if (string.IsNullOrEmpty(mLink.text))
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("Please add an online video address to the configuration list");
                return;
            }
            if (!string.IsNullOrEmpty(Model.video) && File.Exists(Model.video)) File.Delete(Model.video);
            videoPath = mLink.text.Replace("\n", "").Replace("\r", ""); ;
            secretPath = PCSecretPath = "";
        }
        Model.name = mName.text.Replace("\n", "").Replace("\r", ""); ;
        Model.category = DataManager.Instance.GetCategoryId(DataType.Video, mCategory.captionText.text);
        Model.type = DataManager.Instance.GetVideoTypeIndex(mType.captionText.text);
        Model.picture = Path.GetFileName(picturePath);
        Model.secret = Path.GetFileName(secretPath);
        Model.pcSecret = Path.GetFileName(PCSecretPath);
        switch (mVType.value)
        {
            case 0:
                Model.mode = "Local";
                break;
            case 1:
                Model.mode = "Online";
                break;
        }
        Model.video = Model.mode == "Local" ? Path.GetFileName(videoPath) : videoPath;

        if (copyPic)
        {
            //if (File.Exists(picturePath)) File.Delete(picturePath);
            File.Copy(mImgPath, picturePath);
        }
        if (copySecret)
        {
            if (File.Exists(secretPath)) File.Delete(secretPath);
            File.Copy(mSecretPath, secretPath);
        }
        if (copyPCSecret)
        {
            if (File.Exists(PCSecretPath)) File.Delete(PCSecretPath);
            File.Copy(mPCSecretPath, PCSecretPath);
        }
        DataManager.Instance.EditVideo(Model, Add);
        UIManager.Instance.PopUI();
        if (copyVideo)
        {
            //if (File.Exists(videoPath)) File.Delete(videoPath);
            mCopyView = UIManager.Instance.PushUI(UI.Copy).GetComponent<CopyView>();
            Tools.Instance.CopyFile(mVideoPath, videoPath);
        }
    }

    public void OnCancleClick()
    {
        UIManager.Instance.PopUI();
    }

    public void OnFreshCategory()
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.Video));
    }

    void SetVideoPath(string path,bool local = true)
    {
        if (local)
        {
            mUploadVideoText.text = path;
            mVideoPath = path;
            secret = Getsecret(path);
        }
        else {
            mLink.text = path;
            secret = "";
        }
    }
    void SetImgPath(string path)
    {
        mImgPath = path;
        mUploadImgText.text = path;
    }

    void SetSecretPath(string path)
    {
        mSecretPath = path;
        mUploadSecretText.text = path;
    }
    void SetPCSecretPath(string path)
    {
        mPCSecretPath = path;
        mUploadPCSecretText.text = path;
    }
    public void OnValueChanged()
    {
        string str = Tools.SplitNameByASCII(mName.text.Trim());
        mName.text = str;
    }
    public void OnLinkTypeValueChanged()
    {
        mLocal = mVType.value == 0;
        mUploadVideoText.transform.parent.gameObject.SetActive(mLocal);
        mLink.transform.parent.gameObject.SetActive(!mLocal);
        if (!mLocal)
        {
            mSecretUpload.SetActive(false);
            mPCSecretUpload.SetActive(false);
        }
        else {
            mSecretUpload.SetActive(secret == "Secret1");
            mPCSecretUpload.SetActive(secret == "Secret1");
        }
    }

    public string Getsecret(string path)
    {
        if (File.Exists(path))
        {
            FileStream str = File.OpenRead(path);
            byte[] bytes = new byte[7];
            str.Read(bytes, 0, 7);
            str.Close();
            return System.Text.Encoding.UTF8.GetString(bytes);

        }
        return "";
    }
}