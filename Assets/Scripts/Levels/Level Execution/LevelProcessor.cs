using System;
using System.Collections;
using EventStreams;
using RhythmBeatObjects;
using UnityEngine;

namespace RhythmLevel
{
    [RequireComponent(typeof(LevelRunner), typeof(LevelScoreKeeper), typeof(LevelAudioPlayer))]
    public class LevelProcessor : MonoBehaviour
    {
        [SerializeField] private LevelData levelData;
        [SerializeField] private BasicEventStream levelStartingStream;
        [SerializeField] private IntEventStream overrideNumLanesForExampleBuildStream;
        [SerializeField] private FloatEventStream noteRushTimeStream;
        [SerializeField] private IntEventStream numberLanesStream;
        [SerializeField] private BasicEventStream playerDeathStream;

        [SerializeField] private GameObject beatLinesObject;
        private Material beatLinesMaterial;

        private LevelAudioPlayer audioPlayer;
        private double secPerBeat;
        private int beatPos = 0;

        private bool levelRunning = false;

        void Awake()
        {
            audioPlayer = GetComponent<LevelAudioPlayer>();
        }

        void OnEnable()
        {
            // levelStartingStream.Sub(StartLevel);
            overrideNumLanesForExampleBuildStream.Sub(StartLevelWithNumLanes);
            playerDeathStream.Sub(StopLevel);
        }
        
        void OnDisable()
        {
            // levelStartingStream.Unsub(StartLevel);
            overrideNumLanesForExampleBuildStream.Unsub(StartLevelWithNumLanes);
            playerDeathStream.Unsub(StopLevel);
        }

        void Start()
        {
            beatLinesMaterial = Instantiate(beatLinesObject.GetComponent<SpriteRenderer>().material);
            beatLinesObject.GetComponent<SpriteRenderer>().material = beatLinesMaterial;
            BeatManager.instance.InitializeObjects(levelData.maxBeatsOnScreenAtOnce);
            numberLanesStream.Invoke(levelData.numLanes);
            noteRushTimeStream.Invoke(levelData.noteRushTime);
            secPerBeat = 60f / levelData.bpm;
            audioPlayer.SetSong(levelData.song);
        }

        void Update()
        {
            if (!levelRunning) return;

            if (levelData.events[beatPos].beatTime * secPerBeat - levelData.noteRushTime <= DSPSyncer.instance.GetDspSongProgress())
            {
                levelData.events[beatPos].ExecuteEvent();
                beatPos += 1;
            }
            if (beatPos >= levelData.events.Count) { levelRunning = false; }
        }

        void StopLevel()
        {
            levelRunning = false;
        }

        void StartLevelWithNumLanes(int howMany)
        {
            numberLanesStream.Invoke(howMany);
            beatPos = 0;
            StartLevel();
        }

        void StartLevel()
        {
            levelStartingStream.Invoke();

            // schedule the start of the song, and the start of the TikToks. the song start should be scheduled relative
            // to either 1) the amount of time the tiktoks will take or 2) the amount of time it will take for the first beat to reach the player,
            // whichever is longer

            double timeForFirstBeatObject = Math.Abs(levelData.events[0].beatTime * secPerBeat - levelData.noteRushTime);
            double timeForTickTocks = Math.Abs(levelData.presongTickAmount * secPerBeat);

            if (timeForTickTocks > timeForFirstBeatObject)
            {
                audioPlayer.ScheduleTickTock(DSPSyncer.currentEstimatedDspTime, secPerBeat, levelData.presongTickAmount);
                audioPlayer.ScheduleSongPlayback(DSPSyncer.currentEstimatedDspTime + timeForTickTocks);
            }
            else
            {
                audioPlayer.ScheduleSongPlayback(DSPSyncer.currentEstimatedDspTime + timeForFirstBeatObject);
                audioPlayer.ScheduleTickTock(DSPSyncer.currentEstimatedDspTime + timeForFirstBeatObject - timeForTickTocks, secPerBeat, levelData.presongTickAmount);
            }

            levelRunning = true;
        }
    }
}