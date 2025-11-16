using UnityEngine;

[CreateAssetMenu(fileName = "TextEffectData", menuName = "Effects/Text Effect")]
public class TextEffectData : ScriptableObject
{
    [Tooltip("should have a particle system component on it; can be left null")]
    public GameObject particles;
    [Header("color flashing")]
    public Color initialColor;
    public Color flashColor;
    [Tooltip("the amount of time that the text will stay on initial/flash color")]
    public float flashStayColorTime;
    [Tooltip("the amount of time that it takes the text to change color")]
    public float flashChangeColorTime;
    public int flashTimes;
    public float fadeInTime = 0.05f;
    public float fadeOutTime = 0.05f;

    [Header("movement")]
    public Vector3 movement;
    public float movementTime;
    public float movementDelay;
}
