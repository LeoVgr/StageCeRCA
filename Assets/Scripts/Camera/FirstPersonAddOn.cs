using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class FirstPersonAddOn : MonoBehaviour
{
    public BoolEvent isGameInFPSEvent;

    public bool InvertY = false;
    public BoolVariable a_isPlayerLock;
    public FloatVariable a_lookSpeed;
    
    private CinemachinePOV _virtualCameraComponent;
    private CinemachineVirtualCamera _virtualCamera;
    
    public void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _virtualCameraComponent = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
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
        
        lookMovement.y = lookMovement.y * -90f; 
        

        //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
        _virtualCameraComponent.m_HorizontalAxis.Value += lookMovement.x * lookSpeed * Time.deltaTime;
        _virtualCameraComponent.m_VerticalAxis.Value += lookMovement.y * lookSpeed * Time.deltaTime;
        
        }
    }

    public void SwitchCameraPriority(bool b)
    {
        _virtualCamera.Priority = b ? 1 : 0;
    }
}