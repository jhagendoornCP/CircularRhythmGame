using System.Collections.Generic;
using UnityEngine;

namespace RhythmEffects
{
    public class EffectRequestManager : MonoBehaviour
    {
        public static EffectRequestManager instance;
        [SerializeField] List<KeyedValue<string, EffectPool>> pools;

        private Dictionary<string, EffectPool> keyedPools = new();

        void Awake()
        {
            if (instance != null) Destroy(gameObject);
            else
            {
                instance = this;
                foreach (var kv in pools)
                {
                    GameObject poolClone = Instantiate(kv.value.gameObject);
                    keyedPools.Add(kv.key, poolClone.GetComponent<EffectPool>());
                }
            }
        }

        public void RequestEffect(string which, Vector3 where)
        {
            if (!keyedPools.ContainsKey(which))
            {
                Debug.LogWarning("Requested nonexistance keyed pool: " + which);
                return;
            }

            keyedPools[which].PlayEffect(where);
        }
    }
}