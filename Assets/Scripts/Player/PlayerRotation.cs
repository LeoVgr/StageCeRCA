using Cinemachine;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Player
{
    public class PlayerRotation : MonoBehaviour
    {
        public BoolVariable isPlayerFPS;
        public BoolVariable isPlayerLock;

        public float MaxRotation = 90;
        public float MinRotation = -90;

        private float _xAngle = 0;
        private float _yAngle = 0;

        private CinemachineDollyCart _dolly;

        private void Start()
        {
            _dolly = GetComponent<CinemachineDollyCart>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlayerLock.Value) return;

            //Input
            InputManager inputManager = Player.InputManager.Instance;
            Vector3 input = inputManager.lookRotation;
            //Reset the value to indicate this is used
            inputManager.lookRotation = Vector2.zero;

            //move mouse on Y rotate around X axis
            _xAngle += input.y * (inputManager.Settings.InverseY ? 1 : -1);
            if (_xAngle > MaxRotation)
                _xAngle = MaxRotation;
            else if (_xAngle < MinRotation)
                _xAngle = MinRotation;
            //move mouse on X rotate around Y axis
            _yAngle += input.x;
            if (_yAngle > MaxRotation)
                _yAngle = MaxRotation;
            else if (_yAngle < MinRotation)
                _yAngle = MinRotation;

            //Get forward direction with dolly reference
            if (!_dolly.m_Path)
            {
                //TODO _dolly.m_Path is null when isPlayerLock is false : player can move before the maze finished to be generated
                Debug.LogWarning("Maze not yet generated");
                return;
            }

            var forward = _dolly.m_Path.EvaluateOrientationAtUnit(_dolly.m_Position,_dolly.m_PositionUnits).eulerAngles;

            var newRotation = new Vector3(forward.x + _xAngle, forward.y + _yAngle, 0);

            Quaternion look = Quaternion.Euler(newRotation);

            transform.rotation = look;
        }
    }
}