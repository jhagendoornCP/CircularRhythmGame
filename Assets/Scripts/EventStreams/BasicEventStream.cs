using UnityEngine;
using UnityEngine.Events;

namespace EventStreams
{
    [CreateAssetMenu(fileName = "BasicEventStream", menuName = "Streams/Basic stream")]
    public class BasicEventStream : ScriptableObject
    {
        private event UnityAction Stream;
        public void Sub(UnityAction subber) => Stream += subber;
        public void Unsub(UnityAction subber) => Stream -= subber;
        public void Invoke() => Stream?.Invoke();
    }
}
