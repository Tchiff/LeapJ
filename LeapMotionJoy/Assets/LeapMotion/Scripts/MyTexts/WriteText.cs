using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class WriteText
{
    string name; //имя файла
    List<string> hash; //очередь записи


    //создание файла
    public WriteText()
    {
            hash = new List<string>();
    }

    //добавить в список для записи
    public void Add(string text)
    {
        hash.Add(text);
    }

    public void Reset()
    {
        hash.Clear();
    }

    public string SaveToNewFile()
    {
        string path = Application.persistentDataPath + "/Log_" +
                      System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt";
        return SaveToNewFile(path);
    }

    public string SaveToNewFile(string path)
    {
        if (File.Exists(@path))
        {
            File.Delete(@path);
        }

        using (StreamWriter sw = File.AppendText(path))
        {
            foreach (string i in hash)
                sw.WriteLine(i);
        }
        return path;
    }

}
