using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class POVAddOn : MonoBehaviour
{
    public FloatVariable a_lookSpeed;
    public bool InvertY = false;
    public BoolVariable a_isPlayerLock;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachinePOV _pov;
    
    public void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    // Update the look movement each time the event is trigger
   
// Update the look movement each time the event is trigger
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!a_isPlayerLock.Value)
        {
            float lookSpeed = a_lookSpeed.Value / 15.0f;
            
            //Normalize the vector to have an uniform vector in whichever form it came from (I.E Gamepad, mouse, etc)
            Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
            lookMovement.y = InvertY ? -lookMovement.y : lookMovement.y;

            // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
            lookMovement.x *= 180f;
            lookMovement.y *= 180f;

            //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
            _pov.m_HorizontalAxis.Value += lookMovement.x * lookSpeed * Time.deltaTime;
            _pov.m_VerticalAxis.Value +=   lookMovement.y * lookSpeed * Time.deltaTime;
            
        }
    }
}