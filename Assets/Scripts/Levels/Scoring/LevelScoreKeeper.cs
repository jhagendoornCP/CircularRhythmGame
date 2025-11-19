using EventStreams;
using UnityEngine;

namespace RhythmLevel
{
    public class LevelScoreKeeper : MonoBehaviour
    {
        [Header("streams")]
        [SerializeField] private IntEventStream scoredStream;
        [SerializeField] private IntEventStream maxComboStream;
        [SerializeField] private IntEventStream currentScoreStream;
        [SerializeField] private IntEventStream currentComboStream;
        [SerializeField] private IntEventStream currentMultiplierStream;
        [SerializeField] private FloatEventStream progressToNextMultiplierStream;
        [SerializeField] private BasicEventStream levelStartingStream;

        [Header("Scoring data")]
        [SerializeField] private int badBaseScore;
        [SerializeField] private int goodBaseScore;
        [SerializeField] private int perfectBaseScore;
        [SerializeField] private int combosNeededToReachNextMultiplier;
        [Tooltip("multiplier of two otherwise everything will explode!")]
        [SerializeField] private int maxComboMultiplier;
        [Tooltip("when you miss somethign the combo mult goes down by this amounts")]
        [SerializeField] private int breakingComboSubtractsMultiplierAmt = 2;

        private int currentComboTracker;
        private int progressToNextMultiplier = 0;
        private int maxComboTracker;
        private int currentComboMultiplier;

        private int currentScore;

        void OnEnable()
        {
            scoredStream.Sub(NewScoredEvent);
            levelStartingStream.Sub(StartingLevel);
            currentComboMultiplier = 1;
            currentComboTracker = 0;
            progressToNextMultiplier = 0;
            maxComboTracker = 0;
        }
        void OnDisable()
        {
            scoredStream.Unsub(NewScoredEvent);
            levelStartingStream.Unsub(StartingLevel);
        }

        private void StartingLevel()
        {
            currentComboMultiplier = 1;
            currentComboTracker = 0;
            currentScore = 0;
            currentScoreStream.Invoke(currentScore);
            currentComboStream.Invoke(currentComboTracker);
        }

        private void NewScoredEvent(int type)
        {
            switch ((ScoreType)type)
            {
                case ScoreType.MISS:
                    Missed();
                    break;
                case ScoreType.BAD:
                    Scored(badBaseScore);
                    break;
                case ScoreType.GOOD:
                    Scored(goodBaseScore);
                    break;
                case ScoreType.PERFECT:
                    Scored(perfectBaseScore);
                    break;
                default:
                    Debug.Log("received scored event " + type + " which doesnt correspodn to anything");
                    break;
            }
        }

        private void Missed()
        {
            currentComboTracker = 0;
            currentComboMultiplier = Mathf.Max(1, currentComboMultiplier - breakingComboSubtractsMultiplierAmt);
            progressToNextMultiplier = 0;
            progressToNextMultiplierStream.Invoke(progressToNextMultiplier);
            currentComboStream.Invoke(currentComboTracker);
            currentMultiplierStream.Invoke(currentComboMultiplier);
        }

        private void Scored(int amount)
        {
            currentComboTracker += 1;
            // update the max combo if we need to
            if (currentComboTracker > maxComboTracker)
            {
                maxComboTracker = currentComboTracker;
                maxComboStream.Invoke(maxComboTracker);
            }
            currentComboStream.Invoke(currentComboTracker);

            // if we havent hit the current combo mult, need to update progress
            if (currentComboMultiplier < maxComboMultiplier)
            {
                progressToNextMultiplier += 1;
                // update the multiplier if we need to
                if (progressToNextMultiplier >= combosNeededToReachNextMultiplier)
                {
                    progressToNextMultiplier = 0;
                    currentComboMultiplier += 1; 
                    currentMultiplierStream.Invoke(currentComboMultiplier);
                }

                progressToNextMultiplierStream.Invoke(progressToNextMultiplier / (float)combosNeededToReachNextMultiplier);
            }

            // update the score
            currentScore += amount * currentComboMultiplier;
            currentScoreStream.Invoke(currentScore);
        }
    }

    public enum ScoreType
    {
        MISS=0, BAD=1, GOOD=2, PERFECT=3
    }
}