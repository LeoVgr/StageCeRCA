using System;
using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        #region "Attributs"
        public BoolVariable isPlayerFPS;

        public GameObject FpsCamera;
        public GameObject TpsCamera;

        public Transform RotatorX;
        public float MaxRotationX = 90;
        public float MinRotationX = -90;
        public Transform RotatorY;
        public float MaxRotationY = 90;
        public float MinRotationY = -90;

        private Vector3 _forwardX;
        private Vector3 _forwardY;
        private float _xAngle = 0;
        private float _yAngle = 0;
        #endregion

        #region "Events"

        private void Start()
        {
            _forwardX = RotatorX.localRotation.eulerAngles;
            _forwardY = RotatorY.localRotation.eulerAngles;
        }

        void Update()
        {          
            //Update rotation of the camera
            UpdateCameraRotation();
        }
        #endregion

        #region "Methods"
        private void UpdateCameraRotation()
        {
            //Input
            InputManager inputManager = InputManager.instance;
            Vector3 input = inputManager.GetInputAimVector();

            //Reset the value to indicate this is used
            //move mouse on Y rotate around X axis
            _xAngle += input.y * -1;
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


            var newRotationX = new Vector3(_forwardX.x + _xAngle,  _forwardX.y);
            var newRotationY = new Vector3(_forwardY.x, _forwardY.y + _yAngle);

            Quaternion lookX = Quaternion.Euler(newRotationX);
            Quaternion lookY = Quaternion.Euler(newRotationY);

            RotatorX.localRotation = lookX;
            RotatorY.localRotation = lookY;
        }
        #endregion
    }
}