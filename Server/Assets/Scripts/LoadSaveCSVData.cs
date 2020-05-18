using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

/// <summary>  
/// Read and save CSV utility class
/// （Requirement: UTF-8 format）  
/// </summary>  
public class LoadSaveCSVData : MonoBehaviour
{
    public delegate void toDirectoryCallBack(Dictionary<int, List<string>> csvdata);

    /// <summary>  
    /// Start reading CSV document asynchronously  
    /// </summary>  
    /// <param name="Filename">StreamingAssetsFile name in directory</param>  
    /// <param name="LoadCBK">Callback event after reading</param>  
    /// <returns></returns>  
    public IEnumerator LoadCSV_toDirectory(string Filename, toDirectoryCallBack LoadCBK)
    {

        string CSVText = null;
        yield return StartCoroutine(LoadWWW("file://" + GetFilepath(Filename), (www) => { CSVText = www; }));
        Dictionary<int, List<string>> CSVData = getDirectory(readcsv(CSVText.ToString()));
        LoadCBK(CSVData);
    }

    /// <summary>  
    /// Get full file path 
    /// </summary>  
    /// <param name="Filename"></param>  
    /// <returns></returns>  
    public static string GetFilepath(string Filename)
    {
        string filePath = Path.Combine("pre_resource/broadcast", Filename);
        if (!Application.isEditor && !Path.IsPathRooted(filePath))
        {
            string rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            filePath = Path.Combine(rootPath, filePath);
        }
        return filePath.Replace("\\","/");
    }

    /// <summary>  
    ///read file
    /// </summary>  
    /// <param name="filepath">File path</param>  
    /// <param name="loadCBK">Read completion callback</param>  
    /// <returns></returns>  
    IEnumerator LoadWWW(string filepath, Action<string> loadCBK)
    {
        WWW www = new WWW(filepath);

        yield return www;

        string CSVText;
        if (!string.IsNullOrEmpty(www.error))
        {
            CSVText = www.error;
            CSVText = "SN,Video name, playback times\n";
        }
        else
        {
            CSVText = www.text;
        }
        loadCBK(CSVText);
    }

    public static List<List<string>> readcsv(string strin)
    {
        return readcsv(strin, Encoding.UTF8);
    }

    public static List<List<string>> readcsv(string strin, Encoding encoding)
    {
        List<List<string>> ret = new List<List<string>>();

        strin = strin.Replace("\r", "");
        string[] lines = strin.Split('\n');

        if (lines.Length > 0)
        {
            byte[] byt = encoding.GetBytes(lines[0]);
            if (byt.Length >= 3 &&
                byt[0] == 0xEF &&
                byt[1] == 0xBB &&
                byt[2] == 0xBF)
            {
                lines[0] = encoding.GetString(byt, 3, byt.Length - 3);
            }
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i]) ||
                    lines[i].StartsWith("#"))
                continue;
            List<string> s = split(lines[i], encoding);
            ret.Add(s);
        }
        return ret;
    }

    static List<string> split(string line, Encoding encoding)
    {
        byte[] b = encoding.GetBytes(line);
        List<string> bls = new List<string>();
        int end = b.Length - 1;

        List<byte> bl = new List<byte>();
        bool inQuote = false;
        for (int i = 0; i < b.Length; i++)
        {
            switch ((char)b[i])
            {
                case ',':
                    if (inQuote)
                        bl.Add(b[i]);
                    else
                    {
                        bls.Add(makefield(ref bl, encoding));
                        bl.Clear();
                    }
                    break;
                case '"':
                    inQuote = !inQuote;
                    bl.Add((byte)'"');
                    break;
                case '\\':
                    if (i < end)
                    {
                        switch ((char)b[i + 1])
                        {
                            case 'n':
                                bl.Add((byte)'\n');
                                i++;
                                break;
                            case 't':
                                bl.Add((byte)'\t');
                                i++;
                                break;
                            case 'r':
                                i++;
                                break;
                            default:
                                bl.Add((byte)'\\');
                                break;
                        }
                    }
                    else
                        bl.Add((byte)'\\');
                    break;
                default:
                    bl.Add(b[i]);
                    break;
            }
        }
        bls.Add(makefield(ref bl, encoding));
        bl.Clear();

        return bls;
    }

    static string makefield(ref List<byte> bl, Encoding encoding)
    {
        if (bl.Count > 1 && bl[0] == '"' && bl[bl.Count - 1] == '"')
        {
            bl.RemoveAt(0);
            bl.RemoveAt(bl.Count - 1);
        }
        int n = 0;
        while (true)
        {
            if (n >= bl.Count)
                break;
            if (bl[n] == '"')
            {
                if (n < bl.Count - 1 && bl[n + 1] == '"')
                {
                    bl.RemoveAt(n + 1);
                    n++;
                }
                else
                    bl.RemoveAt(n);
            }
            else
                n++;
        }

        return encoding.GetString(bl.ToArray());
    }

    /// <summary>  
    /// Convert to dictionary type
    /// </summary>  
    /// <param name="listin"></param>  
    /// <returns></returns>  
    public static Dictionary<int, List<string>> getDirectory(List<List<string>> listin)
    {
        Dictionary<int, List<string>> dir = new Dictionary<int, List<string>>();
        for (int i = 0; i < listin.Count; i++)
        {
            if (string.IsNullOrEmpty(listin[i][0]))
                continue;
            dir[i] = listin[i];
            //dir[listin[i][0]] = listin[i];
        }
        return dir;
    }

    /// <summary>  
    /// Print the two-dimensional array converted by CSV, process the received string, judge by SN number and video file name, add or watch 1 times. 
    /// </summary>  
    /// <param name="grid">Data dictionary loaded by source file</param>  
    public static void SaveCSVData(string recvString, string fullPath, Dictionary<int, List<string>> grid)
    {
        //Process the received string in the format："SN"+"|"+"xxx.mp4"
        string[] recvStr = recvString.Split('|');
        if (recvStr.Length == 2)
        {
            int tempState = 0;
            int tempLine = 0;
            int tempCount = 0;

            List<string> tempList = null;
            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < grid[i].Count; j++)
                {
                    if (grid[i][j] == recvStr[0])
                    {
                        if (grid[i][j + 1] == recvStr[1])
                        {
                            tempState = 1;
                            tempLine = i;
                            tempCount = j;
                        }
                        else if (grid[i][j + 1] != recvStr[1] && tempState == 0)
                        {
                            tempState = 2;
                            tempLine = grid.Count;
                        }
                    }
                    else if (tempState == 0)
                    {
                        tempState = 2;
                        tempLine = grid.Count;
                    }
                }
            }
            //Increase the playing times of a movie in a Sn
            if (tempState == 1)
            {
                grid[tempLine][tempCount + 2] = (int.Parse(grid[tempLine][tempCount + 2]) + 1).ToString();
            }
            //Add a new play record of a Sn movie
            if (tempState == 2)
            {
                tempList = new List<string> { recvStr[0], recvStr[1], "1" };
                grid[tempLine] = tempList;
            }

            //Arrange strings in CSV format
            StringBuilder textOutput = new StringBuilder();
            foreach (var line in grid)
            {
                for (int i = 0; i < line.Value.Count; i++)
                {
                    string row = line.Value[i];
                    textOutput.Append(row);
                    if (i < line.Value.Count - 1)
                        textOutput.Append(",");
                }
                textOutput.Append("\n");
            }
            //Debug.Log("SaveCSVData:"+textOutput);

            //Save to target CSV file
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.Write(textOutput);//Write string in behavior units
            Debug.Log("SaveCSVData:String written:  " + textOutput);
            sw.Close();
            sw.Dispose();//FileStream release
        }
        else
        {
            Debug.Log("SaveCSVData:Received string:" + recvString + "Wrong rules");
        }
    }
}