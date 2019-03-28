using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeAxisControlTest : MonoBehaviour {

    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    Rigidbody RB;
    bool grab = true;

    void Start () {
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            RB.isKinematic = !RB.isKinematic;
            RB.useGravity = !RB.useGravity;
            grab = !grab;
        }

        if (grab)
        {
            float translationY = -Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float translationZ = Input.GetAxis("LB") * speed * Time.deltaTime;
            transform.Translate(translationX, translationZ, translationY);
        }
    }
}
