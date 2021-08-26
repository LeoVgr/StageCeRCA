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

        public GameObject PauseMenuGameObject;
        public CinemachineDollyCart DollyCartInfo;
        private float _currentSpeed;
        private CinemachineSmoothPath _playerPath;
        public AudioSource BreakingSound;
        public AudioSource DrivingSound;

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
                //Fix the cart when the game is not running
                if (!(GameManager.instance.GetGameStatement() == GameManager.GameStatement.Running))
                {
                    DollyCartInfo.m_Speed = 0;
                    return;
                }


                //Check if the player is breaking (semi auto mode)
                if (InputManager.instance.IsBreakAction() && DataManager.instance.IsSemiAutoMode.Value)
                {
                    DollyCartInfo.m_Speed = Mathf.Max(0,
                        DollyCartInfo.m_Speed - DataManager.instance.BreakForce.Value * Time.deltaTime);

                    //Sound of breaking
                    if (!BreakingSound.isPlaying)
                        BreakingSound.Play();
                }
                else
                {
                    float ratioSpeed;
                    //Check if we're in manual mode
                    if (DataManager.instance.IsManualMode.Value)
                    {
                        ratioSpeed = InputManager.instance.GetInputMovementVector().y;
                        _currentSpeed = DataManager.instance.Speed.Value * ratioSpeed;
                        DollyCartInfo.m_Speed = _currentSpeed;
                    }
                    else
                    {
                        //We assume here that the break force is also the start force
                        DollyCartInfo.m_Speed = Mathf.Min(DataManager.instance.Speed.Value,
                            DollyCartInfo.m_Speed + DataManager.instance.BreakForce.Value * Time.deltaTime);
                        ratioSpeed = DollyCartInfo.m_Speed / DataManager.instance.Speed.Value;
                    }

                    DrivingSound.volume = ratioSpeed;
                    //Sound of driving
                    if (ratioSpeed > float.Epsilon)
                    {
                        if (!DrivingSound.isPlaying)
                        {
                            DrivingSound.Play();
                        }
                    }
                    else if (DrivingSound.isPlaying)
                        DrivingSound.Stop();
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
            if (DataManager.instance.WaypointIndex.Value + 1 < waypoints.Length &&
                Vector3.Distance(this.transform.position,
                    waypoints[DataManager.instance.WaypointIndex.Value + 1].position) <= 0.5f)
            {
                DataManager.instance.WaypointIndex.SetValue(DataManager.instance.WaypointIndex.Value + 1);
            }
        }

        public void SetPath(CinemachineSmoothPath smoothPath)
        {
            _playerPath = smoothPath;
            if (DollyCartInfo)
            {
                DollyCartInfo.m_Path = _playerPath;
                DollyCartInfo.m_Position = 0.0f;
                DataManager.instance.WaypointIndex.SetValue(0);
            }
        }

        #endregion
    }
}