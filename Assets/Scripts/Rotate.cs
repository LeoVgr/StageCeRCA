using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public Vector3 rotateAround;
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, rotateAround, rotateSpeed);
    }
}
