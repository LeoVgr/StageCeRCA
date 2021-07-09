using Cinemachine;
using UnityEngine;

namespace CameraBehavior
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class FirstPersonAddOn : CameraControl
    {
        private CinemachinePOV _virtualCameraComponent;
        private CinemachineVirtualCamera _virtualCamera;

        protected override void Initialize()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _virtualCameraComponent = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }

        protected override void RotateLook(Vector2 lookMovement)
        {
            // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
            lookMovement.x = lookMovement.x * 180f;
            lookMovement.y = lookMovement.y * -90f;
            //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
            _virtualCameraComponent.m_HorizontalAxis.Value += lookMovement.x * Time.deltaTime;
            _virtualCameraComponent.m_VerticalAxis.Value += lookMovement.y * Time.deltaTime;
        }

        public override void SwitchCameraPriority(bool isPriority)
        {
            _virtualCamera.Priority = isPriority ? 1 : 0;
        }
    }
}