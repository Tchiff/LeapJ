using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ComandManagr : MonoBehaviour {

    public GameObject Tool;

    SphereMove MoveTool;
    Rigidbody SphereRB;
    HandModel hand_model;
    Hand leap_hand;
    CommandGesture command;

    public enum CommandGesture
    {
        NULL,
        Grab,
        Contact
    }

    //public delegate void ComandAction(CommandGesture command);
    //public event ComandAction ComandStart;
    //public event ComandAction ComandFinish;
    /*
    ~ComandManagr()
    {
        MoveTool.MoveHome();
    }*/

    void Start () {
        hand_model = GetComponent<HandModel>();
        leap_hand = hand_model.GetLeapHand();
        SphereRB = Tool.GetComponent<Rigidbody>();
        MoveTool = Tool.GetComponent<SphereMove>();
    }
	
	void Update ()
    {
        if (leap_hand == null)
            return;

        Vector3 centroid = GetCentroidFingers();

        if (command == CommandGesture.NULL)
        {
            if (isComandGrab(centroid, JoyConfig.Config.DistanceGrab))
            {
                command = CommandGesture.Grab;
            }
        }
        else
        {
            switch (command)
            {
                case CommandGesture.Grab:
                    if (!isComandGrab(centroid, JoyConfig.Config.DistanceRelease))
                    {
                        command = CommandGesture.NULL;
                        MoveTool.MoveHome();
                    }
                    else
                    {
                        Tool.transform.rotation = hand_model.GetPalmRotation();
                        FingerModel midle = hand_model.fingers[2];
                        MoveTool.MoveTo(midle.GetJointPosition(1));
                    }
                    break;
                default:
                    break;
            }
        }
    }

    Vector3 GetCentroidFingers()
    {
        Vector3 sum = Vector3.zero;
        foreach (FingerModel finger in hand_model.fingers)
        {
            sum += finger.GetTipPosition();
        }
        return sum / hand_model.fingers.Length;
    }

    bool isComandGrab(Vector3 centroid, float dist)
    {
        foreach (FingerModel finger in hand_model.fingers)
        {
            float distan = (centroid - finger.GetTipPosition()).sqrMagnitude;
            if (distan > dist)
                return false;
        }
        return true;
    }
}
