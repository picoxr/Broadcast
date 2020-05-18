using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum euStatus
//{
//    Standby,
//    Playing,
//    Pause,
//    Free mode
//}
public enum PlayType
{
    video,
    gallery,
    app
}

public class CBaseData {

    //public static string[] strStatus = new string[4] { "Free mode", "Standby", "Playing", "Pause"};

    //Name of video file to be played (including suffix)
    public static string strPlayVideoName = "";

    //Video title to play
    public static string strPlayVideoTitle = "";

    //Types of movies
    public static string strVideoType = "";

    //Movie local / Online
    public static string strLinkModel = "";

    //Is it 8K video
    public static bool is_8K = false;

    //Total number of client lists
    public static int nMaxClient;

    //Progress of server seek
    public static string strSeekTime = "";

    //Synchronized client Sn number
    public static string strSynClientSN = "";


    //App package name to start
    public static string strPlayAppName = "";

    //App title to launch
    public static string strPlayAppTitle = "";

    //Panorama file name (including suffix) to be played
    public static string strPlayPictureName = "";

    //Panorama title to play
    public static string strPlayPictureTitle = "";

    //Panorama title to play
    public static string strPlayPictureCategory = "";

    //public static enum ClientStatus
    //{
    //    Moning = 0,
    //    Afternoon = 1,
    //    Evening = 2,
    //};
    //Playback type
    public static PlayType ePlayType = PlayType.video;
}
