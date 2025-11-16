using DG.Tweening;
using UnityEngine;

namespace RhythmBeatObjects
{
    public class SpikeBeat : SimpleMovingDamager
    {
        [Header("[[[SPIKE BEAT]]]")]
        [SerializeField] private float lungeTime;
        [SerializeField] private float lungesToOffset;
        [SerializeField] private float startScaleDownAtPercentThroughLunge;
        [SerializeField] private float scaleDownTime;

        private bool scalingDown = false;
        private Vector3 finalLungePosition = Vector3.zero;

        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            scalingDown = false;
            finalLungePosition = -finalPosition.normalized * lungesToOffset;
        }
        protected override void StartDamageAction()
        {
            base.StartDamageAction();
            Debug.DrawLine(transform.position, finalLungePosition, Color.blue, 5);
        }

        protected override void DoDamageAction()
        {
            double lerpAmt = (DSPSyncer.currentEstimatedDspTime - centerCircleTime) / lungeTime;
            if (!scalingDown && lerpAmt > startScaleDownAtPercentThroughLunge) { scalingDown = true; transform.DOScale(0, scaleDownTime).OnComplete(BeatHasVanished); }

            transform.position = Vector3.Lerp(finalPosition, finalLungePosition, (float)lerpAmt);
        }
    }
}