using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ScaleDownHead : MonoBehaviour
{
    public BoolEvent isGameInFps;

    private Vector3 baseScale;
    
    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        isGameInFps.Register(SetScale);
    }

    private void SetScale(bool inFps)
    {
        transform.localScale = inFps ? Vector3.zero : baseScale;
    }
}
