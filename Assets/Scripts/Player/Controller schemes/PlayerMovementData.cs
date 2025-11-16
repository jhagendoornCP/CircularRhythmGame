
using UnityEngine;

namespace RhythmPlayer
{
    [CreateAssetMenu(fileName = "Player Movement Data", menuName = "Player/Movement Data")]
    public class PlayerMovementData : ScriptableObject
    {
        [Header("Jumping")]
        public float jumpMoveInTime;
        public float jumpMoveOutTime;
        public float jumpMaxHoldTime;
        public float jumpOffset;
    }
}