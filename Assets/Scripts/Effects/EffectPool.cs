using UnityEngine;

namespace RhythmEffects
{
    public abstract class EffectPool : MonoBehaviour
    {
        public abstract void PlayEffect(Vector3 where);
    }
}