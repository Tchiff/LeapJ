using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMove : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        Vector2 dir = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        transform.Translate(dir);
    }
}
