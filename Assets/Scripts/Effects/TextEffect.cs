using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RhythmEffects
{
    public class TextEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private TextEffectData data;

        private ParticleSystem linkedParticleSystem;
        private Color initialWithAlpha;

        void Awake()
        {
            if (text == null) text = GetComponentInChildren<TMP_Text>();
            if (data.particles != null)
            {
                linkedParticleSystem = Instantiate(data.particles, transform).GetComponent<ParticleSystem>();
                linkedParticleSystem.transform.localPosition = Vector3.zero;
            }
            if (data.flashTimes < 1) Debug.LogWarning("Flash times for " + gameObject.name + " has a non-positive number");

            initialWithAlpha = new(data.initialColor.r, data.initialColor.g, data.initialColor.b, 0);
        }
        public void RunEffect(UnityAction<GameObject> onFinish)
        {
            text.color = initialWithAlpha;
            // Do some cool color shenanigans && maybe some effect
            if (linkedParticleSystem != null) linkedParticleSystem.Play();
            Sequence flashSequence = DOTween.Sequence();
            // set up the initial flash which includes a fade in
            flashSequence.Append(text.DOColor(data.initialColor, data.fadeInTime));
            if (data.fadeInTime < data.flashStayColorTime) flashSequence.AppendInterval(data.flashStayColorTime - data.fadeInTime);
            flashSequence.Append(text.DOColor(data.flashColor, data.flashChangeColorTime))
                .AppendInterval(data.flashStayColorTime)
                .Append(text.DOColor(data.initialColor, data.flashChangeColorTime));

            // 1...n-1
            for (int i = 1; i < data.flashTimes - 1; i++)
            {
                flashSequence.AppendInterval(data.flashStayColorTime)
                    .Append(text.DOColor(data.flashColor, data.flashChangeColorTime))
                    .AppendInterval(data.flashStayColorTime)
                    .Append(text.DOColor(data.initialColor, data.flashChangeColorTime));
            }

            // set up the final flash which includes a fade out
            if (data.fadeOutTime < data.flashStayColorTime) flashSequence.AppendInterval(data.flashStayColorTime - data.fadeOutTime);
            flashSequence.Append(text.DOColor(initialWithAlpha, data.fadeOutTime))
                .AppendCallback(() => onFinish.Invoke(gameObject));

            flashSequence.Play();
        }
    }
}