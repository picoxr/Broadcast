using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class LaunchPic : MonoBehaviour {

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// Recive Android Texture
    /// </summary>
    /// <param name="path"></param>
    private void TexturePtrCreatedOK(string path)
    {
    
        int texPtr = 0;
        PicoUnityActivity.CallObjectMethod<int>(ref texPtr, "CreateTexturePtr", path);
        Texture2D nativeTexture = Texture2D.CreateExternalTexture(4096, 2048, TextureFormat.RGBA32, false, false, (IntPtr)texPtr);
        transform.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        transform.GetComponent<Renderer>().material.mainTexture = nativeTexture;
        GC.Collect();
    }   
}
