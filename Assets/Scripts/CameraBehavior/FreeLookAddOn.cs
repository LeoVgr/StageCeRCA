using Cinemachine;
using UnityEngine;

namespace CameraBehavior
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class FreeLookAddOn : CameraControl
    {
        private CinemachineFreeLook _freeLookComponent;

        protected override void Initialize()
        {
            _freeLookComponent = GetComponent<CinemachineFreeLook>();
        }

        protected override void RotateLook(Vector2 lookMovement)
        {
            // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
            lookMovement.x = lookMovement.x * 180f;
            //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
            _freeLookComponent.m_XAxis.Value += lookMovement.x * Time.deltaTime;
            _freeLookComponent.m_YAxis.Value += lookMovement.y * Time.deltaTime;
        }

        public override void SwitchCameraPriority(bool isPriority)
        {
            _freeLookComponent.Priority = isPriority ? 0 : 1;
        }
    }
}