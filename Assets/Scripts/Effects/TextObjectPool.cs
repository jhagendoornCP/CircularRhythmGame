using System.Collections.Generic;
using UnityEngine;

namespace RhythmEffects
{
    public class TextObjectPool : EffectPool
    {
        [SerializeField] private GameObject prefabWithTextEffect;
        [SerializeField] private int poolSize = 10;

        private readonly Queue<GameObject> pool = new();

        void Awake()
        {
            // Pre-instantiate dust objects
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefabWithTextEffect);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        private void SpawnText(Vector3 position)
        {
            if (pool.Count == 0)
            {
                Debug.LogWarning("Particle pool empty! Consider increasing pool size.");
                return;
            }

            GameObject text = pool.Dequeue();
            text.transform.position = position;
            text.SetActive(true);
            text.GetComponent<TextEffect>().RunEffect(ReturnToPool);
        }

        public void ReturnToPool(GameObject which)
        {
            which.SetActive(false);
            pool.Enqueue(which);
        }

        public override void PlayEffect(Vector3 where)
        {
            SpawnText(where);
        }
    }
}