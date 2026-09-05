using System;
using System.Collections.Generic;
using UnityEngine;

    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;             // Tên nhận diện pool (VD: "WoodChipFX")
            public GameObject prefab;      // Prefab cần nhân bản
            public int size = 10;          // Số lượng khởi tạo ban đầu
            public bool shouldExpand = true; // Tự tăng số lượng nếu dùng hết
        }

        [Header("--- POOLS CONFIGURATION ---")]
        [SerializeField] private List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, Pool> poolLookup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitPools();
        }

        /// <summary>
        /// Khởi tạo toàn bộ các Pool đã khai báo
        /// </summary>
        private void InitPools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            poolLookup = new Dictionary<string, Pool>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                // Tạo GameObject cha gom nhóm cho gọn Hierarchy
                GameObject parentContainer = new GameObject($"[Pool] {pool.tag}");
                parentContainer.transform.SetParent(transform);

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, parentContainer.transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
                poolLookup.Add(pool.tag, pool);
            }
        }

        /// <summary>
        /// Lấy một Object ra từ Pool và đặt vào vị trí mong muốn
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[ObjectPooler] Không tìm thấy Pool với Tag: '{tag}'!");
                return null;
            }

            Queue<GameObject> objectPool = poolDictionary[tag];

            // Nếu các object trong Pool đều đang active và cho phép tự nở (Expand)
            if (objectPool.Count == 0 || objectPool.Peek().activeSelf)
            {
                Pool pool = poolLookup[tag];
                if (pool.shouldExpand)
                {
                    Transform parentContainer = transform.Find($"[Pool] {tag}");
                    GameObject newObj = Instantiate(pool.prefab, parentContainer != null ? parentContainer : transform);
                    newObj.SetActive(false);
                    objectPool.Enqueue(newObj);
                }
                else
                {
                    Debug.LogWarning($"[ObjectPooler] Pool '{tag}' đã hết và không cho phép mở rộng!");
                    return null;
                }
            }

            GameObject objectToSpawn = objectPool.Dequeue();

            // Nếu truyền Parent 
            if (parent != null)
            {
                objectToSpawn.transform.SetParent(parent, false);
            }

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            // Đưa lại vào cuối Queue để xoay vòng tái sử dụng
            objectPool.Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        /// <summary>
        /// Hàm Generic hỗ trợ lấy nhanh Component (VD: UI Particle Effect)
        /// </summary>
        public T SpawnFromPool<T>(string tag, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            GameObject spawnedObj = SpawnFromPool(tag, position, rotation, parent);
            if (spawnedObj != null)
            {
                return spawnedObj.GetComponent<T>();
            }
            return null;
        }
    }
