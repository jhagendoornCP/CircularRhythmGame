using EventStreams;
using RhythmBeatObjects;
using UnityEditor;
using UnityEngine;

namespace RhythmLevel
{
    [CreateAssetMenu(fileName = "LevelBeatEvent", menuName = "Levels/LevelEvents/Beat Event")]
    public class LevelBeatEvent : BasicLevelEvent
    {
        private readonly LevelEventType eventType = LevelEventType.BeatEvent;
        public override LevelEventType GetEventType() => eventType;

        public BeatEventStream stream;
        public BeatObject beatObject;

        public override void ExecuteEvent()
        {
            stream.Invoke(lane, beatObject.beatId, BeatEventStreamAction.Add);
        }
    }
}