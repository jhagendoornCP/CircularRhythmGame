using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmPlayer
{
    public class PlayerInputWrapper : MonoBehaviour
    {
        public InputAction W, A, S, D, LC, RC; // scheme 1
        public InputAction LClick, RClick, MousePos;
        public InputAction Space, Shift;
        private FrameActions actions;
        void Awake()
        {
            actions = new();
            W = actions.ControlScheme1WASD.W;
            A = actions.ControlScheme1WASD.A;
            S = actions.ControlScheme1WASD.S;
            D = actions.ControlScheme1WASD.D;
            LC = actions.ControlScheme1WASD.LClick;
            RC = actions.ControlScheme1WASD.RClick;

            LClick = actions.ControlScheme2Clicking.LClick;
            RClick = actions.ControlScheme2Clicking.RClick;
            MousePos = actions.ControlScheme2Clicking.MousePos;

            Space = actions.Player.Jump;
            Shift = actions.Player.Sprint;
        }

        void OnEnable()
        {
            actions.Enable();
        }

        void OnDisable()
        {
            actions.Disable();
        }
    }
}