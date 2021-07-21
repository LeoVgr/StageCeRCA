using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputManager : Singleton<InputManager>
{
    #region "Attributs"
    private PlayerControls _controls;
    private float _sensibility;
    private bool _isUsingGamepad = false;
    private bool _isUsingKeyboardMouse = false;
    private bool _isInputEnabled = true;
    private bool _isMovementInputEnabled = true;

    /* Inputs */
    private Vector2 _inputMovementVector = Vector2.zero;
    private Vector2 _inputAimVector = Vector2.zero;
    private bool _isFireAction = false;
    private bool _isCancelAction = false;
    private bool _isBreakAction = false;
    #endregion"

    #region "Events"
    void Start()
    {
        //Enables controls
        _controls = new PlayerControls();
        _controls.Enable();

        //Call events on input pressing
        _controls.Gameplay.Move.performed += UpdateInputMovementVector;
        _controls.Gameplay.Move.canceled += UpdateInputMovementVector;

        _controls.Gameplay.Aim.performed += UpdateInputAimVector;
        _controls.Gameplay.Aim.canceled += UpdateInputAimVector;

        _controls.Gameplay.Break.performed += UpdateBreak;
        _controls.Gameplay.Fire.performed += UpdateFire;
        _controls.Gameplay.Cancel.performed += UpdateCancel;
        _controls.Gameplay.Break.canceled += UpdateBreak;
        _controls.Gameplay.Fire.canceled += UpdateFire;
        _controls.Gameplay.Cancel.canceled += UpdateCancel;

        //Load player's pref options
        _sensibility = PlayerPrefs.GetFloat("sensibility", 0.5f);
    }
    private void Update()
    {
        if (Gamepad.current != null)
        {
            _isUsingGamepad = true;
        }

        if (Keyboard.current != null && Mouse.current != null)
        {
            _isUsingKeyboardMouse = true;
        }
    }
    private void OnApplicationQuit()
    {
        Gamepad.current?.ResetHaptics();
    }
    #endregion

    #region "Methods"
    public void DisableInputs()
    {
        _isInputEnabled = false;
    }
    public void EnableInputs()
    {
        _isInputEnabled = true;
    }
    public bool IsInputEnabled()
    {
        return _isInputEnabled;
    }
    public void DisableMovementInputs()
    {
        _isMovementInputEnabled = false;
    }
    public void EnableMovementInputs()
    {
        _isMovementInputEnabled = true;
    }
    public bool IsInputMovementEnabled()
    {
        return _isMovementInputEnabled;
    }

    public void UpdateInputMovementVector(InputAction.CallbackContext context)
    {
        _inputMovementVector = context.ReadValue<Vector2>();
    }
    public void UpdateInputAimVector(InputAction.CallbackContext context)
    {
        _inputAimVector = context.ReadValue<Vector2>();
    }
    public void UpdateBreak(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isBreakAction = true;
        }

        if (context.canceled)
        {
            _isBreakAction = false;
        }

    }
    public void UpdateFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isFireAction = true;
        }

        if (context.canceled)
        {
            _isFireAction = false;
        }
    }
    public void UpdateCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isCancelAction = true;
        }

        if (context.canceled)
        {
            _isCancelAction = false;
        }
    }

    public Vector2 GetInputMovementVector()
    {
        if (!_isInputEnabled || !_isMovementInputEnabled)
            return Vector2.zero;

        return _inputMovementVector;
    }
    public Vector2 GetInputAimVector()
    {
        if (!_isInputEnabled)
            return Vector2.zero;

        return _inputAimVector;
    }
    public bool IsBreakAction()
    {
        if (!_isInputEnabled)
            return false;

        return _isBreakAction;
    }
    public bool IsCancelAction()
    {
        if (!_isInputEnabled)
            return false;

        return _isCancelAction;
    }
    public bool IsFireAction()
    {
        if (!_isInputEnabled)
            return false;

        return _isFireAction;
    }
    public void SetSensibility(float value)
    {
        _sensibility = value;
        PlayerPrefs.SetFloat("sensibility", _sensibility);
    }
    public float GetSensibility()
    {
        return _sensibility;
    }
    public void AddControllerVibrations(float duration, float intensity)
    {
        Gamepad.current.SetMotorSpeeds(1 - intensity, intensity);
        StartCoroutine(ResetControllerVibrations(duration));
    }
    IEnumerator ResetControllerVibrations(float duration)
    {
        if (duration > 0)
        {
            yield return new WaitForSeconds(.1f);
            duration -= 0.1f;
        }
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
    #endregion"
}
