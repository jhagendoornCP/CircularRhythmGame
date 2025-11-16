using UnityEngine;

namespace RhythmLevel
{
    public abstract class BasicLevelEvent : ScriptableObject
    {
        public float beatTime; // this is the time of the event
        public int lane;
        public abstract LevelEventType GetEventType();
        public virtual void ExecuteEvent() { }
    }

    public enum LevelEventType
    {
        StartEvent, EffectEvent, BeatEvent, DangerEvent, EndEvent
    }
}
