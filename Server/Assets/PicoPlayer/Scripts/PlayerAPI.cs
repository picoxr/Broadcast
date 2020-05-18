using System;
using System.Runtime.InteropServices;

public class PlayerAPI
{
    public enum Video3DMode
    {
        Off = 0,
        SideBySide = 1
    }
    public enum DecoderPlayerType
    {
        MediaFoundation = 0,
        ffmp = 1,
    }
    public enum LocalOnline
    {
        Online = 0,
        Local = 1,
    }
    public enum MovieType
    {
        ONLINE,
        LOCAL
    }
    public enum HWAccel
    {
        HWAccel_Off,
        HWAccel_On
    }
    public const string LibFileName = "../../Plugin/UnityMovie";
    //public const string LibFileName = "UnityMovie";

    [DllImport(LibFileName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateSession(DecoderPlayerType playerType, MovieType movieType, HWAccel hwAccel);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroySession(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SetTexturePointers(IntPtr session, IntPtr leftEyeTexture, IntPtr rightEyeTexture);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetLastFrameUploaded(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int VolumeDown(IntPtr session, int value);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int VolumeUp(IntPtr session, int value);

    [DllImport(LibFileName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern int OpenMovie(IntPtr session, string filename, Video3DMode videoMode, int flag);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Play(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Pause(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Stop(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetMovieWidth(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetMovieHeight(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern long GetMovieDuration(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetPosition(IntPtr session, out long position);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SeekTo(IntPtr session, long position);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GerFrame(IntPtr session, IntPtr leftEyeTexture, IntPtr rightEyeTexture);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int UpdateCanCopy();

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetEnd(IntPtr session);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetCachedPosition(IntPtr session, out long position);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetIsBuffering(IntPtr session);//Return value: 1, buffering; 0, no buffering

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetIsPlayFinished(IntPtr session);//Return value: 1, buffering; 0, no buffering

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetDecryptionFilePasswordInputCallback(IntPtr session, PasswordInputCallback passwordInputCallback, IntPtr data);

    [DllImport(LibFileName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetDecryptionFilePasswordIncorrectCallback(IntPtr session, PasswordIncorrectCallback passwordIncorrectCallback, IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate string PasswordInputCallback(IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PasswordIncorrectCallback(IntPtr data);
}
