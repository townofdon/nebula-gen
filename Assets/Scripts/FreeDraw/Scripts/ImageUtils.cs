
using UnityEngine;
using System.Runtime.InteropServices;

public static class ImageUtils
{
    // NOTE - full path will be /Assets/<DIR_NAME>
    // NOTE - make sure your /Generated directory is excluded from source control
    const string SCREENSHOTS_FOLDER = "Generated/";
    const string SCREENSHOTS_FILENAME = "Nebula";
    const string TIMESTAMP_FORMAT = "yyyyMMddHHmmssfff";
    const string FILE_EXTENSION = "png";

    // see: /Assets/Plugins/download.jslib
    [DllImport("__Internal")] static extern void DownloadFile(byte[] array, int byteLength, string fileName);

    public static void SaveImage(Texture2D texture)
    {
        byte[] byteArray = GetEncodedData(texture, FILE_EXTENSION);

#if (UNITY_EDITOR)
        SaveFileLocal(byteArray);
#elif (UNITY_WEBGL)
        SaveFileWebGL(byteArray);
#elif (UNITY_STANDALONE_WIN)
        SaveFileWindows(byteArray);
#elif (UNITY_STANDALONE_OSX)
        SaveFileOSX(byteArray);
#elif (UNITY_STANDALONE)
        SaveFileOther(byteArray);
#endif
    }

    static byte[] GetEncodedData(Texture2D screenshotTexture, string fileExtension)
    {
        if (fileExtension == "png")
        {
            return screenshotTexture.EncodeToPNG();
        }
        if (fileExtension == "jpg")
        {
            return screenshotTexture.EncodeToJPG();
        }
        throw new UnityException("Screenshot file must be either .png or .jpg");
    }

    static string GetFilename()
    {
        System.DateTime currentTime = System.DateTime.Now;
        string formattedTime = currentTime.ToString(TIMESTAMP_FORMAT);
        return $"{SCREENSHOTS_FILENAME}{formattedTime}.{FILE_EXTENSION}";
    }

    static void SaveFileWebGL(byte[] byteArray)
    {
        DownloadFile(byteArray, byteArray.Length, GetFilename());
    }

    static void SaveFileWindows(byte[] byteArray)
    {
        // note - if this does not work, will try this solution: https://stackoverflow.com/a/61722837
        string homePath = System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        System.IO.File.WriteAllBytes($"{homePath}/Downloads/{GetFilename()}", byteArray);
    }

    static void SaveFileOSX(byte[] byteArray)
    {
        // see: https://stackoverflow.com/questions/1143706/getting-the-path-of-the-home-directory-in-c
        string homePath = System.Environment.GetEnvironmentVariable("HOME");
        System.IO.File.WriteAllBytes($"{homePath}/Downloads/{GetFilename()}", byteArray);
    }

    static void SaveFileOther(byte[] byteArray)
    {
        System.IO.File.WriteAllBytes($"{Application.persistentDataPath}/{GetFilename()}", byteArray);
    }

    static void SaveFileLocal(byte[] byteArray)
    {
        System.IO.Directory.CreateDirectory($"{Application.dataPath}/{SCREENSHOTS_FOLDER}");
        System.IO.File.WriteAllBytes($"{Application.dataPath}/{SCREENSHOTS_FOLDER}{GetFilename()}", byteArray);
    }
}
