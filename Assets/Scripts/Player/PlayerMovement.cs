using System;
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
        //Atom's variable
        public FloatVariable Speed;
        public FloatVariable BreakForce;
        public BoolVariable IsAutoMode;
        public BoolVariable IsSemiAutoMode;
        public BoolVariable IsManualMode;
        public IntVariable WaypointIndex;
        public GameObjectVariable CurrentPlayerGameObject;

        public GameObject PauseMenuGameObject;
        public CinemachineDollyCart DollyCartInfo;
        private float _currentSpeed;
        private CinemachineSmoothPath _playerPath;
        #endregion

        #region Events
        private void Update()
        {
            //If the player reach the end show UI
            if (CheckTheEnd())
            {
                //The player won the game
                GameManager.instance.EndGame(false);
            }

            //Move the player depending the mode
            MovePlayer();
        }
        #endregion

        #region Methods
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

            var waypoints = DollyCartInfo.m_Path.GetComponent<CinemachineSmoothPath>().m_Waypoints;
            if(WaypointIndex.Value + 1 < waypoints.Length && Vector3.Distance(this.transform.position, waypoints[WaypointIndex.Value + 1].position) <= 0.5f)
            {
                WaypointIndex.SetValue(WaypointIndex.Value + 1);
            }
        }
        public void SetPath(CinemachineSmoothPath smoothPath)
        {
            _playerPath = smoothPath;
            if (DollyCartInfo)
            {
                DollyCartInfo.m_Path = _playerPath;
                DollyCartInfo.m_Position = 0.0f;
                //transform.position = _dollyCartInfo.transform.position;
                WaypointIndex.SetValue(0);
                
            }
        }
        #endregion
    }
}