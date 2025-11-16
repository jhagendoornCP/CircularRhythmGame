using System;
using System.Collections.Generic;
using NUnit.Framework;
using RhythmLevel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace RhythmBeatObjects
{
    public class BeatManager : MonoBehaviour
    {
        public static BeatManager instance;

        [SerializeField] private List<BeatObject> allBeatObjects;

        private Dictionary<string, Queue<BeatObject>> availableBeats = new();
        private Dictionary<BeatObject, string> unavailableBeats = new();
        private Dictionary<string, bool> isBeatClickable = new();
        private const int INSTANTIATION_OFFSET = 20;

        void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }


        // this function will set up pools for the beat objects that actually exist in the game
        public void InitializeObjects(List<LevelDataBeatContainmentInfo> containsBeats)
        {
            foreach (var info in containsBeats)
            {
                if (availableBeats.ContainsKey(info.beatId))
                {
                    Debug.LogWarning("Duplicate key in LevelDataBeatContainmentInfo: " + info.beatId);
                    continue;
                }
                BeatObject bObj = allBeatObjects.Find(i => i.beatId == info.beatId);
                if (bObj == null)
                {
                    Debug.LogWarning($"Could not find beatId {info.beatId} in the known beat objects");
                    continue;
                }

                Queue<BeatObject> instantiatedBeatObjects = new();
                Vector2 spawnLoc = Vector2.one * INSTANTIATION_OFFSET;
                for (int i = 0; i < info.amount; i++)
                {
                    GameObject newObj = Instantiate(bObj.gameObject, spawnLoc, Quaternion.identity);
                    newObj.SetActive(false);
                    instantiatedBeatObjects.Enqueue(newObj.GetComponent<BeatObject>());
                }

                availableBeats[info.beatId] = instantiatedBeatObjects;
                isBeatClickable[info.beatId] = instantiatedBeatObjects.Peek().TryGetComponent<ClickableBeat>(out _);
            }
        }

        public Tuple<BeatObject, bool> GetBeatObject(string which)
        {
            if (availableBeats[which].Count == 0)
            {
                Debug.LogWarning("Queue does not have enough beats of " + which);
                return null; // this should probably isntantiatnea a new object and add it to the queue
            }

            BeatObject obj = availableBeats[which].Dequeue();
            obj.gameObject.SetActive(true);
            unavailableBeats[obj] = which;

            return new(obj, isBeatClickable[which]);
        }

        public void ReturnBeatObject(BeatObject which)
        {
            if (!unavailableBeats.ContainsKey(which))
            {
                Debug.LogWarning("ReturnBeatObject was passed a BeatObject not found in the Unavailable dictionary.");
                return;
            }
            which.gameObject.SetActive(false);
            string beatId = unavailableBeats[which];
            unavailableBeats.Remove(which);
            availableBeats[beatId].Enqueue(which);
        }

        public bool IsBeatClickable(string which) => isBeatClickable[which];

        public void ResetPools()
        {
            BeatObject[] toRemove = new BeatObject[unavailableBeats.Keys.Count];
            unavailableBeats.Keys.CopyTo(toRemove, 0);
            foreach(BeatObject beat in toRemove) ReturnBeatObject(beat);
        }
    }
}