using System;
using System.IO;
using System.Collections.Generic;

public class AssetManager
{
    public static Dictionary<string, SimpleAssetBundle> LoadedBundles = new Dictionary<string, SimpleAssetBundle>();
    static object lockObj = new object();

    #region 命令行测试
    static void Main(string[] args)
    {
        LoadedBundles.Clear();
        SimpleAssetBundle.LoadedBundle.Clear();
        WaitForConsole();
    }

    static void WaitForConsole()
    {
        Console.WriteLine("-help显示帮助信息\npack打包\nunpack读取文件");
        string cmd = Console.ReadLine();
        if (cmd == "" || cmd == " ")
        {
            WaitForConsole();
            return;
        }

        if (cmd == "-h" || cmd == "-help")
        {
            DisplayHelpMsg();
            return;
        }

        if (cmd == "pack")
        {
            StartPack();
            return;
        }

        if (cmd == "unpack")
        {
            StartUnpack();
            return;
        }

        if (cmd == "exit")
        {
            return;
        }
    }

    static void DisplayHelpMsg()
    {
        string helpMsg = @"
打包命令:pack
解包命令:unpack
            ";
        Console.WriteLine(helpMsg);
        WaitForConsole();
    }

    static void StartPack()
    {
        Console.WriteLine("请输入目标路径和需要打包的文件，以空格区分");
        string cmd = Console.ReadLine();
        string[] split = cmd.Split(' ');
        string[] filesPath = new string[split.Length - 1];
        for (int i = 0; i < filesPath.Length; i++)
        {
            filesPath[i] = split[i + 1];
        }
        Pack(split[0], filesPath);
        WaitForConsole();
    }

    static void StartUnpack()
    {
        Console.WriteLine("请输入已打包文件路径");
        string cmd = Console.ReadLine();
        Unpack(cmd);
        WaitForConsole();
    }

    public static void Pack(string path, params string[] filesPath)
    {
        SimpleAssetBundle.GenerateBundle(path, filesPath);
    }

    public static void Unpack(string path)
    {
        Console.WriteLine("请输入想要提取的文件名称和想要保存的路径");
        string cmd = Console.ReadLine();
        string[] split = cmd.Split(' ');
        CheckFile(split[0], split[1], path);
    }

    static void CheckFile(string fileName, string filePath, string bundlePath)
    {
        SimpleAssetBundle sab = LoadFromFile(bundlePath);
        if (sab == null)
        {
            Console.WriteLine("Bundle 包不存在");
            return;
        }
        if (sab.isExists(fileName))
        {
            Console.WriteLine("文件存在");
            using (BinaryWriter bw = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                int len = sab.FileLength(fileName);
                byte[] bytes = new byte[len];//sab.GetData(fileName);
                sab.Read(fileName, bytes, 0, len);
                bw.Write(bytes);
            }
        }
        else
        {
            Console.WriteLine("文件不存在");
        }
    }
    #endregion

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