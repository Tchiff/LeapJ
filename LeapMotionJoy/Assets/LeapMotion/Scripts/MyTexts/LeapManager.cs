using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class LeapManager : MonoBehaviour {

    public KeyCode SaveLog = KeyCode.S;
    public KeyCode ResetLog = KeyCode.R;
    public KeyCode PrintSpace = KeyCode.Space;
    bool isStartBags = false;
    string startStr = "START--------------------------------------";
    string finishStr = "FINISH-------------------------------------";

    //ListenerWrite listener;
    //Controller controller;
    static public WriteText log;

    // Use this for initialization
    void Start () {
        //listener = new ListenerWrite();
        //controller = new Controller();
        log = new WriteText();

        //controller.AddListener(listener);
    }

    private void HandleKeyInputs()
    {
        if (Input.GetKeyDown(SaveLog))
        {
            Debug.Log("Saving...");
            string path = log.SaveToNewFile();
            Debug.Log("File save to: " + path);
        }
        if (Input.GetKeyDown(ResetLog))
        {
            isStartBags = false;
            log.Reset();
            Debug.Log("Log clean");
        }
        if(Input.GetKeyDown(PrintSpace))
        {
            isStartBags = !isStartBags;
            log.Add(isStartBags ? startStr : finishStr);
            Debug.Log(isStartBags ? startStr : finishStr);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        HandleKeyInputs();
    }
}
