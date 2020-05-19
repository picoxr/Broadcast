using System;
using UnityEngine;
using System.IO;
using UnityEngine.Video;


public class NetCommandHandler
{
    public ServerPlayer serverplay;
    private string mCommnad;
    private string mType;
    private int mSeekpos;
    private string mFullpath;
    public NetCommandHandler()
    {
        serverplay = GameObject.Find("clip").GetComponent<ServerPlayer>();
    }
    public void OnCommand(string command, string parameter)
    {
        switch (command)
        {
            case "ready": //Kiosk Mode
                Main.isfreeMode = false;
                AndroidTools.EnableKeyAndroid(Main.isfreeMode);
                Main.statusBar.GetComponent<Canvas>().enabled = Main.isfreeMode;
                Main.clinetControl.HidePlayer();
                Main.statusBar.showMessage.text = "Under Kiosk Mode";
                break;

            case "free": // Free Mode
                Main.isfreeMode = true;
                Main.freeMode = true;
                if (!AndroidMsgReciver.allowChangeVolume)
                {
                    AndroidMsgReciver.allowChangeVolume = true;
                    PicoUnityActivity.CallObjectMethod("setVolumeKeyEnabled", true);
                }
                AndroidTools.EnableKeyAndroid(Main.isfreeMode);
                Main.statusBar.GetComponent<Canvas>().enabled = Main.isfreeMode;
                Main.statusBar.showMessage.text = "Under Free Mode";
                break;


            case "volume": //Control Volume
                {
                    int volume = int.Parse(parameter);
                    AndroidMsgReciver.allowChangeVolume = volume != -1;
                    PicoUnityActivity.CallObjectMethod("setVolumeKeyEnabled", AndroidMsgReciver.allowChangeVolume);
                    if (volume == -1) volume = 0;
                    AndroidVolume.SetVolume(volume);

                }
                break;
            case "1"://Play 1803D_LR Video

                serverplay.ServerPlayerVideo(command, parameter);

                break;
            case "2"://Play 1803D_TB Video

                serverplay.ServerPlayerVideo(command, parameter);
                break;
            case "7"://Play 360 Video

                serverplay.ServerPlayerVideo(command, parameter);
                break;

            case "playGallery": //Play Picture
                serverplay.ServerPlayPicture(parameter);
                break;
            case "synchro":
                Main.synchro = bool.Parse(parameter);
                break;

            case "config":
                string ConfigPath = "/sdcard/pre_resource/broadcast/config";
                string[] str = parameter.Split('|');
                string tarPath = Path.Combine(ConfigPath, str[0]);
                FileStream file = new FileStream(tarPath, FileMode.Create);
                byte[] bts = System.Text.Encoding.UTF8.GetBytes(str[1]);
                file.Write(bts, 0, bts.Length);
                if (file != null)
                {
                    file.Close();
                }
                string error = null;
                AndroidTools.Initialize(ref error);

                break;
            //case "Pause"://Video Pause
            //    serverplay.ServerPlayerControl(command);
            //    break;
            //case "Continue": //Video Continue
            //    serverplay.ServerPlayerControl(command);
            //    break;
            //case "Stop": //Video Stop
            //    serverplay.ServerPlayerControl(command);
            //    break;
            default://Control PlayerOrPictrue
                serverplay.ServerPlayerControl(command);
                break;
        }
    }

}