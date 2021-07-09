using System;
using Cinemachine;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityAtoms.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * @author : Samuel BUSSON
 * @brief : PlayerMovement allows the player to move along the rail generaty by MazeGenerator
 * @date : 07/2020
 */
public class PlayerMovement : MonoBehaviour
{
    //TODO add to settings
    [Range(0, 10)] [SerializeField] private float speed = 5.0f;

    public IntVariable waypointIndex;
    public BoolEvent isGameStart;
    public BoolVariable a_isPlayerLock;
    public GameObjectVariable currentPlayerGameObject;

    public GameObject pauseMenu;


    [Header("Restart var")] public IntVariable targetCount;
    public IntVariable targetHit;
    public GameObjectValueList _targetList;

    private Animator _animator;

    private PlayerSaveData _playerSaveData;
    private InputActionMap _inputActions;
    private PlayerInput _input;
    private CinemachineDollyCart _dollyCartInfo;

    private float _currentPosition;
    private float _currentSpeed;
    private CinemachineSmoothPath _playerPath;

    private float _waypointsDelta;

    private bool _isMenuOn;

    private VictoryScreen _victoryScreenScript;

    private float _animatorSpeed;
    private float _currentPositionTemp;
    private static readonly int DirectionX = Animator.StringToHash("DirectionX");
    private static readonly int DirectionZ = Animator.StringToHash("DirectionZ");

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInput>();
        _playerSaveData = GetComponent<PlayerSaveData>();
        _inputActions = _input.actions.actionMaps[0];
        isGameStart.Register(IsGameStart);
    }

    private void OnDestroy()
    {
        isGameStart.UnregisterAll();
        RemoveListener();
        Cursor.visible = true;
    }

    private void IsGameStart(bool b)
    {
        if (b)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            ListenKeyBoard();
        }
        else
        {
            RemoveListener();
        }
    }

    private void ListenKeyBoard()
    {
        _inputActions["MovingForward"].performed += MoveForward;
        _inputActions["MovingForward"].canceled += StopMovement;

        _inputActions["MovingBackward"].performed += MoveBackward;
        _inputActions["MovingBackward"].canceled += StopMovement;

        _inputActions["Escape"].performed += PauseMenu;
    }

    private void RemoveListener()
    {
        _inputActions["MovingForward"].canceled -= StopMovement;
        _inputActions["MovingForward"].performed -= MoveForward;

        _inputActions["MovingBackward"].performed -= MoveBackward;
        _inputActions["MovingBackward"].canceled -= StopMovement;

        _inputActions["Escape"].performed -= PauseMenu;
    }

    private void PauseMenu(InputAction.CallbackContext obj)
    {
        if (currentPlayerGameObject.Value == gameObject)
            ShowMenu();
    }

    public void ShowMenu()
    {
        _isMenuOn = !_isMenuOn;

        Time.timeScale = _isMenuOn ? 0.0f : 1.0f;

        Cursor.lockState = _isMenuOn ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isMenuOn;

        pauseMenu.SetActive(!pauseMenu.activeSelf);

        Canvas c = pauseMenu.GetComponentInParent<Canvas>();

        if (c != null)
        {
            c.sortingOrder = _isMenuOn ? 100 : 0;
        }

        if (_victoryScreenScript == null)
            _victoryScreenScript = FindObjectOfType<VictoryScreen>();

        _victoryScreenScript.gameObject.SetActive(!pauseMenu.activeSelf);

        if (pauseMenu.GetComponentInChildren<SetTextMeshProPlayer>())
            pauseMenu.GetComponentInChildren<SetTextMeshProPlayer>().SetAllText();

        a_isPlayerLock.SetValue(pauseMenu.activeSelf);
    }

    private void FixedUpdate()
    {
        if (!a_isPlayerLock.Value)
        {
            if (CheckTheEnd())
            {
                DisplayScreen(false);
            }

            MovePlayer();
        }
    }

    public void DisplayScreen(bool isLosse)
    {
        a_isPlayerLock.SetValue(true);

        if (_victoryScreenScript == null)
            _victoryScreenScript = FindObjectOfType<VictoryScreen>();
        _victoryScreenScript.ShowScreen();
        _playerSaveData.EndGame();
    }

    private void MovePlayer()
    {
        if (_dollyCartInfo)
        {
            // float cameraForward = Vector3.Dot(_dollyCartInfo.transform.forward, transform.forward);
            //float angle = Vector3.Angle(_dollyCartInfo.transform.forward, transform.forward);

            if (Mathf.Abs(_currentSpeed) > 0.2f)
            {
                _dollyCartInfo.m_Position += _currentSpeed * Time.fixedDeltaTime;
                Vector3 animatorValue = Vector3.forward * _animatorSpeed;

                _animator.SetFloat(DirectionX, animatorValue.x);
                _animator.SetFloat(DirectionZ, animatorValue.z);

                SetWayPointIndex();

                Vector3 deltaPosition = _dollyCartInfo.transform.position - transform.position;
                transform.position += deltaPosition;
            }
            else
            {
                _animator.SetFloat(DirectionX, 0);
                _animator.SetFloat(DirectionZ, 0);
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
        if (_dollyCartInfo.m_Position >= _currentPositionTemp + _waypointsDelta)
        {
            waypointIndex.SetValue(waypointIndex.Value + 1);
            _currentPositionTemp += _waypointsDelta;
        }

        if (_dollyCartInfo.m_Position <= _currentPositionTemp - _waypointsDelta)
        {
            waypointIndex.SetValue(waypointIndex.Value - 1);
            _currentPositionTemp -= _waypointsDelta;
        }
    }

    private void StopMovement(InputAction.CallbackContext callbackContext)
    {
        _currentSpeed = 0.0f;
        DOVirtual.Float(1.0f, .0f, 0.3f, SetSpeed);
    }

    public void MoveForward(InputAction.CallbackContext callbackContext)
    {
        float ratio = callbackContext.ReadValue<float>();
        _currentSpeed = speed * ratio;
        DOVirtual.Float(0, ratio, 0.3f, SetSpeed);
    }

    public void MoveBackward(InputAction.CallbackContext callbackContext)
    {
        float ratio = callbackContext.ReadValue<float>();
        _currentSpeed = -speed * ratio;
        DOVirtual.Float(0, ratio, 0.3f, SetSpeed);
    }

    private void SetSpeed(float f)
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
        _currentPositionTemp = 0.0f;
        _currentSpeed = 0.0f;
    }

    public void Restart()
    {
        _dollyCartInfo.m_Position = 0.0f;
        transform.position = _dollyCartInfo.transform.position;
        _waypointsDelta = 1.0f;
        _currentPositionTemp = 0.0f;
        waypointIndex.SetValue(0);
        targetCount.SetValue(0);
        targetHit.SetValue(0);
        _targetList.Clear();
    }
}