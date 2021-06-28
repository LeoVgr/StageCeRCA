using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Random = UnityEngine.Random;

public class FeetTarget : MonoBehaviour
{

    public GameObject feetLink;
    public GameObject feetObject;
    public FloatReference floatVar;
    public FeetTarget oppositeFeet;
    public bool isGrounded = true;
    public float time = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = feetObject.transform.position + Vector3.forward * 0.5f;
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(feetObject.transform.position, transform.position) > floatVar && oppositeFeet.isGrounded && isGrounded)
        {
           Sequence moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => isGrounded = false);
            moveSequence.Append(feetLink.transform.DOMoveX(transform.position.x, time));
            moveSequence.Join(feetLink.transform.DOMoveZ(transform.position.z, time));
            moveSequence.Join(feetLink.transform.DOMoveY(transform.position.y + 0.15f, time));
                //moveSequence.Append(feetLink.transform.DOMoveY(transform.position.y, time/2.0f));
            moveSequence.AppendCallback(() => isGrounded = true);
        }
    }

    private void LateUpdate()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = hitInfo.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawSphere(position, 0.1f);
        Gizmos.DrawLine(position, feetObject.transform.position);
    }
}
