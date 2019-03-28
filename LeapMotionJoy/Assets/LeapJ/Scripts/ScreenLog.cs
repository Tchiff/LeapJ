using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenLog : MonoBehaviour {

    public static ScreenLog jLog = null;
    public static ScreenLog JLog
    {
        get
        {
            if (jLog == null)
            {
                GameObject go = new GameObject();
                go.name = "ScreenLogManager";
                return go.AddComponent<ScreenLog>();
            }
            return jLog;
        }
    }

    [SerializeField]
    private Text TextLog;
    [SerializeField]
    private Text UbdateLog;

    Queue<string> dynamicText = new Queue<string>();

    void Awake()
    {
        if (jLog == null)
        {
            jLog = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        if(TextLog == null)
        {
            TextLog = FindObjectOfType<Text>();
        }
    }

    public void PrintToLog(string text)
    {
        TextLog.text += text;
    }

    public void LinkString (string text)
    {
        dynamicText.Enqueue(text);
    }

    void FixedUpdate()
    {
        if (dynamicText.Count > 0 && UbdateLog != null)
        {
            string log = "";
            for(int i = 0;i< dynamicText.Count;i++)
            {
                log += dynamicText.Dequeue() + "\n";
            }
            UbdateLog.text = log;
        }
    }
}
