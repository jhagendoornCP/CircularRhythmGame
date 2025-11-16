

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EventStreams;
using UnityEngine;
using UnityEngine.AI;

namespace RhythmLevel
{
    public class LevelAudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip tickClip;
        [SerializeField] private AudioClip tockClip;
        [SerializeField] private AudioSource songSource;
        [SerializeField] private BasicEventStream playerDeathStream;

        private List<AudioSource> tickTockSources = new();
        private const int tickTockSourceNumber = 8;

        Sequence pitchDownSequence;


        void Awake()
        {
            for (int i = 0; i < tickTockSourceNumber; i++)
            {
                GameObject g = new()
                {
                    name = "TickTockSource" + i
                };
                AudioSource added = g.AddComponent<AudioSource>();
                g.transform.parent = songSource.transform.parent;
                tickTockSources.Add(added);
            }
            if (songSource == null) Debug.LogWarning("LevelAudioPlayer missing song source");
        }

        void OnEnable()
        {
            playerDeathStream.Sub(StopAndPitchDownAudio);
        }

        void OnDisable()
        {
            playerDeathStream.Unsub(StopAndPitchDownAudio);
        }

        private void StopAndPitchDownAudio()
        {
            pitchDownSequence = DOTween.Sequence();
            pitchDownSequence
                .Append(songSource.DOPitch(-1f, 2f).SetEase(Ease.InSine))
                .Join(songSource.DOFade(0, 2f).SetEase(Ease.InSine))
                .AppendCallback(() => songSource.Stop());
        }

        public void SetSong(AudioClip song)
        {
            songSource.clip = song;
        }

        public void ScheduleSongPlayback(double when)
        {
            if (pitchDownSequence.IsActive()) pitchDownSequence.Kill();
            songSource.Stop();
            songSource.pitch = 1;
            songSource.volume = 0.345f;
            DSPSyncer.instance.SongStartsAt(when);
            songSource.PlayScheduled(when);
        }

        public void ScheduleTickTock(double startingWhen, double bps, int number)
        {
            // Debug.Log($"Scheduling {number} tocks that should happen every {bps}, starting at {startingWhen}");
            for (int i = 0; i < number; i++)
            {
                if (i % 2 == 0)
                {
                    tickTockSources[i % tickTockSources.Count].clip = tickClip;
                }
                else tickTockSources[i % tickTockSources.Count].clip = tockClip;
                tickTockSources[i % tickTockSources.Count].PlayScheduled(startingWhen + bps * i);
            }
        }
    }
}