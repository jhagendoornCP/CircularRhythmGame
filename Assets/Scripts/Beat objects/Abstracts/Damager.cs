using UnityEngine;

namespace RhythmBeatObjects
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Damager : BeatObject
    {
        [Header("[[[DAMAGER]]]")]
        [SerializeField] private int damage;
        [SerializeField] private DamageType damageType;
        [SerializeField] private float damageIntervalSeconds; // if damageType not Single, damage can be done this often

        public int GetDamage() => damage;
    }

    public enum DamageType
    {
        Single,
        Multiplicative
    }
}