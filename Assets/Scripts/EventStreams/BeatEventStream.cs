using UnityEngine;
using UnityEngine.Events;

namespace EventStreams
{
    [CreateAssetMenu(fileName = "BeatEventStream", menuName = "Streams/Beat stream")]
    public class BeatEventStream : ScriptableObject
    {
        private event UnityAction<int, string, BeatEventStreamAction> Stream;
        public void Sub(UnityAction<int, string, BeatEventStreamAction> subber) => Stream += subber;
        public void Unsub(UnityAction<int, string, BeatEventStreamAction> subber) => Stream -= subber;
        public void Invoke(int lane, string beatId, BeatEventStreamAction action = BeatEventStreamAction.Add) => Stream?.Invoke(lane, beatId, action);
    }

    public enum BeatEventStreamAction
    {
        Add, Subtract
    }
}
