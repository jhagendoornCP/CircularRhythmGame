using EventStreams;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private FloatEventStream playerHealthStream;
    private Image healthbar;
    void Awake()
    {
        healthbar = GetComponent<Image>();
    }
    void OnEnable()
    {
        playerHealthStream.Sub(HealthChanged);
    }
    void OnDisable()
    {
        playerHealthStream.Unsub(HealthChanged);
    }

    void HealthChanged(float to)
    {
        healthbar.fillAmount = to;
    }
}
