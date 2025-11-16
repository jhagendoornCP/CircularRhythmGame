using System;
using System.Collections.Generic;
using EventStreams;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmPlayer
{
    // this is the weird input class that is keyboard only
    public class PlayerControllerScheme4 : PlayerControllerBase
    {
        [Header("Streams")]
        [SerializeField] private FloatEventStream lane0AngleStream;
        [SerializeField] private IntEventStream numberOfLanesStream;
        [Header("extra movement info")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float burstSpeed;
        [SerializeField] private float burstFalloffTime;

        private Dictionary<int, Vector2> lanePositions = new();

        private float lane0Angle = Mathf.Infinity;
        private float anglePerLane = Mathf.Infinity;
        private int numberOfLanes = -1;

        private bool bursting = false;
        private float burstStart = Mathf.Infinity;
        private float burstEnd = Mathf.Infinity;
        private bool leftDown = false;
        private bool rightDown = false;

        private float currentAngle;
        private float currentBurst = 0;

        void OnEnable()
        {
            lane0AngleStream.Sub(SetLane0Angle);
            numberOfLanesStream.Sub(SetNumLanes);
            inp.A.started += ADown;
            inp.A.canceled += AUp;
            inp.W.performed += WPress;
            inp.D.started += DDown;
            inp.D.canceled += DUp;
            inp.S.performed += SPress;
            inp.Space.started += StartJump;
            inp.Space.canceled += EndJump;
            inp.Shift.performed += BurstSpeed;
        }
        void OnDisable()
        {
            lane0AngleStream.Unsub(SetLane0Angle);
            numberOfLanesStream.Unsub(SetNumLanes);
            inp.A.started -= ADown;
            inp.A.canceled -= AUp;
            inp.W.performed -= WPress;
            inp.D.started -= DDown;
            inp.D.canceled -= DUp;
            inp.S.performed -= SPress;
            inp.Space.started -= StartJump;
            inp.Space.canceled -= EndJump;
            inp.Shift.performed -= BurstSpeed;
        }

        private void SetLane0Angle(float to) => lane0Angle = to;
        private void SetNumLanes(int to) { numberOfLanes = to; RecalcLaneDirections(); }

        void FixedUpdate()
        {
            RunMovement();
        }

        private void ADown(InputAction.CallbackContext _) { rightDown = false; leftDown = true; }
        private void AUp(InputAction.CallbackContext _) => leftDown = false;

        private void DDown(InputAction.CallbackContext _) { leftDown = false; rightDown = true; }
        private void DUp(InputAction.CallbackContext _) => rightDown = false;

        private void WPress(InputAction.CallbackContext _)
        {
            currentSelectedLane = FindLaneFromCurrentRotation();
            Debug.Log("clicked on " + currentSelectedLane);
            ClickLane(_);
        }
        private void SPress(InputAction.CallbackContext _)
        {
            currentAngle += 180;
        }

        private void BurstSpeed(InputAction.CallbackContext _)
        {
            if (bursting) return;
            Debug.Log("bursting");
            bursting = true;
            burstStart = Time.fixedTime;
            burstEnd = Time.fixedTime + burstFalloffTime;
        }

        protected override void ClickLane(InputAction.CallbackContext _)
        {
            someLaneClicked.Invoke(currentSelectedLane);
        }
        protected override void StartJump(InputAction.CallbackContext _)
        {
            if (jumping) return;
            currentSelectedLane = FindLaneFromCurrentRotation();

            jumpStartedAt = Time.time;
            jumping = true;
            jumpDown = true;
            jumpingToPoint = lanePositions[currentSelectedLane] * movementData.jumpOffset;

            NewLaneJumped();
        }

        private int FindLaneFromCurrentRotation()
        {
            if (numberOfLanes == -1 || lane0Angle == Mathf.Infinity)
            {
                Debug.LogWarning("did not ever recieve lane 0 or number of lanes");
                return 0;
            }

            // find the angle
            float vectorAngle = Mathf.Atan2(playerCollider.transform.up.y, playerCollider.transform.up.x) * Mathf.Rad2Deg;
            if (vectorAngle > lane0Angle)
            {
                vectorAngle -= 360;
            }

            vectorAngle -= lane0Angle; // brings it to 0--> -360

            // turn the angle into a lane based on lane0Angle and anglePerLane
            int lane = Mathf.Abs(Mathf.FloorToInt(vectorAngle / anglePerLane)) - 1;
            return lane;
        }

        private void RecalcLaneDirections()
        {
            anglePerLane = 360f / numberOfLanes;
            float curAngle = numberOfLanes % 2 == 0 ? anglePerLane / 2 : anglePerLane;
            if (numberOfLanes == 8) curAngle = anglePerLane;

            for (int i = 0; i < numberOfLanes; i++)
            {
                float halfwayAngle = curAngle - (anglePerLane * 0.5f);
                Vector2 laneDirection = new(Mathf.Cos(halfwayAngle * Mathf.Deg2Rad), Mathf.Sin(halfwayAngle * Mathf.Deg2Rad));

                curAngle -= anglePerLane;
                lanePositions[i] = laneDirection;
            }
        }

        private void RunMovement()
        {
            // Update the current angle based on leftDown, rightDown, and bursting
            if (bursting)
            {
                float lerpAmt = Mathf.Clamp((Time.fixedTime - burstStart) / burstFalloffTime, 0, 1);
                currentBurst = Mathf.Lerp(burstSpeed, 0, lerpAmt);
                if (Time.fixedTime > burstEnd) bursting = false;
            }

            if (leftDown)
            {
                currentAngle += (movementSpeed + currentBurst) * Time.fixedDeltaTime;
            }
            else if (rightDown)
            {
                currentAngle -= (movementSpeed + currentBurst) * Time.fixedDeltaTime;
            }

            playerCollider.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }
}
