using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class ShowHideObject : MonoBehaviour
{
    public BoolEvent thisEvent;
    public BoolVariable cameraInFPS;

    public bool cameraNeedToBeInFPS;
    // Start is called before the first frame update
    void Awake()
    {
        thisEvent.Register(Hide);
    }

    // Update is called once per frame
    private void Hide(bool b)
    {
        if (cameraNeedToBeInFPS)
        {
            if(cameraInFPS.Value)
                gameObject.SetActive(b);
        }
        else
        {
            gameObject.SetActive(b);
        }
        
    }
}
