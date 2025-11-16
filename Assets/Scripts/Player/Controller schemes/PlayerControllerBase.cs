using EventStreams;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

namespace RhythmPlayer
{
    [RequireComponent(typeof(PlayerInputWrapper))]
    public abstract class PlayerControllerBase : MonoBehaviour
    {
        [SerializeField] protected IntEventStream someLaneClicked;
        [SerializeField] protected PlayerMovementData movementData;

        public event UnityAction<int> OnNewLaneSelected;
        public event UnityAction<int> OnNewLaneJumped;

        protected int currentSelectedLane = 0;
        protected PlayerInputWrapper inp;
        protected Collider2D playerCollider;

        protected bool jumping = false;
        protected bool jumpDown = false;
        private bool holdingJump = false;
        protected float jumpStartedAt = Mathf.Infinity;
        protected float jumpOutStartedAt = Mathf.Infinity;
        protected float jumpEndsAt = Mathf.Infinity;
        protected Vector3 jumpingToPoint = Vector3.zero;
        protected Vector3 jumpOutStartPosition;

        void Awake()
        {
            inp = GetComponent<PlayerInputWrapper>();
            playerCollider = GetComponentInChildren<Collider2D>();
        }

        private float t;
        protected virtual void Update()
        {
            t = Time.time;
            if (jumping && jumpDown && t < jumpStartedAt + movementData.jumpMoveInTime) JumpIn();
            else if (jumping && t >= jumpEndsAt) ResetJump();
            else if (jumping && !jumpDown) JumpOut();
            else if (jumping) HoldJump();
        }

        #region Jumping
        protected abstract void StartJump(CallbackContext _);
        private void JumpIn()
        {
            float lerpAmt = Mathf.Clamp((t - jumpStartedAt) / movementData.jumpMoveInTime, 0, 1);
            playerCollider.transform.position = Vector3.Lerp(Vector3.zero, jumpingToPoint, lerpAmt);
        }
        private void HoldJump()
        {
            holdingJump = true;
            if (t >= jumpStartedAt + movementData.jumpMoveInTime + movementData.jumpMaxHoldTime)
            {
                jumpDown = false;
                jumpOutStartedAt = t;
                jumpEndsAt = t + movementData.jumpMoveOutTime;
                jumpOutStartPosition = jumpingToPoint;
                holdingJump = false;
            }
        }
        private void JumpOut()
        {
            float lerpAmt = Mathf.Clamp((t - jumpOutStartedAt) / (jumpEndsAt - jumpOutStartedAt), 0, 1);
            playerCollider.transform.position = Vector3.Lerp(jumpOutStartPosition, Vector3.zero, lerpAmt);
        }
        protected void EndJump(CallbackContext _)
        {
            if (!jumping || !jumpDown) return;
            jumpDown = false;
            jumpOutStartedAt = Time.time;
            if (holdingJump)
            {
                jumpEndsAt = jumpOutStartedAt + movementData.jumpMoveOutTime;
                jumpOutStartPosition = jumpingToPoint;
            }
            else
            {
                float throughJumpAmount = playerCollider.transform.position.magnitude / movementData.jumpOffset;
                jumpEndsAt = jumpOutStartedAt + Mathf.Lerp(0, movementData.jumpMoveOutTime, throughJumpAmount);
                jumpOutStartPosition = playerCollider.transform.position;
            }
        }

        private void ResetJump()
        {
            jumping = false;
            jumpDown = false;
            holdingJump = false;
            jumpOutStartedAt = Mathf.Infinity;
            jumpStartedAt = Mathf.Infinity;
            jumpEndsAt = Mathf.Infinity;
            jumpingToPoint = Vector3.zero;
            jumpOutStartPosition = Vector3.zero;
            playerCollider.transform.position = Vector3.zero;
        }
        #endregion

        protected void ChangeFacingDirection(Vector2 towards)
        {
            playerCollider.transform.rotation = Quaternion.LookRotation(Vector3.forward, towards);
        }

        protected abstract void ClickLane(CallbackContext _);

        protected void NewLaneJumped() => OnNewLaneJumped?.Invoke(currentSelectedLane);
        protected void NewLaneSelected() => OnNewLaneSelected?.Invoke(currentSelectedLane);
    }
}