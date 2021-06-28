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
        this.transform.rotation = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up);
        if (!isPlayerFPS.Value)
        {
            this.LookAt(Camera.main.transform.forward + (Camera.main.transform.right * .1f));
        }
        else
        {
            Vector3 look = Camera.main.transform.forward;
            look.y = 0;
            this.LookAt(look);
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), rotationSpeed);
    }
}
