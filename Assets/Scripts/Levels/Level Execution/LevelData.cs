using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmLevel
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Levels/LevelData")]
    public class LevelData : ScriptableObject
    {
        // to be extended on later probably or not mayebs
        public float bpm;
        public int numLanes;
        public float noteRushTime; // this gonna be the amount of time it takes from note spawn to when note should be pressed
        public List<BasicLevelEvent> events;
        public AudioClip song; // this probably shouldn't be here lwkey but WHATEVer 
        public int presongTickAmount;
        public List<LevelDataBeatContainmentInfo> maxBeatsOnScreenAtOnce; // beatId, amount
    }

    [System.Serializable]
    public class LevelDataBeatContainmentInfo
    {
        public string beatId;
        public int amount;
    }
}  