using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;
using LitJson;

public class PictureSetEdit : MonoBehaviour {

    [SerializeField]
    InputField mName;
    [SerializeField]
    Dropdown mCategory;
    [SerializeField]
    Text mUploadPanoramaText;
    [SerializeField]
    Text mUploadImgText;

    private PictureData Model;
    public bool Add
    {
        get; set;
    }

    public void SetData(PictureData model)
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.picture));
        if (model != null)
        {
            Model = model;
            mName.text = model.name;
            //mCategory.captionText.text = model.category;
            mUploadPanoramaText.text = model.pictureURL;
            mUploadImgText.text = model.posterURL;
            int index = DataManager.Instance.GetCategories(DataType.picture).IndexOf(DataManager.Instance.GetCategory(DataType.picture, model.category));
            mCategory.value = index;
        }
        else {

            Model = null;
            mName.text = "";
            mUploadPanoramaText.text = "Unselected panorama file";
            mUploadImgText.text = "Unselected thumbnail file";
        }
    }

    public void OnUploadPanoramaClick()
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
        ofn.title ="Upload panorama";

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
            Debug.Log("OnUploadPanoramaClick:" + "Selected file with full path: {0}" + ofn.file);
            mUploadPanoramaText.text = ofn.file;
            DataManager.Instance.DefaultPath = Path.GetDirectoryName(ofn.file);
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
            Debug.Log("OnUploadImgClick:" + "Selected file with full path: {0}" + ofn.file);
            mUploadImgText.text = ofn.file;
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
        if (DataManager.Instance.ExistPicture(mName.text))
        {
            if (Add)
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("The panorama file has already existed");
                return;
            }
            else if (mName.text != Model.name)
            {

                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("The panorama file has already existed");
                return;
            }
        }
        string picturePath = "";
        string panoramaPath = "";
        bool copyPic = false;
        bool copyPanorama = false;
        if (Add)
        {
            Model = new PictureData();
        }
        else
        {
            picturePath = Model.posterURL;
            panoramaPath = Model.pictureURL;
        }
        if (mUploadImgText.text.Equals("Unselected thumbnail file"))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Unselected thumbnail file");
            return;
        }
        else if (File.Exists(mUploadImgText.text))
        {
            if (Path.IsPathRooted(mUploadImgText.text))
            {
                if (Path.IsPathRooted(mUploadImgText.text))
                {
                    if (!Add && File.Exists(Path.Combine(DataManager.Instance.PicturePosterPath, Model.posterURL)) && DataManager.Instance.GetPictureCoverPathCount(Model.posterURL) == 1) File.Delete(Path.Combine(DataManager.Instance.PicturePosterPath, Model.posterURL));
                    string fTarPath = Path.Combine(DataManager.Instance.PicturePosterPath,Path.GetFileName(mUploadImgText.text));
                    if (!File.Exists(fTarPath))
                    {
                        using (FileStream image = new FileStream(mUploadImgText.text, FileMode.Open))
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
        }
        else if (string.IsNullOrEmpty(Model.posterURL))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The thumbnail file does not exist");
            return;
        }
        if (mUploadPanoramaText.text.Equals("Unselected panorama file"))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Unselected panorama file");
            return;
        }
        else if (File.Exists(mUploadPanoramaText.text))
        {
            if (Path.IsPathRooted(mUploadPanoramaText.text))
            {
                if (!Add && File.Exists(Path.Combine(DataManager.Instance.PicturePath, Model.pictureURL)) && DataManager.Instance.GetPicturePhotoPathCount(Model.pictureURL) == 1) File.Delete(Path.Combine(DataManager.Instance.PicturePath, Model.pictureURL));
                string fTarPath = Path.Combine(DataManager.Instance.PicturePath, Path.GetFileName(mUploadPanoramaText.text));
                if (!File.Exists(fTarPath))
                {
                    copyPanorama = true;
                }
                panoramaPath = fTarPath;
            }
        }
        else if (string.IsNullOrEmpty(Model.pictureURL))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("Panorama files do not exist");
            return;
        }
        Model.name = mName.text;
        Model.category = DataManager.Instance.GetCategoryId(DataType.picture, mCategory.captionText.text);
        Model.posterURL = Path.GetFileName(picturePath);
        Model.pictureURL = Path.GetFileName(panoramaPath);
        if (copyPic)
        {
            //if (File.Exists(picturePath)) File.Delete(picturePath);
            File.Copy(mUploadImgText.text, picturePath);
        }
        if (copyPanorama)
        {
            //if (File.Exists(panoramaPath)) File.Delete(panoramaPath);
            File.Copy(mUploadPanoramaText.text, panoramaPath);
        }
        DataManager.Instance.EditPicture(Model, Add);
        UIManager.Instance.PopUI();
    }

    public void OnCancleClick()
    {
        UIManager.Instance.PopUI();
    }

    public void OnFreshCategory()
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.picture));
    }
    public void OnValueChanged()
    {
        string str = Tools.SplitNameByASCII(mName.text.Trim());
        mName.text = str;
    }
}
