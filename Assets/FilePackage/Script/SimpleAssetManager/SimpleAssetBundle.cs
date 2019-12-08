using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class SimpleAssetBundle
{
    public Dictionary<uint, BundleItemInfo> FileNameLookupMap = new Dictionary<uint, BundleItemInfo>();
    public Dictionary<uint, BundleItemInfo> FullNameLookupMap = new Dictionary<uint, BundleItemInfo>();
    public FileStream contentStream;
    public int contentStreamOffset;
    public static long contentByteLength;
    static object lockObj = new object();
    public static Dictionary<string, SimpleAssetBundle> LoadedBundle = new Dictionary<string, SimpleAssetBundle>();

    #region 文件查询和访问操作

    public bool isExists(string file)
    {
        string fileLower = file.ToLower();
        uint hash = fileLower.GuiGetHashCode();
        if (FileNameLookupMap.ContainsKey(hash) || FullNameLookupMap.ContainsKey(hash))
        {
            return true;
        }

        return false;
    }

    public byte[] GetData(string file)
    {
        if (this.isExists(file))
        {
            string fileLower = file.ToLower();
            uint hash = fileLower.GuiGetHashCode();
            BundleItemInfo info = FileNameLookupMap.ContainsKey(hash) ? FileNameLookupMap[hash] : FullNameLookupMap[hash];
            byte[] data = new byte[info.length];
            lock (lockObj)
            {
                this.contentStream.Seek(this.contentStreamOffset + info.startIdx, SeekOrigin.Begin);
                this.contentStream.Read(data, 0, info.length);
            }
            return data;
        }

        return null;
    }

    public int FileLength(string file)
    {
        if (isExists(file))
        {
            string fileLower = file.ToLower();
            uint hash = fileLower.GuiGetHashCode();
            BundleItemInfo info = FileNameLookupMap.ContainsKey(hash) ? FileNameLookupMap[hash] : FullNameLookupMap[hash];
            return info.length;
        }
        return 0;
    }

    public void Read(string file, byte[] buffer,int offset,int count)
    {
        if (this.isExists(file))
        {
            string fileLower = file.ToLower();
            uint hash = fileLower.GuiGetHashCode();
            BundleItemInfo info = FileNameLookupMap.ContainsKey(hash) ? FileNameLookupMap[hash] : FullNameLookupMap[hash];
            
            lock (lockObj)
            {
                this.contentStream.Seek(this.contentStreamOffset + info.startIdx, SeekOrigin.Begin);
                this.contentStream.Read(buffer, offset, count);
            }
        }
    }
    #endregion

    static void GetPackageFileInfo(params string[] bundleFilePath)
    {
        foreach (string path in bundleFilePath)
        {
            if (Directory.Exists(path))
            {//目录
                GetFileInfoAtDirectory(path, "");
            }

            if (File.Exists(path))
            {//文件
                GetFileInfoAtFile(path, "");
            }
        }
    }

    static void GetFileInfoAtDirectory(string folderFullName, string parentPath)
    {
        DirectoryInfo dir = new DirectoryInfo(folderFullName);
        string myPath;
        if (parentPath == "")
        {
            myPath = dir.Name;
        }
        else
        {
            myPath = string.Format("{0}/{1}", parentPath, dir.Name);
        }

        foreach (DirectoryInfo di in dir.GetDirectories())
        {
            GetFileInfoAtDirectory(di.FullName, myPath);
        }
        foreach (FileInfo fi in dir.GetFiles())
        {
            GetFileInfoAtFile(fi.FullName, myPath);
        }
    }

    static void GetFileInfoAtFile(string fileFullName, string parentPath)
    {
        FileInfo fi = new FileInfo(fileFullName);
        string path;
        if (parentPath == "")
        {
            path = fi.Name;
        }
        else
        {
            path = string.Format("{0}/{1}", parentPath, fi.Name);
        }

        int startIdx = (int)contentByteLength;
        int fileLength = (int)fi.Length;
        contentByteLength += fi.Length;
        string lowerPath = path.ToLower();
        string lowerName = fi.Name.ToLower();
        uint filePathHashID = lowerPath.GuiGetHashCode();
        uint fileNameHashID = lowerName.GuiGetHashCode();
        Manifest.Append(string.Format("{0},{1},{2},{3}|", filePathHashID, fileNameHashID, startIdx, fileLength));
    }

    static StringBuilder Manifest = new StringBuilder();
    static List<byte[]> BundleContentByteList = new List<byte[]>();
    /// <summary>
    /// 生成Bundle包
    /// </summary>
    /// <param name="filePath">打包好的文件生成路径</param>
    /// <param name="bundleFilePath">需要打包的文件</param>
    public static void GenerateBundle(string filePath, params string[] bundleFilePath)
    {
        contentByteLength = 0;
        Manifest.Remove(0, Manifest.Length);
        BundleContentByteList.Clear();
        GetPackageFileInfo(bundleFilePath);
        Console.WriteLine(Manifest.ToString());

        using (BinaryWriter bundleWriter = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            string bundleInfo = Manifest.ToString();
            byte[] bundleInfoByte = Encoding.UTF8.GetBytes(bundleInfo);//索引头第二部分 索引信息

            int length = bundleInfoByte.Length;
            byte[] lengthByte = System.BitConverter.GetBytes(length);//索引头第一部分 索引信息的长度
            if(!BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthByte);
            }
            bundleWriter.Write(lengthByte);
            bundleWriter.Write(bundleInfoByte);

            foreach (string path in bundleFilePath)
            {
                if (Directory.Exists(path))
                {//目录
                    AddDirectory(bundleWriter, path);
                }

                if (File.Exists(path))
                {//文件
                    AddFile(bundleWriter, path);
                }
            }
        }
    }

    public static void AddDirectory(BinaryWriter bundleWriter, string folderFullName)
    {
        DirectoryInfo dir = new DirectoryInfo(folderFullName);
       
        foreach (DirectoryInfo di in dir.GetDirectories())
        {
            AddDirectory(bundleWriter, di.FullName);
        }
        foreach (FileInfo fi in dir.GetFiles())
        {
            AddFile(bundleWriter, fi.FullName);
        }
    }

    public static void AddFile(BinaryWriter bundleWriter, string fileFullName)
    {
        using (FileStream fs = File.OpenRead(fileFullName))
        {
            fs.Seek(0, SeekOrigin.Begin);
            int cache = fs.ReadByte();
            byte[] cacheByte = BitConverter.GetBytes(cache);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(cacheByte);
            }
            while(cache != -1)
            {
                bundleWriter.Write(cacheByte[0]);
                cache = fs.ReadByte();
                cacheByte = BitConverter.GetBytes(cache);
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(cacheByte);
                }
            }
        }
    }

    public static SimpleAssetBundle GetBundleAtPath(string filePath)
    {
        lock (lockObj) {
            if (LoadedBundle.ContainsKey(filePath))
            {
                return LoadedBundle[filePath];
            }
            else
            {
                SimpleAssetBundle sab = LoadBundleAtPath(filePath);
                if (sab == null)
                {
                    return null;
                }
                LoadedBundle.Add(filePath, sab);
                return sab;
            }
        }
    }

    static SimpleAssetBundle LoadBundleAtPath(string bundlePath)
    {
        if (!File.Exists(bundlePath))
        {
            Console.WriteLine("LoadBundleAtPath failed, no such a path");
            return null;
        }

        FileStream fs = File.OpenRead(bundlePath);

        byte[] headLengthByte = new byte[4];
        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(headLengthByte, 0, 4);

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(headLengthByte);
        }

        int bundleInfoLength = BitConverter.ToInt32(headLengthByte, 0);

        byte[] bundleInfoByte = new byte[bundleInfoLength];
        fs.Seek(4, SeekOrigin.Begin);
        fs.Read(bundleInfoByte, 0, bundleInfoLength);

        SimpleAssetBundle assetBundle = new SimpleAssetBundle();
        assetBundle.contentStream = fs;
        assetBundle.contentStreamOffset = 4 + bundleInfoLength;

        string bundleInfoStr = Encoding.UTF8.GetString(bundleInfoByte);
        string[] bundleInfos = bundleInfoStr.Split('|');
        foreach (string info in bundleInfos)
        {
            if (!info.Contains(","))
            {
                continue;
            }

            string[] bundleItemInfo = info.Split(',');
            BundleItemInfo itemInfo = new BundleItemInfo(Convert.ToUInt32(bundleItemInfo[0]), Convert.ToUInt32(bundleItemInfo[1]), Convert.ToInt32(bundleItemInfo[2]), Convert.ToInt32(bundleItemInfo[3]));
            assetBundle.FileNameLookupMap.Add(itemInfo.fileHashID, itemInfo);
            assetBundle.FullNameLookupMap.Add(itemInfo.pathHashID, itemInfo);
        }
        return assetBundle;
    }

    public static void UnloadAllBundle()
    {
        foreach (SimpleAssetBundle sab in LoadedBundle.Values)
        {
            sab.Unload();
        }
        LoadedBundle.Clear();
    }

    public void Unload()
    {
        FileNameLookupMap.Clear();
        FullNameLookupMap.Clear();
        contentStream.Close();
    }
}

public class BundleItemInfo
{
    public uint pathHashID;
    public uint fileHashID;
    public int startIdx;
    public int length;

    public BundleItemInfo(uint path, uint name, int start, int len)
    {
        this.pathHashID = path;
        this.fileHashID = name;
        this.startIdx = start;
        this.length = len;
    }
}