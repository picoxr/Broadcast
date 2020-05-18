using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerUtils
{
    /// <summary>
    /// Applicable to loading external pictures with texture.loadimage method
    /// </summary>
    /// <param name="path">Absolute path of picture file</param>
    /// <returns></returns>
    public static byte[] LoadByIO(string path)
    {
        byte[] bytes = null;
        //Create file read stream
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //Create file length buffer
        bytes = new byte[fileStream.Length];
        //Read file
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //Release file read stream
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        return bytes;
    }

    /// <summary>
    /// Convert milliseconds to HH: mm: SS format and place 0
    /// </summary>
    /// <param name="l">millisecond</param>
    /// <returns></returns>
    public static string FormatLongToTimeStr(long l)
    {
        int hour = 0;
        int minute = 0;
        int second = 0;
        string hours = string.Empty;
        string mins = string.Empty;
        string seconds = string.Empty;
        second = (int)(l / 10000000);
        if (second > 60)
        {
            minute = second / 60;
            second = second % 60;
        }
        if (minute > 60)
        {
            hour = minute / 60;
            minute = minute % 60;
        }
        if (hour < 10)
        {
            hours = "0" + hour.ToString();
        }
        else
        {
            hours = hour.ToString();
        }
        if (minute < 10)
        {
            mins = "0" + minute.ToString();
        }
        else
        {
            mins = minute.ToString();
        }
        if (second < 10)
        {
            seconds = "0" + second.ToString();
        }
        else
        {
            seconds = second.ToString();
        }
        return (hours + ":" + mins + ":" + seconds);
    }

    /// <summary>
    /// Recursively find hidden nodes
    /// </summary>
    /// <param name="target">Parent directory</param>
    /// <param name="name">Node name</param>
    /// <returns></returns>
    public static Transform Find(Transform target, string name)
    {
        if (target.name == name)
        {
            return target;
        }
        for (int i = 0; i < target.childCount; ++i)
        {
            Transform result = Find(target.GetChild(i), name);
            if (result != null) return result;
        }
        return null;
    }

    /// <summary>
    /// Get MD5 value of video file (first 400 bytes)
    /// </summary>
    /// <param name="filePath">Video file path</param>
    /// <returns></returns>
    public static string GetFileHash(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            long len = fs.Length;
            if (len > 400)
                len = 400;
            byte[] data = new byte[len];
            fs.Read(data, 0, (int)len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach (byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            return fileMD5;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning("GetFileHash"+e.Message);
            return string.Empty;
        }
    }
}