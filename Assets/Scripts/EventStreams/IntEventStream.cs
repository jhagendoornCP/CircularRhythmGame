using UnityEngine;
using UnityEngine.Events;

namespace EventStreams
{
    [CreateAssetMenu(fileName = "IntEventStream", menuName = "Streams/Int stream")]
    public class IntEventStream : ScriptableObject
    {
        private event UnityAction<int> Stream;
        public void Sub(UnityAction<int> subber) => Stream += subber;
        public void Unsub(UnityAction<int> subber) => Stream -= subber;
        public void Invoke(int value) => Stream?.Invoke(value);
    }
}
