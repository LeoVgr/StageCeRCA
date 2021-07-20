﻿using System;
using Cinemachine;
using Data;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /**
 * @author : Samuel BUSSON
 * @brief : PlayerMovement allows the player to move along the rail generaty by MazeGenerator
 * @date : 07/2020
 */
    public class PlayerMovement : MonoBehaviour
    {
        #region Attributs
        public FloatVariable Speed;
        public FloatVariable BreakForce;
        public BoolVariable IsAutoMode;
        public BoolVariable IsSemiAutoMode;
        public BoolVariable IsManualMode;
        public IntVariable WaypointIndex;
        public BoolEvent IsGameStartEvent;
        public BoolVariable IsPlayerLock;
        public GameObjectVariable CurrentPlayerGameObject;
        public GameObject PauseMenuGameObject;
        public CinemachineDollyCart DollyCartInfo;
        public Animator[] Animators = new Animator[3];
        private int _modelIndex = -1;

        [Header("Restart var")] 
        public IntVariable TargetCount;
        public IntVariable TargetHit;
        public GameObjectValueList TargetList;


        private PlayerSaveData _playerSaveData;
        private float _currentSpeed;
        private CinemachineSmoothPath _playerPath;
        private float _waypointsDelta;
        private bool _isMenuOn;
        private VictoryScreen _victoryScreenScript;
        private float _animatorSpeed;
        private static readonly int _directionX = Animator.StringToHash("DirectionX");
        private static readonly int _directionZ = Animator.StringToHash("DirectionZ");
        #endregion

        #region Events
        void Start()
        {
            //Get references
            _playerSaveData = transform.parent.GetComponent<PlayerSaveData>();
            _modelIndex = _playerSaveData._modelIndex;
            IsGameStartEvent.Register(IsGameStart);
        }
        private void OnDestroy()
        {
            //Unregister events
            IsGameStartEvent.UnregisterAll();

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
        //private void StopMovement(InputAction.CallbackContext callbackContext)
        //{
        //    _currentSpeed = 0.0f;
        //    DOVirtual.Float(1.0f, .0f, 0.3f, SetAnimatorSpeed);
        //}
        //private void Break(InputAction.CallbackContext callbackContext)
        //{
        //    if (callbackContext.performed)
        //    {
        //        _isPlayerBreaking = true;
        //    }


        //    if (callbackContext.canceled)
        //    {
        //        _isPlayerBreaking = false;
        //    }
        //}
        //public void MoveForward(InputAction.CallbackContext callbackContext)
        //{
        //    if (IsManualMode)
        //    {
        //        float ratio = callbackContext.ReadValue<float>();
        //        _currentSpeed = Speed.Value * ratio;
        //        DOVirtual.Float(0, ratio, 0.3f, SetAnimatorSpeed);
        //    }
        //}
        //public void MoveBackward(InputAction.CallbackContext callbackContext)
        //{
        //    if (IsManualMode)
        //    {
        //        float ratio = callbackContext.ReadValue<float>();
        //        _currentSpeed = -Speed.Value * ratio;
        //        DOVirtual.Float(0, ratio, 0.3f, SetAnimatorSpeed);
        //    }
        //}
        //private void PauseMenu(InputAction.CallbackContext obj)
        //{
        //    if (CurrentPlayerGameObject.Value == gameObject)
        //        ShowMenu();
        //}
        #endregion

        #region Methods
        private void IsGameStart(bool b)
        {
            //TODO RENAME THIS PARAMETER
            if (b)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                //AddListeners();
            }
            else
            {
                //RemoveListener();
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
            if (DollyCartInfo)
            {
                //Check if the player is breaking (semi auto mode)
                if (InputManager.instance.IsBreakAction() && IsSemiAutoMode.Value)
                {
                    DollyCartInfo.m_Speed = Mathf.Max(0, DollyCartInfo.m_Speed - BreakForce.Value * Time.deltaTime);
                }
                else
                {
                    //Check if we're in manual mode
                    if (IsManualMode.Value)
                    {
                        _currentSpeed = Speed.Value * InputManager.instance.GetInputMovementVector().y;
                        DollyCartInfo.m_Speed = _currentSpeed;
                    }
                    else
                    {
                        //We assume here that the break force is also the start force
                        DollyCartInfo.m_Speed = Mathf.Min(Speed.Value,
                            DollyCartInfo.m_Speed + BreakForce.Value * Time.deltaTime);
                    }
                }

                //Update the position of the player on the track
                SetWayPointIndex();

                if (!_playerSaveData.IsFPSBool)
                {
                    //Check if we got any speed
                    if (Mathf.Abs(_currentSpeed) > 0.2f)
                    {
                        //Play animation with the right speed
                        Vector3 animatorValue = Vector3.forward * _animatorSpeed;
                        Animators[_modelIndex].SetFloat(_directionX, animatorValue.x);
                        Animators[_modelIndex].SetFloat(_directionZ, animatorValue.z);
                    }
                    else
                    {
                        //Stop animation
                        Animators[_modelIndex].SetFloat(_directionX, 0);
                        Animators[_modelIndex].SetFloat(_directionZ, 0);
                    }
                }
            }
        }
        private bool CheckTheEnd()
        {
            if (DollyCartInfo && DollyCartInfo.m_Path)
                return Math.Abs(DollyCartInfo.m_Position - DollyCartInfo.m_Path.PathLength) < 0.5f;
            return false;
        }
        private void SetWayPointIndex()
        {
            if (!DollyCartInfo.m_Path)
                return;

            if(Vector3.Distance(this.transform.position, DollyCartInfo.m_Path.GetComponent<CinemachineSmoothPath>().m_Waypoints[WaypointIndex.Value + 1].position) <= 0.5f)
            {
                WaypointIndex.SetValue(WaypointIndex.Value + 1);
            }
        }
        private void SetAnimatorSpeed(float f)
        {
            _animatorSpeed = f;
        }
        public void SetPath(CinemachineSmoothPath smoothPath)
        {
            _playerPath = smoothPath;
            if (DollyCartInfo)
            {
                DollyCartInfo.m_Path = _playerPath;
                DollyCartInfo.m_Position = 0.0f;
                //transform.position = _dollyCartInfo.transform.position;
                _waypointsDelta = 1.0f;
                WaypointIndex.SetValue(0);
                TargetCount.SetValue(0);
                TargetHit.SetValue(0);
                TargetList.Clear();
            }
        }
        #endregion
    }
}