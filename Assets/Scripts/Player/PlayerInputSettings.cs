using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerInputSettings", menuName = "Input/PlayerInputSettings", order = 1)]
    public class PlayerInputSettings : ScriptableObject
    {
        public InputAction LookJoystick;
        public InputAction LookPointer;
        public InputAction MovingForward;
        public InputAction MovingBackward;
        public InputAction Break;
        public InputAction Fire;
        public InputAction Escape;
        public float JoystickPower = 300;
        public bool InverseY = false;
    }
}
