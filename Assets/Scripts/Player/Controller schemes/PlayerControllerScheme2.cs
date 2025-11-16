using System.Collections.Generic;
using EventStreams;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RhythmPlayer
{
    // This is the entirely clicking scheme (left click to tap, right click to jump)
    public class PlayerControllerScheme2 : PlayerControllerBase
    {
        [Header("Streams")]
        [SerializeField] private FloatEventStream lane0AngleStream;
        [SerializeField] private IntEventStream numberOfLanesStream;

        private Dictionary<int, Vector2> lanePositions = new();

        private float lane0Angle = Mathf.Infinity;
        private float anglePerLane = Mathf.Infinity;
        private int numberOfLanes = -1;

        void OnEnable()
        {
            lane0AngleStream.Sub(SetLane0Angle);
            numberOfLanesStream.Sub(SetNumLanes);
            inp.LClick.performed += ClickLane;
            inp.RClick.started += StartJump;
            inp.RClick.canceled += EndJump;
        }
        void OnDisable()
        {
            lane0AngleStream.Unsub(SetLane0Angle);
            numberOfLanesStream.Unsub(SetNumLanes);
            inp.LClick.performed -= ClickLane;
            inp.RClick.started -= StartJump;
            inp.RClick.canceled -= EndJump;
        }

        private void SetLane0Angle(float to) => lane0Angle = to;
        private void SetNumLanes(int to) { numberOfLanes = to; RecalcLaneDirections(); }

        protected override void ClickLane(InputAction.CallbackContext _)
        {
            currentSelectedLane = FindLaneFromMousePos(inp.MousePos.ReadValue<Vector2>());
            someLaneClicked.Invoke(currentSelectedLane);
            ChangeFacingDirection(lanePositions[currentSelectedLane]);
        }

        protected override void StartJump(InputAction.CallbackContext _)
        {
            if (jumping) return;
            currentSelectedLane = FindLaneFromMousePos(inp.MousePos.ReadValue<Vector2>());
            ChangeFacingDirection(lanePositions[currentSelectedLane]);

            jumpStartedAt = Time.time;
            jumping = true;
            jumpDown = true;
            jumpingToPoint = lanePositions[currentSelectedLane] * movementData.jumpOffset;

            NewLaneJumped();
        }

        private int FindLaneFromMousePos(Vector2 mousePos)
        {
            if (numberOfLanes == -1 || lane0Angle == Mathf.Infinity)
            {
                Debug.LogWarning("did not ever recieve lane 0 or number of lanes");
                return 0;
            }
            // convert to world
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.DrawLine(Vector2.zero, worldPos, Color.blue, 5);

            // find the angle
            float vectorAngle = Mathf.Atan2(worldPos.y, worldPos.x) * Mathf.Rad2Deg;
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
    }
}
