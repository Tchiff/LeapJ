using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyConfig : MonoBehaviour {

    public static JoyConfig config = null;
    public static JoyConfig Config
    {
        get
        {
            if (config == null)
            {
                //config = FindObjectOfType<JoyConfig>();

               // if (config == null)
                //{
                    GameObject go = new GameObject();
                    go.name = "ConfigManager";
                return go.AddComponent<JoyConfig>();
                    //DontDestroyOnLoad(go);
                //}
            }
            return config;
        }
    }

    public Vector3 MaxPosition { get; private set; }
    public Vector3 MinPosition { get; private set; }
    public Vector3 Deadband { get; private set; }
    public Vector3 OffcetPosition { get; private set; }
    //Calibration Grab
    public float DistanceGrab { get; private set; }
    public float DistanceRelease { get; private set; }


    void Awake()
    {
        if (config == null)
        {
            config = this;
            DontDestroyOnLoad(gameObject);
            PlayerPrefs.DeleteAll();//ВРЕМЕННО
            InitializeConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }
	
	void InitializeConfig()
    {
        MaxPosition = GetVector3("MaxPosition", new Vector3(1.5f, 1.5f, 1.5f));
        MinPosition = GetVector3("MinPosition", new Vector3(-1.5f, -1.2f, -1.2f));
        Deadband = GetVector3("Deadband", new Vector3(0.15f, 0.15f, 0.15f));
        OffcetPosition = GetVector3("OffcetPosition", Vector3.zero);
        DistanceGrab = PlayerPrefs.GetFloat("DistanceGrab", 1f);
        DistanceRelease = PlayerPrefs.GetFloat("DistanceRelease", 1.4f);
    }

    Vector3 GetVector3(string key, Vector3 value)
    {
        Vector3 ret = new Vector3();
        ret.x = PlayerPrefs.GetFloat(key+"_X", value.x);
        ret.y = PlayerPrefs.GetFloat(key + "_Y", value.y);
        ret.z = PlayerPrefs.GetFloat(key + "_Z", value.z);
        return ret;
    }

    void SetVector3(string key, Vector3 value)
    {
        PlayerPrefs.SetFloat(key + "_X", value.x);
        PlayerPrefs.GetFloat(key + "_Y", value.y);
        PlayerPrefs.GetFloat(key + "_Z", value.z);
    }
}
