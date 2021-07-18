using Cinemachine;
using Player;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace CameraBehavior
{
    public class CameraControl : MonoBehaviour
    {
        public BoolEvent isGameInFPSEvent;
        public bool IsFPSCamera;
        private CinemachineVirtualCamera _camera;

        public void Start()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
            isGameInFPSEvent.Register(SwitchCameraPriority);
        }

        private void SwitchCameraPriority(bool isFPS)
        {
            _camera.Priority = (isFPS == IsFPSCamera) ? 0 : 1;
        }
    }
}