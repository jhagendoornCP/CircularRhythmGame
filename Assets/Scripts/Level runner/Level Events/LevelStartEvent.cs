using UnityEngine;

namespace RhythmLevel
{
    [CreateAssetMenu(fileName = "Start", menuName = "Levels/LevelEvents/Start Event")]
    public class LevelStartEvent : BasicLevelEvent
    {
        private readonly LevelEventType eventType = LevelEventType.StartEvent;
        public override LevelEventType GetEventType() => eventType;
    }
}