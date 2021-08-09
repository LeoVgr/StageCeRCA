using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOnOverAnimation : MonoBehaviour
{
    private Vector3 _initialScale;
    private Quaternion _initialRotation;

    private void Start()
    {
        _initialScale = transform.localScale;
        _initialRotation = transform.rotation;
    }
    public void HighLight()
    {
        //Make sure to start animatioon only if previous DoPunchScale has finished
        if (transform.localScale == _initialScale && transform.rotation == _initialRotation)
        {
            transform.DOPunchScale(transform.localScale * 0.1f, 0.5f).SetUpdate(true);
            transform.DOPunchRotation(new Vector3(0, 0, 5), 0.5f).SetUpdate(true);
        }
        
    }
}
