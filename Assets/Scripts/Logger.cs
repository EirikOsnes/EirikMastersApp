﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{

    public Text text;

    public void SetLog(string str)
    {
        text.text = str;
    }

    public void Log(string str)
    {
        text.text += "\n" + str;
    }

    public void ClearLog()
    {
        text.text = "Log:\n";
    }

    public void WriteToFile(string filePath, string text)
    {
        using (FileStream fs = File.Create(Application.persistentDataPath + filePath))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            fs.Write(bytes, 0, bytes.Length);
            Log("WriteToFile ran successfully");
        }
    }

    public void WriteTestToFile(TestPass tests)
    {
        try
        {
            string jsonString = JsonUtility.ToJson(tests);
            Log(jsonString);
            WriteToFile("/test.json", jsonString);
        }
        catch (System.Exception e)
        {
            Log(e.Message);
        }
    }

}
