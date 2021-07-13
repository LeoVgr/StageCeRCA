using System;
using Cinemachine;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * @author : Samuel BUSSON
 * @brief : PlayerMovement allows the player to move along the rail generaty by MazeGenerator
 * @date : 07/2020
 */
public class PlayerMovement : MonoBehaviour
{
    #region "Attributs"
    public FloatVariable Speed;
    public BoolVariable IsAutoMode;
    public BoolVariable IsSemiAutoMode;
    public BoolVariable IsManualMode;
    public IntVariable WaypointIndex;
    public BoolEvent IsGameStartEvent;
    public BoolVariable IsPlayerLock;
    public GameObjectVariable CurrentPlayerGameObject;
    public GameObject PauseMenuGameObject;


    [Header("Restart var")] 
    public IntVariable TargetCount;
    public IntVariable TargetHit;
    public GameObjectValueList TargetList;

    private Animator _animator;
    private PlayerSaveData _playerSaveData;
    private InputActionMap _inputActions;
    private PlayerInput _input;
    private CinemachineDollyCart _dollyCartInfo;
    private bool _isBreaking = false;
    private float _currentSpeed;
    private CinemachineSmoothPath _playerPath;
    private float _waypointsDelta;
    private bool _isMenuOn;
    private VictoryScreen _victoryScreenScript;
    private float _animatorSpeed;
    private float _currentPosition;
    private static readonly int _directionX = Animator.StringToHash("DirectionX");
    private static readonly int _directionZ = Animator.StringToHash("DirectionZ");
    #endregion

    #region "Events"
    void Start()
    {
        //Get references
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInput>();
        _playerSaveData = GetComponent<PlayerSaveData>();
        _inputActions = _input.actions.actionMaps[0];
        IsGameStartEvent.Register(IsGameStart);
    }
    private void OnDestroy()
    {
        //Unregister events
        IsGameStartEvent.UnregisterAll();
        RemoveListener();

        //Set the cursor up
        Cursor.visible = true;
    }
    private void Update()
    {
        //Move the player if he is not locked
        if (!IsPlayerLock.Value)
        {
            //If the player reach the end show UI
            if (CheckTheEnd())
            {
                DisplayScreen(false);
            }

            //Move the player depending the mode
            MovePlayer();
        }
    }
    private void StopMovement(InputAction.CallbackContext callbackContext)
    {
        _currentSpeed = 0.0f;
        DOVirtual.Float(1.0f, .0f, 0.3f, SetAnimatorSpeed);
    }
    private void Break(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
            _isBreaking = true;

        if (callbackContext.canceled)
            _isBreaking = false;
    }
    public void MoveForward(InputAction.CallbackContext callbackContext)
    {
        if (IsManualMode)
        {
            float ratio = callbackContext.ReadValue<float>();
            _currentSpeed = Speed.Value * ratio;
            DOVirtual.Float(0, ratio, 0.3f, SetAnimatorSpeed);
        }      
    }
    public void MoveBackward(InputAction.CallbackContext callbackContext)
    {
        if (IsManualMode)
        {
            float ratio = callbackContext.ReadValue<float>();
            _currentSpeed = -Speed.Value * ratio;
            DOVirtual.Float(0, ratio, 0.3f, SetAnimatorSpeed);
        }      
    }
    private void PauseMenu(InputAction.CallbackContext obj)
    {
        if (CurrentPlayerGameObject.Value == gameObject)
            ShowMenu();
    }
    #endregion

    #region "Methods"
    private void AddListeners()
    {
        _inputActions["MovingForward"].performed += MoveForward;
        _inputActions["MovingForward"].canceled += StopMovement;

        _inputActions["MovingBackward"].performed += MoveBackward;
        _inputActions["MovingBackward"].canceled += StopMovement;

        _inputActions["Break"].performed += Break;
        _inputActions["Break"].canceled += Break;

        _inputActions["Escape"].performed += PauseMenu;
    }
    private void RemoveListener()
    {
        _inputActions["MovingForward"].canceled -= StopMovement;
        _inputActions["MovingForward"].performed -= MoveForward;

        _inputActions["MovingBackward"].performed -= MoveBackward;
        _inputActions["MovingBackward"].canceled -= StopMovement;

        _inputActions["Break"].performed -= Break;
        _inputActions["Break"].canceled -= Break;

        _inputActions["Escape"].performed -= PauseMenu;
    }
    private void IsGameStart(bool b)
    {
        if (b)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            AddListeners();
        }
        else
        {
            RemoveListener();
        }
    }   
    public void ShowMenu()
    {
        _isMenuOn = !_isMenuOn;

        Time.timeScale = _isMenuOn ? 0.0f : 1.0f;

        Cursor.lockState = _isMenuOn ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isMenuOn;

        PauseMenuGameObject.SetActive(!PauseMenuGameObject.activeSelf);

        Canvas c = PauseMenuGameObject.GetComponentInParent<Canvas>();

        if (c != null)
        {
            c.sortingOrder = _isMenuOn ? 100 : 0;
        }

        if (_victoryScreenScript == null)
            _victoryScreenScript = FindObjectOfType<VictoryScreen>();

        _victoryScreenScript.gameObject.SetActive(!PauseMenuGameObject.activeSelf);

        if (PauseMenuGameObject.GetComponentInChildren<SetTextMeshProPlayer>())
            PauseMenuGameObject.GetComponentInChildren<SetTextMeshProPlayer>().SetAllText();

        IsPlayerLock.SetValue(PauseMenuGameObject.activeSelf);
    }
    public void DisplayScreen(bool isLosse)
    {
        IsPlayerLock.SetValue(true);

        if (_victoryScreenScript == null)
            _victoryScreenScript = FindObjectOfType<VictoryScreen>();
        _victoryScreenScript.ShowScreen(isLosse);
        _playerSaveData.EndGame();
    }
    private void MovePlayer()
    {
        if (_dollyCartInfo)
        {
            //Check if the player is breaking (semi auto mode)
            if (_isBreaking)
            {
                _dollyCartInfo.m_Speed = 0;
            }
            else
            {
                //Check if we're in manual mode
                if (IsManualMode.Value)
                {
                    _dollyCartInfo.m_Speed = _currentSpeed;
                }
                else
                {
                    _dollyCartInfo.m_Speed = Speed.Value;
                }
                
            }

            //Update the position of the player on the track
            SetWayPointIndex();

            //Check if we got any speed
            if (Mathf.Abs(_currentSpeed) > 0.2f)
            {
                //Play animation with the right speed
                Vector3 animatorValue = Vector3.forward * _animatorSpeed;
                _animator.SetFloat(_directionX, animatorValue.x);
                _animator.SetFloat(_directionZ, animatorValue.z);
            }
            else
            {
                //Stop animation
                _animator.SetFloat(_directionX, 0);
                _animator.SetFloat(_directionZ, 0);
            }
        }
    }
    private bool CheckTheEnd()
    {
        if (_dollyCartInfo)
            return Math.Abs(_dollyCartInfo.m_Position - _dollyCartInfo.m_Path.MaxPos) < 0.5f;
        return false;
    }
    private void SetWayPointIndex()
    {
        if (_dollyCartInfo.m_Position >= _currentPosition + _waypointsDelta)
        {
            WaypointIndex.SetValue(WaypointIndex.Value + 1);
            _currentPosition += _waypointsDelta;
        }

        if (_dollyCartInfo.m_Position <= _currentPosition - _waypointsDelta)
        {
            WaypointIndex.SetValue(WaypointIndex.Value - 1);
            _currentPosition -= _waypointsDelta;
        }
    }
    private void SetAnimatorSpeed(float f)
    {
        _animatorSpeed = f;
    }
    public void SetPath(CinemachineSmoothPath smoothPath)
    {
        _playerPath = smoothPath;
        if (!_dollyCartInfo)
        {
            GameObject dollyCartChild = new GameObject();
            dollyCartChild.AddComponent<CinemachineDollyCart>();
            dollyCartChild.name = "Dollycart";
            _dollyCartInfo = dollyCartChild.GetComponent<CinemachineDollyCart>();
            _dollyCartInfo.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
        }

        _dollyCartInfo.m_Path = _playerPath;
        _dollyCartInfo.m_Position = 0.0f;
        transform.position = _dollyCartInfo.transform.position;
        _waypointsDelta = 1.0f;
        _currentPosition = 0.0f;
        _currentSpeed = 0.0f;
        
        //set the gameobject player child of the cart
        this.gameObject.transform.parent = _dollyCartInfo.transform;
    }
    public void Restart()
    {
        _dollyCartInfo.m_Position = 0.0f;
        transform.position = _dollyCartInfo.transform.position;
        _waypointsDelta = 1.0f;
        _currentPosition = 0.0f;
        WaypointIndex.SetValue(0);
        TargetCount.SetValue(0);
        TargetHit.SetValue(0);
        TargetList.Clear();
    }
    #endregion
}