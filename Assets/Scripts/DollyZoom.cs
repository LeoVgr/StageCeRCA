using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DollyZoom : MonoBehaviour
{

    public GameObject target;
    public float _horizontalFOVRange = 1.0f;
    private Camera _cam;

    private float _distance;
    private float _angle;
    
    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        //_horizontalFOVRange = _cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        _distance = Vector3.Distance(transform.position, target.transform.position);
        _angle = (180 / Mathf.PI) * Mathf.Atan(_horizontalFOVRange / _distance);
        
        _cam.fieldOfView = _angle * 2;
    }
}
