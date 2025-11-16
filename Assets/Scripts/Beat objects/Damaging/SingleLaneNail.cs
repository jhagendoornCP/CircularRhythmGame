
namespace RhythmBeatObjects
{
    public class SingleLaneNail : SuddenMoveInDamager
    {
        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            QueueWarning(args.inLane);
        }
    }
}