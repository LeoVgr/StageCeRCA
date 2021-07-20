using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        #region "Attributs"
        public BoolVariable isPlayerFPS;
        public BoolVariable isPlayerLock;

        public GameObject FpsCamera;
        public GameObject TpsCamera;

        public float MaxRotationX = 90;
        public float MinRotationX = -90;
        public float MaxRotationY = 90;
        public float MinRotationY = -90;

        private float _xAngle = 0;
        private float _yAngle = 0;

        private CinemachineDollyCart _dolly;
        #endregion

        #region "Events"
        private void Start()
        {
            //Get references
            _dolly = GetComponentInParent<CinemachineDollyCart>();
        }
        void Update()
        {
            //Don't move anything if the player is lock
            if (isPlayerLock.Value) return;

            //Update rotation of the camera
            UpdateCameraRotation();
        }
        #endregion

        #region "Methods"
        private void UpdateCameraRotation()
        {
            //Input
            InputManager inputManager = Player.InputManager.Instance;
            Vector3 input = inputManager.lookRotation;

            //Reset the value to indicate this is used
            inputManager.lookRotation = Vector2.zero;

            //move mouse on Y rotate around X axis
            _xAngle += input.y * (inputManager.Settings.InverseY ? 1 : -1);
            if (_xAngle > MaxRotationX)
                _xAngle = MaxRotationX;
            else if (_xAngle < MinRotationX)
                _xAngle = MinRotationX;
            //move mouse on X rotate around Y axis
            _yAngle += input.x;
            if (_yAngle > MaxRotationY)
                _yAngle = MaxRotationY;
            else if (_yAngle < MinRotationY)
                _yAngle = MinRotationY;

            //Get forward direction with dolly reference
            if (!_dolly.m_Path)
            {
                //TODO _dolly.m_Path is null when isPlayerLock is false : player can move before the maze finished to be generated
                Debug.LogWarning("Maze not yet generated");
                return;
            }

            var forward = _dolly.m_Path.EvaluateOrientationAtUnit(_dolly.m_Position, _dolly.m_PositionUnits).eulerAngles;

            var newRotation = new Vector3(forward.x + _xAngle, forward.y + _yAngle, 0);

            Quaternion look = Quaternion.Euler(newRotation);

            transform.rotation = look;
        }
        #endregion
    }
}