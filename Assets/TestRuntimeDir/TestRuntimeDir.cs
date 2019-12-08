using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TestRuntimeDir : MonoBehaviour
{
    public Text mPath;

    void Start()
    {
        var sbd = new StringBuilder();
        sbd.AppendLine(Directory.GetCurrentDirectory());
        mPath.text = sbd.ToString();
    }
}