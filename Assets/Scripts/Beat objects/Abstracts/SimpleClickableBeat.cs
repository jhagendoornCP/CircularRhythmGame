
using DG.Tweening;
using EventStreams;
using UnityEngine;

namespace RhythmBeatObjects
{
    public abstract class SimpleClickableBeat : ClickableBeat
    {
        [Header("[[[SIMPLE CLICKABLE BEAT]]]")]
        [Header("animations")]
        [SerializeField] protected float missedScaleDownTime = 0.6f;
        [SerializeField] protected float missedMovementTimeRatio = 0.8f;

        protected int lane;

        protected bool inDestructMode = false;
        protected bool initialized = false;

        protected virtual void Update()
        {
            if (inDestructMode || !initialized) return;
            // use dsp time to lerp towards the final position
            float currentAmount = (float)((DSPSyncer.currentEstimatedDspTime - awakeTime) / (centerCircleTime - awakeTime));

            // Debug.Log($"(in beat): Awake: {awakeTime}, Final: {destructionTime}, Current: {DSPSyncer.instance.currentEstimatedDspTime}");

            transform.position = Vector2.Lerp(startPosition, finalPosition, currentAmount);

            if (DSPSyncer.currentEstimatedDspTime >= centerCircleTime) DestructBeat(true);
        }

        public override void EarlyTerminate()
        {
            DestructBeat(true);
        }

        public virtual void DestructBeat(bool withMovementFade = false)
        {
            inDestructMode = true;
            double movementTime = missedMovementTimeRatio * (centerCircleTime - awakeTime);
            transform.DOScale(0, missedScaleDownTime).OnComplete(BeatHasVanished);
            if (withMovementFade) transform.DOMove(new Vector2(0, 0), (float)movementTime);
        }

        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            transform.position = args.startPosition;
            lane = args.inLane;
            inDestructMode = false;
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