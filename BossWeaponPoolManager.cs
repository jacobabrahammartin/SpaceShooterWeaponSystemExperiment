using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPooledObject
{
    void OnObjectSpawn();
}

namespace SpaceShooterFinal
{
    public class BossWeaponPoolManager : MonoBehaviour
    {
        #region Singleton
        public static BossWeaponPoolManager Instance { get; private set; }
        #endregion

        #region Serialized Fields
        [System.Serializable]
        public class Pool
        {
            public AssetReference prefabReference;
            public int size;
        }

        [SerializeField] private List<Pool> pools;
        #endregion

        #region Private Fields
        private Dictionary<AssetReference, Queue<GameObject>> poolDictionary;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                StartCoroutine(InitializeAllPoolsCoroutine());
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Pool Initialization Coroutine
        private IEnumerator InitializeAllPoolsCoroutine()
        {
            Task initializePoolsTask = InitializeAllPools();
            while (!initializePoolsTask.IsCompleted)
            {
                yield return null;
            }

            if (initializePoolsTask.IsFaulted)
            {
                Debug.LogError(initializePoolsTask.Exception);
            }
        }
        #endregion

        #region Pool Initialization
        private async Task InitializeAllPools()
        {
            poolDictionary = new Dictionary<AssetReference, Queue<GameObject>>();
            List<Task> poolInitializationTasks = new List<Task>();

            foreach (Pool pool in pools)
            {
                poolInitializationTasks.Add(InitializePoolAsync(pool.prefabReference, pool.size));
            }

            await Task.WhenAll(poolInitializationTasks);
        }

        private async Task InitializePoolAsync(AssetReference prefabReference, int size)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = await InstantiatePrefabAsync(prefabReference);
                if (obj != null)
                {
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            }

            poolDictionary[prefabReference] = objectPool;
        }

        private async Task<GameObject> InstantiatePrefabAsync(AssetReference prefabReference)
        {
            AsyncOperationHandle<GameObject> handle = prefabReference.InstantiateAsync();
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to instantiate prefab: {prefabReference.RuntimeKey}");
                return null;
            }
        }
        #endregion

        #region Pool Management
        public async Task<GameObject> SpawnFromPool(AssetReference prefabReference, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.TryGetValue(prefabReference, out var objectPool))
            {
                await InitializePoolAsync(prefabReference, 1);
                objectPool = poolDictionary[prefabReference];
            }

            if (objectPool.Count == 0)
            {
                GameObject newObj = await InstantiatePrefabAsync(prefabReference);
                objectPool.Enqueue(newObj);
            }

            GameObject objectToSpawn = objectPool.Dequeue();
            ResetObjectState(objectToSpawn, position, rotation);
            objectToSpawn.SetActive(true);

            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            pooledObj?.OnObjectSpawn();

            return objectToSpawn;
        }

        public void ReturnToPool(AssetReference prefabReference, GameObject objectToReturn)
        {
            if (!poolDictionary.TryGetValue(prefabReference, out var objectPool))
            {
                var initTask = InitializePoolAsync(prefabReference, 1);
                initTask.Wait();
                objectPool = poolDictionary[prefabReference];
            }

            ResetObjectState(objectToReturn, Vector3.zero, Quaternion.identity);
            objectToReturn.SetActive(false);
            objectPool.Enqueue(objectToReturn);
        }
        #endregion

        #region Helper Methods
        private void ResetObjectState(GameObject obj, Vector3 position, Quaternion rotation)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            // Additional component resets can be done here
        }
        #endregion
    }
}

