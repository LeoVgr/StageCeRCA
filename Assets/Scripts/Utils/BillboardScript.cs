using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{

    Transform myTransform;
    Transform target;


    void LateUpdate()
    {
        myTransform.LookAt(target);
        myTransform.Rotate(new Vector3(0f, 180f, 0f));

    }
    void Awake()
    {
        myTransform = transform; //cache the transform
        target = Camera.main.transform; //cace the transform of the camera

    }
}
