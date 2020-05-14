using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for logging any information, as well as writing end results to file.
/// </summary>
public class Logger : MonoBehaviour
{

    public Text text;

    /// <summary>
    /// Overwrites the log.
    /// </summary>
    /// <param name="str">Sets the log to this.</param>
    public void SetLog(string str)
    {
        text.text = str;
    }

    /// <summary>
    /// Adds to the log.
    /// </summary>
    /// <param name="str">String to add to the log on a new line.</param>
    public void Log(string str)
    {
        text.text += "\n" + str;
    }

    /// <summary>
    /// Clears the log.
    /// </summary>
    public void ClearLog()
    {
        text.text = "Log:\n";
    }

    /// <summary>
    /// Writes the string to the designated filepath.
    /// </summary>
    /// <param name="filename">filename to save to</param>
    /// <param name="text">Text to be saved</param>
    public void WriteToFile(string filename, string text, string filepath = "")
    {

        var fullpath = Application.persistentDataPath + "/" + ((filepath!="") ?  (filepath + "/") : "");

        if (!Directory.Exists(fullpath)) Directory.CreateDirectory(fullpath);

        using (FileStream fs = File.Create(fullpath + filename))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            fs.Write(bytes, 0, bytes.Length);
            Log("WriteToFile ran successfully");
        }
    }

    /// <summary>
    /// Writes all information from a TestPass to a file in JSON format.
    /// </summary>
    /// <param name="tests">The TestPass to be saved.</param>
    public void WriteTestPassToFile(TestPass tests, string path = "tests", string filename = "tests")
    {
        try
        {
            string jsonString = JsonUtility.ToJson(tests);
            Log(jsonString);
            WriteToFile(filename + ".json", jsonString, path);
        }
        catch (System.Exception e)
        {
            Log(e.Message);
        }
    }

    /// <summary>
    /// Writes all information from a Test to a file in JSON format.
    /// </summary>
    /// <param name="tests">The Test to be saved.</param>
    public void WriteTestToFile(TestData test, string path = "", string filename = "test")
    {
        try
        {
            string jsonString = JsonUtility.ToJson(test);
            Log(jsonString);
            WriteToFile(filename + ".json", jsonString, path);
        }
        catch (System.Exception e)
        {
            Log(e.Message);
        }
    }

}
