using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vJoyInterfaceWrap;
using Leap;

public class JoySphere : MonoBehaviour {

    public GameObject Tool;
    public uint id = 1;

    vJoy joystick;
    vJoy.JoystickState iReport;
    LeapListener listener;
    Controller controller;

    int AxisZero = 0;
    int AxisMax = 0;

    ~JoySphere()
    {
        AxisReset();
    }

    void Start ()
    {
        if (VjoyStart())
        {
            StartCoroutine(Simulation());
            ScreenLog.JLog.PrintToLog("START!");
        }
    }

    bool VjoyStart()
    {
        joystick = new vJoy();
        iReport = new vJoy.JoystickState();

        // Device ID can only be in the range 1-16
        if (id <= 0 || id > 16)
        {
            ScreenLog.JLog.PrintToLog(string.Format("Illegal device ID {0}\nExit!", id));
            return false;
        }

        // Get the driver attributes (Vendor ID, Product ID, Version Number)
        if (!joystick.vJoyEnabled())
        {
            ScreenLog.JLog.PrintToLog(string.Format("vJoy driver not enabled: Failed Getting vJoy attributes.\n"));
            return false;
        }

        // Get the state of the requested device
        VjdStat status = joystick.GetVJDStatus(id);
        switch (status)
        {
            case VjdStat.VJD_STAT_OWN:
                ScreenLog.JLog.PrintToLog(string.Format("vJoy Device {0} is already owned by this feeder\n", id));
                break;
            case VjdStat.VJD_STAT_FREE:
                ScreenLog.JLog.PrintToLog(string.Format("vJoy Device {0} is free\n", id));
                break;
            case VjdStat.VJD_STAT_BUSY:
                ScreenLog.JLog.PrintToLog(string.Format("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id));
                return false;
            case VjdStat.VJD_STAT_MISS:
                ScreenLog.JLog.PrintToLog(string.Format("vJoy Device {0} is not installed or disabled\nCannot continue\n", id));
                return false;
            default:
                ScreenLog.JLog.PrintToLog(string.Format("vJoy Device {0} general error\nCannot continue\n", id));
                return false;
        };

        // Check which axes are supported
        bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
        bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
        bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
        bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
        bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);

        // Get the number of buttons and POV Hat switchessupported by this vJoy device
        int nButtons = joystick.GetVJDButtonNumber(id);
        int ContPovNumber = joystick.GetVJDContPovNumber(id);
        int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

        // Print results
        ScreenLog.JLog.PrintToLog(string.Format("\nvJoy Device {0} capabilities:\n", id));
        ScreenLog.JLog.PrintToLog(string.Format("Numner of buttons\t\t{0}\n", nButtons));
        ScreenLog.JLog.PrintToLog(string.Format("Numner of Continuous POVs\t{0}\n", ContPovNumber));
        ScreenLog.JLog.PrintToLog(string.Format("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber));
        ScreenLog.JLog.PrintToLog(string.Format("Axis X\t\t{0}\n", AxisX ? "Yes" : "No"));
        ScreenLog.JLog.PrintToLog(string.Format("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No"));
        ScreenLog.JLog.PrintToLog(string.Format("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No"));
        ScreenLog.JLog.PrintToLog(string.Format("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No"));
        ScreenLog.JLog.PrintToLog(string.Format("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No"));

        // Test if DLL matches the driver
        uint DllVer = 0, DrvVer = 0;
        bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
        if (match)
            ScreenLog.JLog.PrintToLog(string.Format("Version of Driver Matches DLL Version ({0:X})\n", DllVer));
        else
            ScreenLog.JLog.PrintToLog(string.Format("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer));

        // Acquire the target
        bool stat = ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE)));
        bool acq = (!joystick.AcquireVJD(id));
        if (stat && acq)
        {
            ScreenLog.JLog.PrintToLog(string.Format("Failed to acquire vJoy device number {0}.\n", id));
            return false;
        }

        listener = new LeapListener();
        controller = new Controller();
        joystick.ResetVJD(id);

        return true;
    }

    IEnumerator Simulation()
    {
        controller.SetPolicy(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
        controller.SetPolicyFlags(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
        controller.AddListener(listener);

        int ContPovNumber = joystick.GetVJDContPovNumber(id);
        int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

        //uint count = 0;
        long maxval = 0;
        joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);

        AxisMax = (int)maxval;
        AxisZero = AxisMax / 2;
        int AxisX = AxisZero;
        int AxisY = AxisZero;
        int AxisZ = AxisZero;
        int AxisZR = AxisZero;
        int AxisXR = AxisZero;
        AxisReset();

        Vector3 OldPosition = Tool.transform.localPosition;

        while (true)
        {
            Vector3 pos = Tool.transform.localPosition;
            if (Mathf.Abs(OldPosition.sqrMagnitude - pos.sqrMagnitude) > 0.0001f)
            {
                AxisX = pos.x > 0 ? AxisPositive(pos.x, JoyConfig.Config.MaxPosition.x, JoyConfig.Config.Deadband.x) :
                                    AxisNegative(pos.x, JoyConfig.Config.MinPosition.x, -JoyConfig.Config.Deadband.x);

                AxisZ = pos.y > 0 ? AxisPositive(pos.y, JoyConfig.Config.MaxPosition.y, JoyConfig.Config.Deadband.y) :
                                    AxisNegative(pos.y, JoyConfig.Config.MinPosition.y, -JoyConfig.Config.Deadband.y);

                AxisY = pos.z > 0 ? AxisPositive(pos.z, JoyConfig.Config.MaxPosition.z, JoyConfig.Config.Deadband.z) :
                                    AxisNegative(pos.z, JoyConfig.Config.MinPosition.z, -JoyConfig.Config.Deadband.z);

                joystick.SetAxis(AxisX, id, HID_USAGES.HID_USAGE_X);
                joystick.SetAxis(AxisY, id, HID_USAGES.HID_USAGE_Y);
                joystick.SetAxis(AxisZ, id, HID_USAGES.HID_USAGE_Z);

                OldPosition = Tool.transform.localPosition;
            }
            ScreenLog.JLog.LinkString("AxisX = " + (float)AxisX / AxisMax);
            ScreenLog.JLog.LinkString("AxisY = " + (float)AxisY / AxisMax);
            ScreenLog.JLog.LinkString("AxisZ = " + (float)AxisZ / AxisMax);
            yield return new WaitForFixedUpdate();
        }
    }

    int AxisPositive(float pos, float maxPos, float deadband)
    {
        if (pos < deadband)
            return AxisZero;

        int axis = (int)(((pos - deadband) * (AxisMax - AxisZero) / (maxPos - deadband)) + AxisZero);
        axis = Mathf.Min(axis, AxisMax);
        axis = Mathf.Max(axis, 0);

        return axis;
    }

    int AxisNegative(float pos, float minPos, float deadband)
    {
        if (pos > deadband)
            return AxisZero;

        int axis = (int)((pos - minPos) * AxisZero / (deadband - minPos));
        axis = Mathf.Min(axis, AxisZero);
        axis = Mathf.Max(axis, 0);

        return axis;
    }

    void AxisReset()
    {
        joystick.SetAxis(AxisZero, id, HID_USAGES.HID_USAGE_X);
        joystick.SetAxis(AxisZero, id, HID_USAGES.HID_USAGE_Y);
        joystick.SetAxis(AxisZero, id, HID_USAGES.HID_USAGE_Z);
    }

}
