using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rig))]
public class ChangeRigWeight : MonoBehaviour
{
    public BoolVariable cameraVariable;

    private Rig _rig;
    
    // Start is called before the first frame update
    void Start()
    {
        _rig = GetComponent<Rig>();
        cameraVariable.Changed.Register(ChaneRig);
        ChaneRig(cameraVariable.Value);
    }

    private void OnDestroy()
    {
        cameraVariable.Changed.UnregisterAll();
    }

    private void ChaneRig(bool camera)
    {
        _rig.weight = camera ? 1.0f : 0.0f;
    }
}
