
using DG.Tweening;
using UnityEngine;

namespace RhythmBeatObjects
{
    [RequireComponent(typeof(SuddenMoveInDamager))]
    public class SuddenDamagerVisuals : MonoBehaviour
    {
        private SpriteRenderer sr;

        private Color initialDangerWallColor;
        private Color initialWithAlpha;

        void Awake()
        {
            SuddenMoveInDamager dw = GetComponent<SuddenMoveInDamager>();
            dw.OnStartMoveIn += MoveInAnimation;
            dw.OnStartMoveOut += MoveOutAnimation;
            sr = GetComponentInChildren<SpriteRenderer>();
            initialDangerWallColor = sr.color;
            initialWithAlpha = initialDangerWallColor;
            initialWithAlpha.a = 0;
            sr.color = initialWithAlpha;
        }

        void MoveInAnimation(float time)
        {
            sr.DOColor(initialDangerWallColor, time);
        }
        
        void MoveOutAnimation(float time)
        {
            sr.DOColor(initialWithAlpha, time);
        }
    }
}