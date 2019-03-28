using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;

public class SphereController : MonoBehaviour {

    public GameObject actor;
    public Text log;
    // Create a sample listener and controller
    LeapListener listener = new LeapListener();
    Controller controller = new Controller();

    void Start () {

        // Have the sample listener receive events from the controller
        controller.SetPolicy(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
        controller.SetPolicyFlags(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
        controller.AddListener(listener);

        /*if (FeederDemoCS.Program.VjoyStart())
        {
            StartCoroutine(FeederDemoCS.Program.TestCoroutine());
        }*/
    }
	
	void Update () {
        if (actor!=null)
        {
            Vector2 dir = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
            log.text = dir.ToString();
        }
	}
}
