using DG.Tweening;
using UnityEngine;

namespace RhythmBeatObjects
{

    public class SpikeVisuals : MonoBehaviour
    {
        [SerializeField] private GameObject spikes1;
        [SerializeField] private GameObject spikes2;
        [SerializeField] private float scaleSpeedSeconds = 0.2f;
        [SerializeField] private Vector3 minScale = new(0.5f, 0.5f, 1f);
        [SerializeField] private Vector3 maxScale = Vector3.one;
        [SerializeField] private Ease scaleEaseType;
        [SerializeField] private float rotationTime;

        private Sequence scaleAnimation;
        private Sequence rotationAnimation;

        void Awake()
        {
            if (spikes1 == null || spikes2 == null) Debug.Log("set spikes on spiker");
            else
            {
                scaleAnimation = DOTween.Sequence();
                spikes1.transform.localScale = minScale;
                spikes2.transform.localScale = maxScale;
                scaleAnimation.Append(spikes1.transform
                        .DOScale(maxScale, scaleSpeedSeconds)
                        .SetEase(scaleEaseType))
                    .Join(spikes2.transform
                        .DOScale(minScale, scaleSpeedSeconds)
                        .SetEase(scaleEaseType))
                    .Append(spikes1.transform
                        .DOScale(minScale, scaleSpeedSeconds)
                        .SetEase(scaleEaseType))
                    .Join(spikes2.transform
                        .DOScale(maxScale, scaleSpeedSeconds)
                        .SetEase(scaleEaseType))
                    .SetLoops(-1)
                    .SetLink(gameObject);
                rotationAnimation = DOTween.Sequence();
                rotationAnimation.Append(
                        transform.DORotate(new Vector3(0, 0, 180), rotationTime / 2).SetEase(Ease.Linear)
                    ).Append(
                        transform.DORotate(new Vector3(0, 0, 360), rotationTime / 2).SetEase(Ease.Linear)
                    ).SetLoops(-1).SetLink(gameObject);
            }
        }
        
        void OnEnable()
        {
            scaleAnimation.Play();
            rotationAnimation.Play();
        }

        void OnDisable()
        {
            scaleAnimation.Pause();
            rotationAnimation.Pause();
        }
    }
}