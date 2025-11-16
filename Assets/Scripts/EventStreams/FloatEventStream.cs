using UnityEngine;
using UnityEngine.Events;

namespace EventStreams
{
    [CreateAssetMenu(fileName = "FloatEventStream", menuName = "Streams/Float stream")]
    public class FloatEventStream : ScriptableObject
    {
        private event UnityAction<float> Stream;
        public void Sub(UnityAction<float> subber) => Stream += subber;
        public void Unsub(UnityAction<float> subber) => Stream -= subber;
        public void Invoke(float value) => Stream?.Invoke(value);
    }
}
