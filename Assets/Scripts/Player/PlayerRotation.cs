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

        private CinemachineDollyCart _dolly;

        private void Start()
        {
            _dolly = GetComponent<CinemachineDollyCart>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isPlayerLock.Value) return;


            InputManager inputManager = Player.InputManager.Instance;
            Vector3 input = inputManager.lookRotation;
            Vector3 inputCorrected = new Vector3(input.y, input.x * (inputManager.Settings.InverseY ? -1 : 1));
            Vector3 newRotation = transform.rotation.normalized.eulerAngles + inputCorrected;
            if (!_dolly.m_Path)
            {
                //TODO _dolly.m_Path is null when isPlayerLock is false : player can move before the maze finished to be generated
                Debug.LogWarning("Maze not yet generated");
                return;
            }

            var forward = _dolly.m_Path.EvaluateOrientation(_dolly.m_Position).normalized.eulerAngles;
            var xDelta = Quaternion.Angle(Quaternion.Euler(newRotation.x, 0, 0).normalized,
                Quaternion.Euler(forward.x, 0, 0).normalized);
            var yDelta = Quaternion.Angle(Quaternion.Euler(0, newRotation.y, 0).normalized,
                Quaternion.Euler(0, forward.y, 0).normalized);

            //TODO Block the teleportation to Min at Max or to Max at Min
            if (xDelta > MaxRotation)
            {
                if (forward.x > newRotation.x)
                    newRotation.x = forward.x + MinRotation;
                else
                    newRotation.x = forward.x + MaxRotation;
            }

            if (yDelta > MaxRotation)
            {
                if (forward.y > newRotation.y)
                    newRotation.y = forward.y + MinRotation;
                else
                    newRotation.y = forward.y + MaxRotation;
            }

            Quaternion look = Quaternion.Euler(newRotation).normalized;

            transform.rotation = look;
        }
    }
}