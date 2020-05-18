using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.IO;
using LitJson;

public class AppSetEdit : MonoBehaviour {

    [SerializeField]
    InputField mName;
    [SerializeField]
    InputField mPackage;
    [SerializeField]
    Dropdown mCategory;
    [SerializeField]
    Text mUploadImgText;

    private AppData Model;
    public bool Add
    {
        get; set;
    }

    public void SetData(AppData model)
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.app));
        if (model != null)
        {
            Model = model;
            mName.text = model.name;
            //mCategory.captionText.text = model.category;
            mPackage.text = model.pkgname;
            mUploadImgText.text = model.picture;
            int index = DataManager.Instance.GetCategories(DataType.app).IndexOf(DataManager.Instance.GetCategory(DataType.app,model.category));
            mCategory.value = index;
        }
        else {

            Model = null;
            mName.text = mPackage.text =  "";
            mUploadImgText.text = "Unselected thumbnail file";
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
                           //Note that items do not have to be all selected, but 0x00000008 items are not missing 
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
        else if (string.IsNullOrEmpty(mPackage.text))
        {
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("App package name can not be empty");
            return;
        }
        //else if (string.IsNullOrEmpty(mCategory.captionText.text))
        //{
        //    ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
        //    toast.SetText(I2.Loc.ScriptLocalization.Get("Classification can not be empty"));
        //    return;
        //}
        string picturePath = "";
        if (Add)
        {
            Model = new AppData();
        }
        else
        {
            picturePath = Model.picture;
        }
        if (DataManager.Instance.ExistApp(mName.text))
        {
            if (Add)
            {
                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("The application has already existed");
                return;
            }
            else if (mName.text != Model.name)
            {

                ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
                toast.SetText("The application has already existed");
                return;
            }
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
                if (!Add && File.Exists(Path.Combine(DataManager.Instance.AppPosterPath, Model.picture)) && DataManager.Instance.GetAppPicPathCount(Model.picture) == 1) File.Delete(Path.Combine(DataManager.Instance.AppPosterPath, Model.picture));
                string fTarPath =  Path.Combine(DataManager.Instance.AppPosterPath,Path.GetFileName(mUploadImgText.text));
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
                    File.Copy(mUploadImgText.text, fTarPath);
                }
                picturePath = fTarPath;
            }
        }
        else if (string.IsNullOrEmpty(Model.picture))
        { 
            ToastView toast = UIManager.Instance.PushUI(UI.Toast).GetComponent<ToastView>();
            toast.SetText("The thumbnail file does not exist");
            return;
        }
        Model.name = mName.text.Replace("\n", "").Replace("\r", ""); ;
        Model.category = DataManager.Instance.GetCategoryId(DataType.app,mCategory.captionText.text);
        Model.pkgname = mPackage.text.Replace("\n", "").Replace("\r", ""); ;
        Model.picture = Path.GetFileName(picturePath);
        DataManager.Instance.EditApp(Model, Add);
        UIManager.Instance.PopUI();
    }

    public void OnCancleClick()
    {
        UIManager.Instance.PopUI();
    }

    public void OnFreshCategory()
    {
        mCategory.ClearOptions();
        mCategory.AddOptions(DataManager.Instance.GetCategories(DataType.app));
    }
    public void OnValueChanged(InputField input)
    {
        if (input != mPackage)
        {
            string str = Tools.SplitNameByASCII(input.text.Trim());
            input.text = str;
        }
    }
}
