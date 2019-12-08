using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestAssetManager : MonoBehaviour
{
    public static Dictionary<string, SimpleAssetBundle> LoadedBundles = new Dictionary<string, SimpleAssetBundle>();
    static object lockObj = new object();
    public string bundlePath;
    public string fileName;
    public string filePath;

    void Start()
    {
        TestLoadAsset();
    }

    void TestLoadAsset()
    {
        SimpleAssetBundle sab = LoadFromFile(bundlePath);
        if (sab.isExists(fileName))
        {
            Debug.LogError("文件存在");
            using (BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                int len = sab.FileLength(fileName);
                byte[] bytes = new byte[len];
                sab.Read(fileName, bytes, 0, len);
                bw.Write(bytes);
            }
        }
    }

    public static SimpleAssetBundle LoadFromFile(string filePath)
    {
        lock (lockObj)
        {
            SimpleAssetBundle ab = null;
            if (LoadedBundles.TryGetValue(filePath, out ab))
            {
                return ab;
            }

            if (File.Exists(filePath))
            {
                ab = SimpleAssetBundle.GetBundleAtPath(filePath);
                if (ab == null)
                {
                    return null;
                }
                LoadedBundles.Add(filePath, ab);
                return ab;
            }
        }
        return null;

    }
}
