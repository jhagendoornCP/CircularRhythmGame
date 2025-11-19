using System.Collections.Generic;
using EventStreams;
using UnityEngine;
using RhythmBeatObjects;
using System;
using UnityEngine.Events;
using System.Collections;
using Unity.VisualScripting;

namespace RhythmLevel
{
    public class LevelRunner : MonoBehaviour
    {
        [Header("Streams")]
        [SerializeField] private BeatEventStream beatEvents;
        [SerializeField] private FloatEventStream noteRushTimeStream;
        [SerializeField] private FloatEventStream lane0AngleStream;
        [SerializeField] private IntEventStream numberOfLanesStream;
        [SerializeField] private IntEventStream laneClickedStream;
        [SerializeField] private BasicEventStream playerDeathStream;

        public event UnityAction<int> EmptyLaneClicked;
        [Header("Prefabs")]
        [SerializeField] private GameObject laneDivider;
        [Header("Display config")]
        [SerializeField] private float laneDividerOffsetDist = 7.95f;
        [SerializeField] private float laneSpawnDistance = 10;
        [SerializeField] private float laneEndDistance = 1.5f;

        // Spaghetti code
        private readonly Dictionary<int, Queue<ClickableBeat>> lanesClickable = new();
        private readonly Dictionary<int, Queue<BeatObject>> lanesUnclickable = new();
        private readonly Dictionary<int, int> cleanupDict = new();

        // (spawn, end)
        private readonly Dictionary<int, Tuple<Vector2, Vector2>> laneKeyPositions = new();
        private float noteRushTime;
        private int numberOfLanes;

        private bool terminatingAllBeats = false;
        private List<GameObject> laneObjects; // this is used for testing purposes


        void OnEnable()
        {
            beatEvents.Sub(NewEvent);
            noteRushTimeStream.Sub(NoteRushChanged);
            numberOfLanesStream.Sub(InitializeLanes);
            laneClickedStream.Sub(ClickLane);
            playerDeathStream.Sub(RemoveAllBeats);
        }
        void OnDisable()
        {
            beatEvents.Unsub(NewEvent);
            noteRushTimeStream.Unsub(NoteRushChanged);
            numberOfLanesStream.Unsub(InitializeLanes);
            laneClickedStream.Unsub(ClickLane);
            playerDeathStream.Unsub(RemoveAllBeats);
        }

        private void NoteRushChanged(float to)
        {
            noteRushTime = to;
        }
        
        private void InitializeLanes(int number)
        {
            terminatingAllBeats = false;
            if (laneObjects != null)
            {
                foreach (var obj in laneObjects) Destroy(obj);
            }

            float angleChange = 360f / number;
            float curAngle = number % 2 == 0 ? angleChange / 2 : angleChange;
            if (number == 8) curAngle = angleChange;
            lane0AngleStream.Invoke(curAngle);
            numberOfLanes = number;
            laneObjects = new();

            for (int i = 0; i < number; i++)
            {
                Vector2 instantiateLoc = new(Mathf.Cos(curAngle * Mathf.Deg2Rad), Mathf.Sin(curAngle * Mathf.Deg2Rad));
                instantiateLoc *= laneDividerOffsetDist;

                laneObjects.Add(Instantiate(laneDivider, instantiateLoc, Quaternion.Euler(0, 0, curAngle)));

                float tempAngle = curAngle - (angleChange * 0.5f);
                Vector2 beatSpawnLoc = new(Mathf.Cos(tempAngle * Mathf.Deg2Rad), Mathf.Sin(tempAngle * Mathf.Deg2Rad));
                Vector2 beatEndLoc = beatSpawnLoc * laneEndDistance;
                beatSpawnLoc *= laneSpawnDistance;

                laneKeyPositions[i] = new(beatSpawnLoc, beatEndLoc);
                lanesClickable[i] = new();
                lanesUnclickable[i] = new();
                cleanupDict[i] = 0;

                curAngle -= angleChange;
            }
        }

        private void NewEvent(int lane, string beatId, BeatEventStreamAction action)
        {
            if (lane >= numberOfLanes) lane = numberOfLanes - 1;
            if (action == BeatEventStreamAction.Add) AddBeat(lane, beatId);
            else RemoveBeat(lane, beatId);
        }

        private void AddBeat(int lane, string beatId)
        {
            (BeatObject beat, bool isClickable) = BeatManager.instance.GetBeatObject(beatId);

            beat.Initialize(
                new()
                {
                    rushTime = noteRushTime,
                    startPosition = laneKeyPositions[lane].Item1,
                    endPosition = laneKeyPositions[lane].Item2,
                    inLane = lane,
                    totalLanes = numberOfLanes
                });
            if (isClickable) lanesClickable[lane].Enqueue(beat.GetComponent<ClickableBeat>());
            else lanesUnclickable[lane].Enqueue(beat);
        }

        // TODO: right now this just removes the first beat in the lane, but it should probably sort by the correct beatId
        private void RemoveBeat(int lane, string beatId)
        {
            if (terminatingAllBeats)
            {
                cleanupDict[lane] += 1;
                return;
            }

            BeatObject bo;
            if (BeatManager.instance.IsBeatClickable(beatId)) bo = RemoveBeatHelper(lane, beatId, lanesClickable);
            else bo = RemoveBeatHelper(lane, beatId, lanesUnclickable);

            if (bo == null)
            {
                Debug.LogWarning("Failed removing beat " + beatId + " from lane " + lane);
            }
            else BeatManager.instance.ReturnBeatObject(bo);
        }

        private DictValue RemoveBeatHelper<DictValue>(int lane, string beatId, Dictionary<int,Queue<DictValue>> from)
        {
            if (from[lane].Count == 0)
            {
                Debug.LogWarning("Recieved a message to remove " + beatId + " from " + lane + ", but that lane is empty");
                return default;
            }
            return from[lane].Dequeue();
        }
        private void ClickLane(int lane)
        {
            if (lanesClickable[lane].Count == 0)
            {
                EmptyLaneClicked?.Invoke(lane);
                return;
            }

            lanesClickable[lane].Peek().ClickBeat();
        }

        private void RemoveAllBeats()
        {
            terminatingAllBeats = true;
            foreach (var q in lanesClickable.Values)
            {
                foreach (var item in q)
                {
                    item.EarlyTerminate();
                }
            }
            
            foreach(var q in lanesUnclickable.Values)
            {
                foreach(var item in q)
                {
                    item.EarlyTerminate();
                }
            }

            StartCoroutine(WaitForCleanup());
        }

        private IEnumerator WaitForCleanup(float timeout=2f)
        {
            float start = Time.time;
            WaitUntil check = new(() =>
            {
                bool allBeatsDead = true;
                foreach(var keyNumber in cleanupDict)
                {
                    if (lanesClickable[keyNumber.Key].Count + lanesUnclickable[keyNumber.Key].Count == keyNumber.Value) continue;
                    allBeatsDead = false;
                    break;
                }
                return Time.time > start + timeout || allBeatsDead;
            });

            yield return check;

            BeatManager.instance.ResetPools();

            terminatingAllBeats = false;
        }
    }
}