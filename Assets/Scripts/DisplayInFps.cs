using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class DisplayInFps : MonoBehaviour
{
    public BoolVariable isGameInFPS;
    
    // Start is called before the first frame update
    void Start()
    {
        isGameInFPS.Changed.Register(SetActive);
        SetActive(isGameInFPS.Value);
    }

    private void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }
}
