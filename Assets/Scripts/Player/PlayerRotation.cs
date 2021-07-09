using UnityAtoms.BaseAtoms;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public float rotationSpeed = 1;
    
    public BoolVariable isPlayerFPS;
    public BoolVariable isPlayerLock;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mainTransform = Camera.main.transform;
        this.transform.rotation = Quaternion.AngleAxis(mainTransform.rotation.eulerAngles.y, Vector3.up);
        if (!isPlayerFPS.Value)
        {
            var transformForward = mainTransform.forward + (mainTransform.right * .1f);
            //Stop rotation up or down
            //transformForward.y = 0;
            this.LookAt(transformForward);
        }
        else
        {
            Vector3 look = mainTransform.forward;
            look.y = 0;
            this.LookAt(look);
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), rotationSpeed);
    }
}
