using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.Win32;
using UnityEngine;

public class AimPosition : MonoBehaviour
{
    public Transform Aim;
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position,transform.forward);
        if (Physics.Raycast(ray, out var hitInfo, 100f))
        {
            Aim.position = hitInfo.point;
        }
        else
        {
            Aim.position = ray.GetPoint(100f);
        }
    }
}
