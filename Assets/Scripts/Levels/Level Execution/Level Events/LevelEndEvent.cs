using UnityEngine;

namespace RhythmLevel
{
    [CreateAssetMenu(fileName = "EndEvent", menuName = "Levels/LevelEvents/End Event")]
    public class LevelEndEvent : BasicLevelEvent
    {
        private readonly LevelEventType eventType = LevelEventType.EndEvent;
        public override LevelEventType GetEventType() => eventType;
    }
}