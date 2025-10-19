using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance;

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Pool with tag {tag} doesn't exist!");
                return null;
            }

            GameObject obj = poolDictionary[tag].Dequeue();

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            if (parent != null) obj.transform.SetParent(parent);
            obj.SetActive(true);

            poolDictionary[tag].Enqueue(obj); 

            return obj;
        }

        public void ResetAll()
        {
            foreach (var pool in poolDictionary.Values)
            {
                foreach (var obj in pool)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }
        }
    }
}

