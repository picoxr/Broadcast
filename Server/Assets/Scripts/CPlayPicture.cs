using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CPlayPicture : MonoBehaviour
{
    //Video player object
    public GameObject picoPlayerObject;

    //Prompt file does not exist
    public GameObject nonExistsText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.U))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    //Start playing panorama
    public void fnPlayPicture()
    {
        CBaseData.ePlayType = PlayType.gallery;
        Debug.Log("fnPlayPicture:Select  Movie ..... " + CBaseData.strPlayPictureName);
        if (File.Exists(CBaseData.strPlayPictureName))
        {
            Debug.Log("fnPlayPicture:"+CBaseData.strPlayPictureName);
            if (nonExistsText.activeSelf)
            {
                nonExistsText.SetActive(false);
            }
            CLoadJson.Instance.Drd_IDList.enabled = !Server.Instance.IsFree;
            PicoGalleryPlayer.FileName = CBaseData.strPlayPictureName;
            PicoGalleryPlayer.PictureName = CBaseData.strPlayPictureTitle;
            Instantiate(picoPlayerObject, picoPlayerObject.transform.position, picoPlayerObject.transform.rotation);
        }
        else
        {
            Debug.Log("fnPlayPicture:"+CBaseData.strPlayVideoName + "：The panorama file does not exist！");
            if (!nonExistsText.activeSelf)
            {
                nonExistsText.GetComponentInChildren<Text>().text = "The panorama file does not exist";
                nonExistsText.SetActive(true);
            }
        }

    }

    //Cancel playing panorama
    public void fnCancelPlay()
    {
        if (nonExistsText.activeSelf)
            nonExistsText.SetActive(false);
    }
}
