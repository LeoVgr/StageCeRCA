using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBody : MonoBehaviour
{

    public float speed = 0.5f;
    public float offsetY = 0.75f;
    
    public GameObject[] legs;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 addVec = Vector3.forward * (speed * Time.deltaTime);
        float yPos = GetAveragePosition().y + offsetY;

        var position = transform.position;
        position += addVec;
        position = new Vector3( position.x, yPos,  position.z);
        transform.position = position;
    }

    private Vector3 GetAveragePosition()
    {
        Vector3 averagePos = Vector3.zero;

        for (var i = 0; i < legs.Length; i++)
        {
            averagePos += legs[i].transform.position;
        }
        averagePos /= legs.Length > 0 ? legs.Length : 1;

        return averagePos;
    }
}
