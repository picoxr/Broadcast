using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetSet : MonoBehaviour {

    [SerializeField]
    List<Text> mTexts;
    //private string[] keys = new string[] { "ssid", "pswd", "serverip", "port", "id" };
    private List<string> mValues = new List<string>();
    // Use this for initialization
    void Start () {

        ParseConfig();
        DataManager.OnEditNet = ParseConfig;
    }

    void ParseConfig()
    {
        string text = DataManager.Instance.ConfigText;
        mValues.Clear();
        if (!string.IsNullOrEmpty(text))
        {
            text = text.Replace("\r", "");
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {
                    mValues.Add(lines[i].Split(':')[1]);
                }
            }
            for (int i = 0; i < mValues.Count; i++)
            {
                mTexts[i].text = mValues[i];
            }
        }
    }

    public void OnEditClick()
    {
        NetSetEdit edit = UIManager.Instance.ReplaceUI(UI.NetEdit).GetComponent<NetSetEdit>();
        edit.SetData(mValues);
    }
}
