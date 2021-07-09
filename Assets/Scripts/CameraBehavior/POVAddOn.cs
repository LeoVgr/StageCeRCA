using Cinemachine;
using UnityEngine;

namespace CameraBehavior
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class POVAddOn : CameraControl
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachinePOV _pov;

        protected override void Initialize()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        }

        protected override void RotateLook(Vector2 lookMovement)
        {
            // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
            lookMovement.x *= 180f;
            lookMovement.y *= 180f;

            //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
            _pov.m_HorizontalAxis.Value += lookMovement.x * Time.deltaTime;
            _pov.m_VerticalAxis.Value += lookMovement.y * Time.deltaTime;
        }

        public override void SwitchCameraPriority(bool isPriority)
        {
            throw new System.NotImplementedException();
        }
    }
}