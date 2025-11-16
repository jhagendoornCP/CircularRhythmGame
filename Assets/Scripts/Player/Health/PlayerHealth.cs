
using EventStreams;
using RhythmLevel;
using UnityEngine;

namespace RhythmPlayer
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Streams")]
        [SerializeField] FloatEventStream currentHealthPercentStream;
        [SerializeField] IntEventStream scoredStream;
        [SerializeField] BasicEventStream deathOccuredStream;
        [SerializeField] FloatEventStream playerDamageStream;
        [SerializeField] BasicEventStream levelStartedStream;
        [Header("Health and damage data")]
        [SerializeField] HealthData healthData;
        [SerializeField] private float healAmountFromHit;
        [SerializeField] private float damageAmountFromMiss;
        [SerializeField] private bool godmode;

        private float currentHealth;

        void Awake()
        {
            currentHealth = healthData.maxHealth;
        }

        void OnEnable()
        {
            scoredStream.Sub(ScoreEvent);
            playerDamageStream.Sub(TakeDamage);
            levelStartedStream.Sub(MaxHealth);
        }

        void OnDisable()
        {
            scoredStream.Unsub(ScoreEvent);
            playerDamageStream.Unsub(TakeDamage);
            levelStartedStream.Unsub(MaxHealth);
        }

        void Start()
        {
            currentHealthPercentStream.Invoke(1);
        }

        void ScoreEvent(int type)
        {
            if ((ScoreType)type == ScoreType.MISS) TakeDamage(damageAmountFromMiss);
            else Heal(healAmountFromHit);
        }

        void TakeDamage(float amount) { 
            Debug.Log("ouch!");
            Heal(-amount);
        }
        void MaxHealth() => Heal(healthData.maxHealth * 100000);
        void Heal(float amount)
        {
            if (godmode) return;
            currentHealth = Mathf.Min(currentHealth + amount, healthData.maxHealth);
            if (currentHealth <= 0) deathOccuredStream.Invoke();
            EmitCurrentHealth();
        }

        private void EmitCurrentHealth()
        {
            currentHealthPercentStream.Invoke(currentHealth / healthData.maxHealth);
        }
    }
}