using UnityEngine;
using UnityEngine.Events;

namespace EventStreams
{
    [CreateAssetMenu(fileName = "AudioClipEventStream", menuName = "Streams/Audio clip stream")]
    public class AudioClipEventStream : ScriptableObject
    {
        private event UnityAction<AudioClip> Stream;
        public void Sub(UnityAction<AudioClip> subber) => Stream += subber;
        public void Unsub(UnityAction<AudioClip> subber) => Stream -= subber;
        public void Invoke(AudioClip clip) => Stream?.Invoke(clip);
    }
}
