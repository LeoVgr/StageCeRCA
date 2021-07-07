using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCursor : MonoBehaviour
{
    public Image CrossHair;
    public LayerMask TargetLayer;

    // Update is called once per frame
    void Update()
    {
        //Change the color of the crosshair if the target aimed has to be shot or not
        Ray ray = Camera.main.ScreenPointToRay(CrossHair.gameObject.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, TargetLayer))
        {
            if (hit.collider.gameObject.GetComponent<Target>())
            {
                if (hit.collider.gameObject.GetComponent<Target>().HasToBeShot)
                {                  
                    CrossHair.color = Color.green;
                }
                else
                {                
                    CrossHair.color = Color.red;
                }
                
            }
            else
            {
                CrossHair.color = Color.white;
            }
        }
        else
        {
            CrossHair.color = Color.white;
        }
    }
}
