using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerSet : MonoBehaviour {

    [SerializeField]
    Button VideoBtn;
    [SerializeField]
    Button AppBtn;
    [SerializeField]
    Button PictureBtn;
    [SerializeField]
    Button ClientBtn;
    [SerializeField]
    Button NetBtn;
    Button CurrentBtn;
    private Color SelectColor = new Color(44 / 255f, 216 / 255f, 109 / 255f);
    private Color PointColor = new Color(0, 1, 240 / 255f);
    // Use this for initialization
    void OnEnable () {
        if (CurrentBtn != null)
        {
            CurrentBtn.image.sprite = CurrentBtn.spriteState.disabledSprite;
            CurrentBtn.GetComponentInChildren<Text>().color = Color.white;
        }
        CurrentBtn = VideoBtn;
        CurrentBtn.image.sprite = CurrentBtn.spriteState.highlightedSprite;
        CurrentBtn.GetComponentInChildren<Text>().color = SelectColor;
    }

    public void OnVideoSet()
    {
        UIManager.Instance.ReplaceUI(UI.VideoSet);
    }

    public void OnAppSet()
    {
        UIManager.Instance.ReplaceUI(UI.AppSet);
    }

    public void OnPictureSet()
    {
        UIManager.Instance.ReplaceUI(UI.PictureSet);
    }

    public void OnClientSet()
    {
        UIManager.Instance.ReplaceUI(UI.ClientSet);
    }
    public void OnNetSet()
    {
        UIManager.Instance.ReplaceUI(UI.NetSet);
    }

    public void OnBack()
    {
        UIManager.Instance.PopAllUI();
    }

    public void OnSelect(Text sender)
    {
        sender.color = SelectColor;
        Button btn = sender.transform.parent.GetComponent<Button>();
        btn.image.sprite = btn.spriteState.highlightedSprite;
        CurrentBtn.GetComponentInChildren<Text>().color = Color.white;
        CurrentBtn.image.sprite = btn.spriteState.disabledSprite;
        CurrentBtn = btn;
    }

    public void OnPointEnter(Text sender)
    {
        if (CurrentBtn.transform != sender.transform.parent) sender.color = PointColor;
    }
    public void OnPointExit(Text sender)
    {
       if (CurrentBtn.transform != sender.transform.parent) sender.color = Color.white;
    }
    public void OnPointUp(Image sender)
    {
        Texture2D tex = Resources.Load("button") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        sender.sprite = temp;
    }
}
