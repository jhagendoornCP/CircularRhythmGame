using EventStreams;
using UnityEngine;

namespace RhythmBeatObjects
{
    public abstract class BeatObject : MonoBehaviour
    {
        [Header("[[[BEAT OBJECT BASE]]]")]
        public string beatId;
        [SerializeField] protected BeatEventStream beatStream;

        protected double centerCircleTime;
        protected double awakeTime;
        protected Vector2 finalPosition;
        protected Vector2 startPosition;

        public virtual void Initialize(BeatObjectInitializationParams args)
        {
            startPosition = args.startPosition;
            centerCircleTime = DSPSyncer.currentEstimatedDspTime + args.rushTime;
            awakeTime = DSPSyncer.currentEstimatedDspTime;
            finalPosition = args.endPosition;
        }

        public abstract void EarlyTerminate();

        public class BeatObjectInitializationParams
        {
            public float rushTime;
            public Vector2 startPosition;
            public Vector2 endPosition;
            public int inLane;
            public int totalLanes;
        }
    }
}