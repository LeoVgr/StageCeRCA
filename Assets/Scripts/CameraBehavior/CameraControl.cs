using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraBehavior
{
    [RequireComponent(typeof(PlayerInput))]
    public abstract class CameraControl : MonoBehaviour
    {
        public BoolEvent isGameInFPSEvent;
        public FloatVariable a_lookSpeed;
        public bool InvertY = false;
        public BoolVariable a_isPlayerLock;
        private Vector2 _lookMovement;
        private bool _isLookMoving = false;

        public void Start()
        {
            Initialize();
            isGameInFPSEvent.Register(SwitchCameraPriority);

            //Bind player inputs on methods
            var input = GetComponent<PlayerInput>();
            var inputActions = input.actions.actionMaps[0];
            inputActions["Look"].performed += OnMoveLook;
            inputActions["Look"].canceled += StopLook;
        }

        /// <summary>
        /// Event call moving camera
        /// </summary>
        /// <param name="context">Input action data</param>
        private void OnMoveLook(InputAction.CallbackContext context)
        {
            _lookMovement = context.ReadValue<Vector2>();
            _lookMovement.y = InvertY ? -_lookMovement.y : _lookMovement.y;

            //Check if is gamepad input
            if (context.control.device == Gamepad.current)
            {
                _lookMovement *= 20f;
                _isLookMoving = true;
            }
            else
            {
                UpdateLook();
                _isLookMoving = false;
                _lookMovement = Vector2.zero;
            }
        }

        /// <summary>
        /// Event call stop moving camera
        /// </summary>
        /// <param name="context">Input action data</param>
        private void StopLook(InputAction.CallbackContext context)
        {
            _isLookMoving = false;
            _lookMovement = Vector2.zero;
        }

        private void Update()
        {
            if (_isLookMoving)
                UpdateLook();
        }

        private void UpdateLook()
        {
            if (!a_isPlayerLock.Value)
            {
                float lookSpeed = a_lookSpeed.Value / 15.0f;
                RotateLook(_lookMovement * lookSpeed * Time.deltaTime);
            }
        }

        protected abstract void Initialize();

        protected abstract void RotateLook(Vector2 lookMovement);

        public abstract void SwitchCameraPriority(bool isPriority);
    }
}