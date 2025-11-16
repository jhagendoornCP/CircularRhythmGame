

using UnityEngine;

[CreateAssetMenu(fileName = "HealthData", menuName = "Player/Health Data")]
public class HealthData : ScriptableObject
{
    public int maxHealth;
    public float regenPerSecond;
}