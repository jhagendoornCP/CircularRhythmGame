using EventStreams;
using RhythmBeatObjects;
using UnityEngine;

namespace RhythmPlayer
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerColliderWrapper : MonoBehaviour
    {
        [SerializeField] private FloatEventStream playerDamageStream;
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Damager")) return;

            playerDamageStream.Invoke(collision.GetComponent<Damager>().GetDamage());
        }
    }
}