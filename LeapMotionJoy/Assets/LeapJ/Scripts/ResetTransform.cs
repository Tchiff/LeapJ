using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTransform : MonoBehaviour {

    Vector3 StartPosition;
    Quaternion StartRotation;
    Rigidbody RB;

    private void Start()
    {
        StartPosition = transform.position;
        StartRotation = transform.rotation;
        RB = GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetButtonUp("Fire2"))
        {
            gameObject.transform.position = StartPosition;
            gameObject.transform.rotation = StartRotation;
            Debug.Log(gameObject.name);
        }
    }
}
