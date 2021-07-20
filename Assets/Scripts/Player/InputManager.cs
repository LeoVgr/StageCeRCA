using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class InputManager : MonoBehaviour
    {
        #region "Attributs"
        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InputManager>();
                }

                return _instance;
            }
        }
        public PlayerInputSettings Settings;

        internal bool isBreaking = false;
        internal bool isFiring = false;

        internal Vector2 lookRotation = Vector2.zero;
        private Vector2 _lookJoystickValue = Vector2.zero;
        #endregion"

        #region "Events"
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;

            Settings.Break.started += context => isBreaking = true;
            Settings.Break.canceled += context => isBreaking = false;
            Settings.Break.Enable();

            Settings.Fire.performed += context => isFiring = true;
            Settings.Fire.canceled += context => isFiring = false;
            Settings.Fire.Enable();
            
            Settings.MovingForward.Enable();
            Settings.MovingBackward.Enable();
            Settings.Escape.Enable();

            #region Gamepad

            Settings.LookJoystick.performed += ctx => OnLookJoystick(ctx.ReadValue<Vector2>());
            Settings.LookJoystick.canceled += ctx => OnLookJoystick(Vector2.zero);

            Settings.LookJoystick.Enable();

            #endregion

            #region Mouse & Keyboard

            Settings.LookPointer.performed += ctx => OnLookMouse(ctx.ReadValue<Vector2>());
            Settings.LookPointer.Enable();

            #endregion
        }
        private void Update()
        {
            lookRotation += _lookJoystickValue * Settings.JoystickPower * Time.deltaTime;          
        }
        #endregion

        #region "Methods"
        private void OnLookJoystick(Vector2 value)
        {
            _lookJoystickValue = value;
        }
        private void OnLookMouse(Vector2 value)
        {
            lookRotation += value;
        }
        #endregion"
    }
}