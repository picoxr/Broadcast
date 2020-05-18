using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerEvent : MonoBehaviour {

    public void OnPointEnter(Image image)
    {
        //image.color = new Color(241 / 255.0f, 241 / 255.0f, 241 / 255.0f);
        Texture2D tex = Resources.Load("dropdown_roll") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnPointExit(Image image)
    {
        //image.color = Color.white;
        Texture2D tex = Resources.Load("dropdown_sprite") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnSelect(Image image)
    {
        image.color = Color.white;
        Texture2D tex = Resources.Load("dropdown_active") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnSetPointEnter(Image image)
    {
        Texture2D tex = Resources.Load("dropdown_roll") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnSetPointExit(Image image)
    {
        Texture2D tex = Resources.Load("dropdown_active") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnButtonPointEnter(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("button_rollOver") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnButtonPointExit(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("button") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnUpButtonPointEnter(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("up_rollOver") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnUpButtonPointExit(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("up") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }

    public void OnUpSButtonPointEnter(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("up_rollOver_s") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }
    public void OnUpSButtonPointExit(Image image)
    {
        if (!image.GetComponent<Button>().interactable) return;
        Texture2D tex = Resources.Load("up_s") as Texture2D;
        Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        image.sprite = temp;
    }

    public void OnButtonPointUp(Image image)
    {
        OnButtonPointExit(image);
    }
}
