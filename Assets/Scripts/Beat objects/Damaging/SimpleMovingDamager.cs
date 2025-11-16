
using EventStreams;
using UnityEngine;

namespace RhythmBeatObjects
{
    public abstract class SimpleMovingDamager : Damager
    {
        [Header("[[[SIMPLE MOVING DAMAGER]]]")]
        protected int lane;
        protected bool initialized = false;
        protected bool takingDamageAction = false;

        protected virtual void Update()
        {
            if (!initialized) return;
            if (takingDamageAction) { DoDamageAction(); return; }
            float currentAmount = (float)((DSPSyncer.currentEstimatedDspTime - awakeTime) / (centerCircleTime - awakeTime));

            transform.position = Vector2.Lerp(startPosition, finalPosition, currentAmount);

            if (DSPSyncer.currentEstimatedDspTime >= centerCircleTime) StartDamageAction();
        }

        public override void EarlyTerminate()
        {
            StartDamageAction();
        }

        protected virtual void StartDamageAction()
        {
            takingDamageAction = true;
        }
        protected abstract void DoDamageAction();

        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            transform.position = args.startPosition;
            lane = args.inLane;
            takingDamageAction = false;
            initialized = true;

            transform.localScale = Vector2.one;
        }

        protected virtual void BeatHasVanished()
        {
            if (!initialized) return;

            beatStream.Invoke(lane, beatId, BeatEventStreamAction.Subtract);
            initialized = false;
        }
 
    }
}