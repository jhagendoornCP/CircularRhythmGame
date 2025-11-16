using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace RhythmPlayer
{
    // this is the WASD controller scheme
    public class PlayerControllerScheme1 : PlayerControllerBase
    {
        void OnEnable()
        {
            inp.W.performed += SelectUp;
            inp.A.performed += SelectLeft;
            inp.S.performed += SelectDown;
            inp.D.performed += SelectRight;
            inp.LC.performed += ClickLane;
            inp.RC.started += StartJump;
            inp.RC.canceled += EndJump;
        }

        void OnDisable()
        {
            inp.W.performed -= SelectUp;
            inp.A.performed -= SelectLeft;
            inp.S.performed -= SelectDown;
            inp.D.performed -= SelectRight;
            inp.LC.performed -= ClickLane;
            inp.RC.started -= StartJump;
            inp.RC.canceled -= EndJump; 
        }

        private void SelectLeft(CallbackContext _)
        {
            currentSelectedLane = 2;
            ChangeFacingDirection(Vector2.left);
            NewLaneSelected();
        }
        private void SelectRight(CallbackContext _)
        {
            currentSelectedLane = 0;
            ChangeFacingDirection(Vector2.right);
            NewLaneSelected();
        }
        private void SelectDown(CallbackContext _)
        {
            currentSelectedLane = 1;
            ChangeFacingDirection(Vector2.down);
            NewLaneSelected();
        }
        private void SelectUp(CallbackContext _)
        {
            currentSelectedLane = 3;
            ChangeFacingDirection(Vector2.up);
            NewLaneSelected();
        }

        protected override void ClickLane(CallbackContext _)
        {
            someLaneClicked.Invoke(currentSelectedLane);
        }

        protected override void StartJump(CallbackContext _)
        {
            if (jumping) return;
            jumpStartedAt = Time.time;
            jumping = true;
            jumpDown = true;
            // this only works if the player is based at 0,0
            jumpingToPoint = playerCollider.transform.up * movementData.jumpOffset;

            NewLaneJumped();
        }
    }
}
