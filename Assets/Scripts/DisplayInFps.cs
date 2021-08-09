using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class DisplayInFps : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.FpsCamera.Changed.Register(SetActive);
        SetActive(DataManager.instance.FpsCamera.Value);
    }

    private void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
