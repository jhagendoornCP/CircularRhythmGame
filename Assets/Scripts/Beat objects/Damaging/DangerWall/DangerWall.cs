namespace RhythmBeatObjects
{
    public class DangerWall : SuddenMoveInDamager
    {
        public override void Initialize(BeatObjectInitializationParams args)
        {
            base.Initialize(args);
            // queue obstacle objects in the other fields
            // some fucky math but it works (trust me bro)
            int notInLane = (args.inLane + args.totalLanes / 2) % args.totalLanes;
            for (int i = 0; i < args.totalLanes; i++)
            {
                if (i != notInLane) QueueWarning(i);
            }
        }
    }
}