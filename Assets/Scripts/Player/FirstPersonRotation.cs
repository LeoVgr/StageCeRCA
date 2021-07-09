using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonRotation : MonoBehaviour
{
    public float rotationSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO camera.main is expensive
        this.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        Vector3 look = Camera.main.transform.forward;
        look.y = 0;
        this.LookAt(look);
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), rotationSpeed);
    }
}
