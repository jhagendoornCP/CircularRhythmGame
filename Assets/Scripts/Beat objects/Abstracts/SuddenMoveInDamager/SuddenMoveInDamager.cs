

using EventStreams;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RhythmBeatObjects
{
    [RequireComponent(typeof(SuddenDamagerVisuals))]
    public abstract class SuddenMoveInDamager : Damager
    {
        [Header("[[[SUDDEN DAMAGER]]]")]
        [SerializeField] private float holdTime;
        [SerializeField] private float moveTime;
        [Tooltip("the amount of distance the wall will move over the origin (basically the higher this is, the harder it is)")]
        [SerializeField] private float distanceOverOrigin = 0.2f;
        [SerializeField] protected BeatObject obstacleWarningObject;

        public event UnityAction<float> OnStartMoveIn;
        public event UnityAction<float> OnStartMoveOut;

        private int mainLane;
        private SuddenDamagerState currentState = SuddenDamagerState.INVISIBLE;
        private bool hasStartedRush = false;

        void Update()
        {
            if (!hasStartedRush && currentState == SuddenDamagerState.INVISIBLE && DSPSyncer.currentEstimatedDspTime >= centerCircleTime - moveTime) StartRush();
            else if (currentState == SuddenDamagerState.MOVING_IN) MoveIn();
            else if (currentState == SuddenDamagerState.HOLDING) Hold();
            else if (currentState == SuddenDamagerState.MOVING_OUT) MoveOut();
        }

        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);

            mainLane = args.inLane;
            // set the position of the wall, as well as the final position of the wall
            transform.SetPositionAndRotation(finalPosition, Quaternion.LookRotation(Vector3.forward, -finalPosition));
            startPosition = finalPosition;
            finalPosition = -finalPosition.normalized * distanceOverOrigin;

            hasStartedRush = false;
        }

        private void StartRush()
        {
            hasStartedRush = true;
            currentState = SuddenDamagerState.MOVING_IN;
            OnStartMoveIn?.Invoke(moveTime);
        }

        private void MoveIn()
        {
            if (DSPSyncer.currentEstimatedDspTime >= centerCircleTime) currentState = SuddenDamagerState.HOLDING;
            else
            {
                transform.position = Vector3.Lerp(startPosition, finalPosition, (float)((DSPSyncer.currentEstimatedDspTime - (centerCircleTime - moveTime)) / moveTime));
            }
        }

        private void Hold()
        {
            if (DSPSyncer.currentEstimatedDspTime >= centerCircleTime + holdTime) { OnStartMoveOut?.Invoke(moveTime); currentState = SuddenDamagerState.MOVING_OUT; }
        }

        private void MoveOut()
        {
            if (DSPSyncer.currentEstimatedDspTime >= centerCircleTime + holdTime + moveTime) { currentState = SuddenDamagerState.INVISIBLE; BeatHasVanished(); }
            else
            {
                transform.position = Vector3.Lerp(finalPosition, startPosition, (float)((DSPSyncer.currentEstimatedDspTime - (centerCircleTime + holdTime)) / moveTime));
            }
        }

        protected void QueueWarning(int lane)
        {
            beatStream.Invoke(lane, obstacleWarningObject.beatId);
        }

        protected virtual void BeatHasVanished()
        {
            beatStream.Invoke(mainLane, beatId, BeatEventStreamAction.Subtract);
        }

        public override void EarlyTerminate()
        {
            if (currentState == SuddenDamagerState.INVISIBLE)
            {
                hasStartedRush = true;
                BeatHasVanished();
            }
        }

        private enum SuddenDamagerState
        {
            MOVING_IN, HOLDING, MOVING_OUT, INVISIBLE    
        }
    }
}