using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ChangeLayer : MonoBehaviour
{
    public BoolVariable cameraVariable;
    public LayerMask firstPersonMask;
    public LayerMask thirdPersonMask;

    private Camera _cam;
    
    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        cameraVariable.Changed.Register(ChangeCameraLayer);
        ChangeCameraLayer(cameraVariable.Value);
    }

    private void OnDestroy()
    {
        cameraVariable.Changed.UnregisterAll();
    }

    private void ChangeCameraLayer(bool camera)
    {
        _cam.cullingMask = camera ? firstPersonMask : thirdPersonMask;
    }
}
