using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager
{
    public static readonly string sConfigPath = "customConfig.ini";

    public Vector2Int LoadTargetResolution(string widthName, string heightName)
    {
        var absoluteConfigPath = Path.Combine(Directory.GetCurrentDirectory(), sConfigPath);
        var allLines = new List<string>(File.ReadAllLines(absoluteConfigPath));
        var width = ParseValue(widthName, FindLine(widthName, allLines));
        var height = ParseValue(heightName, FindLine(heightName, allLines));
        return new Vector2Int(width, height);
    }

    int ParseValue(string keyName, string line)
    {
        var valueStr = line.Substring(line.IndexOf('=') + 1);
        return int.Parse(valueStr);
    }

    string FindLine(string keyName, List<string> lines)
    {
        return lines.Find((item) => item.Contains(keyName));
    }
}