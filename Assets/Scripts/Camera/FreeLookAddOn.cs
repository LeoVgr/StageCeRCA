using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookAddOn : MonoBehaviour
{
    public BoolEvent isGameInFPSEvent;
    public FloatVariable a_lookSpeed;
    public bool InvertY = false;
    public BoolVariable a_isPlayerLock;

    private CinemachineFreeLook _freeLookComponent;
    
    public void Start()
    {
        _freeLookComponent = GetComponent<CinemachineFreeLook>();
        isGameInFPSEvent.Register(SwitchCameraPriority);
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
            lookMovement.x = lookMovement.x * 180f; 

            //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
            _freeLookComponent.m_XAxis.Value += lookMovement.x * lookSpeed * Time.deltaTime;
            _freeLookComponent.m_YAxis.Value += lookMovement.y * lookSpeed * Time.deltaTime;
        }
    }

    public void SwitchCameraPriority(bool b)
    {
        _freeLookComponent.Priority = b ? 0 : 1;
    }
}