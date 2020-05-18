using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Threading;
using System.Runtime.InteropServices;

public class Tools {

    private static Tools tools;
    public static Tools Instance
    {
        get
        {
            if (tools == null) tools = new Tools();
            return tools;
        }
    }
    private const int CHARACTER_LIMIT = 20;
    public static string GetUrl(string path)
    {
        string url = null;
        try
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.Android:
                    if (path.Contains(@"jar:file://"))
                        url = path;
                    else
                        url = @"file://" + path;
                    break;

                case RuntimePlatform.OSXEditor:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.WindowsEditor:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.OSXPlayer:
                    url = @"file://" + path;
                    break;

                case RuntimePlatform.WindowsPlayer:
                    url = @"file://" + path;
                    break;
            }

            return url.Replace("\\","/");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    /// <summary>
    /// Get program external address
    /// </summary>
    public static string GetFilePath(string sFolder, string sFileName)
    {
        string filePath = Path.Combine(sFolder, sFileName);
        //if (_useStreamingAssetsPath)
        //{
        //   filePath = Path.Combine(Application.streamingAssetsPath, filePath);
        //}
        // If we're running outside of the editor we may need to resolve the relative path
        // as the working-directory may not be that of the application EXE.
        if (!Application.isEditor && !Path.IsPathRooted(filePath))
        {
            string rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            filePath = Path.Combine(rootPath, filePath);
        }
        return filePath.Replace("\\", "/");
    }

    /// <summary>
    /// Calculates the length of a string in a specified text control
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static int CalculateLengthOfText(string message,UnityEngine.UI.Text text)
    {
        int totalLength = 0;
        Font myFont = text.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(message, text.fontSize, text.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();

        char[] arr = message.ToCharArray();

        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, text.fontSize);

            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
    public static string SplitNameByASCII(string temp,int limitCount = CHARACTER_LIMIT)
    {
        byte[] encodedBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(temp);

        string outputStr = "";
        int count = 0;

        for (int i = 0; i < temp.Length; i++)
        {
            if ((int)encodedBytes[i] == 63)//Double byte
                count += 2;
            else
                count += 1;

            if (count <= limitCount)
                outputStr += temp.Substring(i, 1);
            else if (count > limitCount)
                break;
        }
        if (count <= limitCount)
        {
            outputStr = temp;
        }

        return outputStr;
    }
    long totalSize;
    long position;
    const int BUFFER_SIZE = 10485760;
    byte[] buffer;
    Stream stream;
    string fromPath,toPath;
    public int percent;
    Thread thread;
    public void CopyFile(string strFile,string toFile)
    {
        fromPath = strFile;
        toPath = toFile;
        position = 0;
        percent = 0;
        thread = new Thread(new ThreadStart(CopyStart));
        thread.Start();
    }
    private void CopyStart()
    {
        Thread.Sleep(1000);
        FileStream fs = new FileStream(fromPath, FileMode.Open, FileAccess.Read);
        totalSize = (long)fs.Length;
        stream = fs;
        //Copy file while larger than 4KB.
        if (totalSize > BUFFER_SIZE)
        {
            buffer = new byte[BUFFER_SIZE];
            // Async Invoke
            stream.BeginRead(buffer, 0, BUFFER_SIZE, new AsyncCallback(AsyncCopyFile), null);
        }
        else
        {
            buffer = new byte[totalSize];
            stream.BeginRead(buffer, 0, (int)totalSize, new AsyncCallback(AsyncCopyFile), null);
            //fs.Close();
        }
    }
    private void AsyncCopyFile(IAsyncResult ar)
    {
        int readenLength;

        //Lock FileStream
        lock (stream)
        {
            // When stream endread, get readed length
            readenLength = stream.EndRead(ar);
        }

        // Write to disk
        FileStream fsWriter = new FileStream(toPath, FileMode.Append, FileAccess.Write);
        fsWriter.Write(buffer, 0, buffer.Length);
        fsWriter.Close();

        // Current stream position 
        position += readenLength;
        percent = (int)(position * 1.0f / totalSize * 100);
        //// Response UI
        //MethodInvoker m = new MethodInvoker(SynchProgressBar);
        //m.BeginInvoke(null, null);
        Debug.Log("AsyncCopyFile:Copy progress：" + position);
        if (position >= totalSize)  //read over
        {
            stream.Close();
            thread.Abort();
            //UIManager.Instance.PopUI();
            //UIManager.Instance.PopUI();
            return;
        }

        // Continue to read and write
        lock (stream)
        {
            long leftSize = totalSize - position;

            if (leftSize < BUFFER_SIZE)
            {
                buffer = new byte[leftSize];
            }
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(AsyncCopyFile), null);
        }
    }

    public static Texture2D ReSetTextureSize(Texture2D tex, int width, int height)
    {
        var rendTex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rendTex.Create(); Graphics.SetRenderTarget(rendTex);
        GL.PushMatrix(); GL.Clear(true, true, Color.clear);
        GL.PopMatrix();
        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null) Debug.Log("ReSetTextureSize:" + "shader == null");
        else Debug.Log("ReSetTextureSize:" + "shader != null");
        var mat = new Material(Shader.Find("Unlit/Transparent"));
        mat.mainTexture = tex; Graphics.SetRenderTarget(rendTex);
        GL.PushMatrix();
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(0, 0, 0);
        GL.TexCoord2(0, 1);
        GL.Vertex3(0, 1, 0);
        GL.TexCoord2(1, 1);
        GL.Vertex3(1, 1, 0);
        GL.TexCoord2(1, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
        GL.PopMatrix();
        var finalTex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.ARGB32, false);
        RenderTexture.active = rendTex;
        finalTex.ReadPixels(new Rect(0, 0, finalTex.width, finalTex.height), 0, 0);
        finalTex.Apply();
        return finalTex;
    }

    public static void SaveTexture(Texture2D tex, string toPath)
    {
        using (var fs = File.OpenWrite(toPath))
        {
            var bytes = tex.EncodeToPNG();
            fs.Write(bytes, 0, bytes.Length);
        }
    }

    public static Texture2D LoadTexture2DByIO(string _url)
    {
        //Create file read stream
        FileStream _fileStream = new FileStream(_url, FileMode.Open, FileAccess.Read);
        _fileStream.Seek(0, SeekOrigin.Begin);
        //Create file length buffer
        Debug.Log("LoadTexture2DByIO"+_fileStream.Length);
        byte[] _bytes = new byte[_fileStream.Length];
        _fileStream.Read(_bytes, 0, (int)_fileStream.Length);
        _fileStream.Close();
        _fileStream.Dispose();
        //Create texture
        Texture2D _texture2D = new Texture2D(1, 1);
        //_texture2D.LoadImage(_bytes);
        return _texture2D;
    }
}