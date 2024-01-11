using System.Collections;
using UnityEngine;

namespace SpaceShooterFinal
{
    public class BossSpawner : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Boss Prefabs")]
        [SerializeField] private GameObject borgStarDestroyerPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private float bossSpawnRate = 5.0f;
        [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10f, 2f);
        [SerializeField] private Vector2 spawnAreaMax = new Vector2(10f, 5f);
        [SerializeField] private Vector3 fixedSpawnPosition = new Vector3(-2.15f, 21.21f, 0f);

        [Header("Spawn Management")]
        [SerializeField] private GameObject bossContainer;
        [SerializeField] private bool spawnBoss = false;

        #endregion

        #region Private Fields

        private bool startSpawning = false;

        #endregion

        #region Unity Lifecycle Methods

        private void Start()
        {
            GameManager.Instance.OnEnemyShipDestroyed += CheckForBossSpawn;
            StartCoroutine(SpawnBossRoutine());
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyShipDestroyed -= CheckForBossSpawn;
            }
        }

        #endregion

        #region Spawning Methods

        private IEnumerator SpawnBossRoutine()
        {
            while (true)
            {
                if (startSpawning && spawnBoss)
                {
                    SpawnBoss();
                    yield return new WaitForSeconds(bossSpawnRate);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void SpawnBoss()
        {
            Vector3 spawnPosition = fixedSpawnPosition;
            GameObject spawnedBoss = CreateBoss(BossType.BorgStarDestroyer);
            spawnedBoss.transform.position = spawnPosition;
            spawnedBoss.transform.parent = bossContainer.transform;

            InitializeBoss(spawnedBoss);
        }

        private GameObject CreateBoss(BossType type)
        {
            switch (type)
            {
                case BossType.BorgStarDestroyer:
                    return Instantiate(borgStarDestroyerPrefab);
                default:
                    throw new System.ArgumentException("Invalid boss type");
            }
        }

        #endregion

        #region Initialization Methods

        public void InitializeBoss(GameObject boss)
        {
            BorgStarDestroyer bossScript = boss.GetComponent<BorgStarDestroyer>();
            if (bossScript != null)
            {
                // Call the Initialize method on the BorgStarDestroyer script
                bossScript.Initialize(); // Make sure there is an Initialize method in BorgStarDestroyer
            }
            else
            {
                Debug.LogError("BorgStarDestroyer script not found on the spawned boss prefab");
            }

            // Initialize DarkShot components in child GameObjects
            DarkShot[] darkShots = boss.GetComponentsInChildren<DarkShot>();
            foreach (var darkShot in darkShots)
            {
                darkShot.InitializeComponents();
            }
        }


        #endregion

        #region Game Event Methods

        private void CheckForBossSpawn(int destroyedEnemyCount)
        {
            if (destroyedEnemyCount >= GameManager.Instance.enemiesToSpawnBoss)
            {
                startSpawning = true;
            }
        }

        #endregion

        #region Public Control Methods

        public void ToggleSpawning(bool shouldSpawn)
        {
            spawnBoss = shouldSpawn;
        }

        public void InstantSpawnBoss()
        {
            SpawnBoss();
        }

        #endregion

        #region Boss Types Enum

        private enum BossType
        {
            BorgStarDestroyer,
            // Add other boss types here
        }

        #endregion
    }
}
