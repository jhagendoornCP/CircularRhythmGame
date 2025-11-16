using DG.Tweening;
using EventStreams;
using RhythmEffects;
using RhythmLevel;
using UnityEngine;

namespace RhythmBeatObjects
{
    public class TapBeat : SimpleClickableBeat
    {
        [Header("[[[TAP BEAT]]]")]
        [Header("streams")]
        [SerializeField] private IntEventStream beatScoredStream;
        [Header("distances")]
        [SerializeField] private float perfectScoreDistanceAllowance = 0.1f;
        [SerializeField] private float goodScoreDistanceAllowance = 0.45f;
        [SerializeField] private float badScoreDistanceAllowance = 1.0f;

        [Header("animations")]
        [SerializeField] private float clickedScaleDownTime = 0.2f;

        private bool wasClicked = false;

        public override void ClickBeat()
        {
            wasClicked = true;

            // calculate the score based on distance from finalPosition
            Vector2 dist = new Vector2(transform.position.x, transform.position.y) - finalPosition;
            float mag = dist.magnitude;

            if (mag < perfectScoreDistanceAllowance) BeatScored(ScoreType.PERFECT, "perfectText");
            else if (mag < goodScoreDistanceAllowance && !inDestructMode) // basically this is saying that if you miss the ring, you can only get perfect or bad, no good
            {
                BeatScored(ScoreType.GOOD, "goodText");
            }
            else if (mag < badScoreDistanceAllowance) BeatScored(ScoreType.BAD, "badText");
            else BeatScored(ScoreType.MISS, "missText");

            if (!inDestructMode) transform.DOScale(0, clickedScaleDownTime).OnComplete(BeatHasVanished);
        }

        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            wasClicked = false;
        }

        public override void DestructBeat(bool withMovementFade = false)
        {
            base.DestructBeat(withMovementFade && !wasClicked);
        }

        protected override void BeatHasVanished()
        {
            if (!initialized) return;
            base.BeatHasVanished();

            // if beat was never clicked but it did get missed
            if (inDestructMode && !wasClicked)
            {
                BeatScored(ScoreType.MISS, "missText");
            }
        }
        
        private void BeatScored(ScoreType type, string effectKey)
        {
            beatScoredStream.Invoke((int)type);
            if (EffectRequestManager.instance != null) EffectRequestManager.instance.RequestEffect(effectKey, transform.position);
        }
    }
}