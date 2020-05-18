using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CButtonManager : MonoBehaviour
{

    public GameObject[] gaUI;

    public Scrollbar scroVideo, scroClient;
    [SerializeField]
    public Button model0Btn, model1Btn,settingBtn;
    [SerializeField]
    GameObject model;
    private Color SelectColor = new Color(44 / 255f, 216 / 255f, 109 / 255f);
    private Color PointColor = new Color(0, 1, 240 / 255f);
    //Server on
    private bool IsServerStart = false;

    List<string> btnsName;
    Button currBtn;
    // Use this for initialization
    void Start()
    {
        btnsName = new List<string>();
        btnsName.Add("Videos");
        btnsName.Add("Device");
        // btnsName.Add("Model");
        //btnsName.Add("StartServer");
        btnsName.Add("Apps");
        btnsName.Add("Images");

        foreach (string btnName in btnsName)
        {
            GameObject btnObj = GameObject.Find(btnName);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(delegate ()
            {
                this.OnClick(btnObj);
            });
            if (btnObj.name == "Videos")
            {
                btn.image.sprite = btn.spriteState.highlightedSprite;
                btn.GetComponentInChildren<Text>().color = SelectColor;
                currBtn = btn;
            }
        }
        settingBtn.onClick.AddListener(delegate ()
        {
            this.OnSettingClick(settingBtn.gameObject);
        });

        model0Btn.onClick.AddListener(delegate ()
        {
            this.OnModelClick(model0Btn.gameObject);
        });

        model1Btn.onClick.AddListener(delegate ()
        {
            this.OnModelClick(model1Btn.gameObject);
        });

    }

    void OnEnable()
    {
        if(currBtn != null) currBtn.image.sprite = currBtn.spriteState.highlightedSprite;
    }

    public void OnClick(GameObject sender)
    {
        //foreach (string btnName in btnsName)
        //{
        //    GameObject btnObj = GameObject.Find(btnName);
        //    Button btn = btnObj.GetComponent<Button>();
        //    btn.image.sprite = btnObj == sender ? btn.spriteState.highlightedSprite : btn.spriteState.disabledSprite;
        //    if(btnObj == sender) currBtn = btn;
        //}
        model.SetActive(sender.name.Equals("Videos"));
        switch (sender.name)
        {
            case "Videos":
                //Debug.Log("OnClick:Videos");
                fnShowUI(0);

                break;

            case "Device":
                //Debug.Log("OnClick:Device");
                fnShowUI(1);

                break;

            case "Model":
                //Debug.Log("OnClick:Model");
                fnShowUI(2);
                break;

            case "Apps":
                //Debug.Log("OnClick:Apps");
                fnShowUI(3);
                break;
            case "Images":
                //Debug.Log("OnClick:Images");
                fnShowUI(4);
                break;

            case "StartServer":
                IsServerStart = !(IsServerStart);
                Text tex = sender.transform.GetChild(0).GetComponent<Text>();
                if (!IsServerStart)
                {
                    tex.text = "Start server";
                    tex.color = Color.green;
                }
                else
                {
                    tex.text = "Shut down the server";
                    tex.color = Color.red;
                }
                break;
            default:
                //Debug.Log("none");
                break;
        }
    }

    public void OnSelect(Text sender)
    {
        sender.color = SelectColor;
        Button btn = sender.transform.parent.GetComponent<Button>();
        btn.image.sprite = btn.spriteState.highlightedSprite;
        currBtn.GetComponentInChildren<Text>().color = Color.white;
        currBtn.image.sprite = btn.spriteState.disabledSprite;
        currBtn = btn;
    }

    public void OnPointEnter(Text sender)
    {
        if(currBtn.transform != sender.transform.parent) sender.color = PointColor;
    }
    public void OnPointExit(Text sender)
    {
        if(currBtn.transform != sender.transform.parent) sender.color = Color.white;
    }

    void fnShowUI(int ndex)
    {
        for (int i = 0; i < gaUI.Length; i++)
        {
            if (i == ndex)
            {
                gaUI[i].SetActive(true);
                gaUI[i].transform.Find("Viewport").Find("Content").localPosition = new Vector3(gaUI[i].transform.Find("Viewport").Find("Content").localPosition.x, 0, 0);
                continue;
            }
            gaUI[i].SetActive(false);
        }

        if (scroVideo)
            scroVideo.value = 1;
        if (scroClient)
        {
            scroClient.value = 1;
        }
    }

    void OnModelClick(GameObject obj)
    {
        model0Btn.gameObject.SetActive(model1Btn.gameObject == obj);
        model1Btn.gameObject.SetActive(model0Btn.gameObject == obj);
       // PicoMediaPlayer.CompletePlay = model0Btn.gameObject.activeSelf;
    }

    void OnSettingClick(GameObject obj)
    {
        UIManager.Instance.RunUI(UI.VideoSet);
    }

    public void OnLaunguePointEnter(Image sender)
    {
        sender.color = new Color(1, 1, 1, 1);
        //sender.rectTransform.sizeDelta = new Vector2(0, 0);
        //Texture2D tex = Resources.Load("dropdown_rollOver") as Texture2D;
        //Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        //sender.sprite = temp;
    }
    public void OnLaunguePointExit(Image sender)
    {
        sender.color = new Color(1,1,1,0);
        //Texture2D tex = Resources.Load("dropdown_normal") as Texture2D;
        //Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        //sender.sprite = temp;
    }
}
