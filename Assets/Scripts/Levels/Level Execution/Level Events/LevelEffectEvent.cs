using UnityEngine;

namespace RhythmLevel
{
    [CreateAssetMenu(fileName = "LevelEffectEvent", menuName = "Levels/LevelEvents/Effect Event")]
    public class LevelEffectEvent : BasicLevelEvent
    {
        private readonly LevelEventType eventType = LevelEventType.EffectEvent;
        public override LevelEventType GetEventType() => eventType;
    }
}