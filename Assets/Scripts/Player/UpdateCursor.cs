using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class UpdateCursor : MonoBehaviour
    {
        public Image CrossHair;
        public LayerMask TargetLayer;
        public BoolVariable IsCrosshairColorized;

        // Update is called once per frame
        void Update()
        {
            //Check if we have to apply crosshair colorization 
            if (IsCrosshairColorized.Value){

                //Change the color of the crosshair if the target aimed has to be shot or not
                Ray ray = Camera.main.ScreenPointToRay(CrossHair.gameObject.transform.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, TargetLayer))
                {
                    Target target = hit.collider.gameObject.GetComponent<Target>();
                    if (target)
                    {
                        CrossHair.color = target.HasToBeShot ? Color.green : Color.red;
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
    }
}
