using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Play load graph GIF animation
/// </summary>
public class LoadingGif : MonoBehaviour
{
    private float speed = 0.3f;//Playback speed  
    private bool isPlayGifs = false;//Play or not  
    private int gifIndex = 0;//Control pictures to play  

    void Start()
    {
        isPlayGifs = true;
    }

    private void Update()
    {
        PlayGifs(CLoadJson.gifFrames);
        //Play gif
    }
    public void PlayGifs(List<Texture2D> frames)
    {
        //Play gif
        if (isPlayGifs == true)
        {
            gifIndex++;
            transform.GetComponent<RawImage>().texture = frames[(int)(gifIndex * speed) % frames.Count];
        }
    }
}
