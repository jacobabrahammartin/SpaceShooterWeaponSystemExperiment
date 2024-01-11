using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpaceShooterFinal
{
    public class DarkShot : MonoBehaviour, BorgStarDestroyer.IStateObserver
    {
        #region Serialized Fields
        [Header("Settings")]
        [SerializeField] private DarkShotSettings settings;
        [SerializeField] private AssetReference projectilePrefabReference;

        [Header("Fire Points")]
        [SerializeField] private Transform defaultFirePoint;
        [SerializeField] private Transform firePointLeft;
        [SerializeField] private Transform firePointRight;
        #endregion

        #region Private Fields
        private BossWeaponPoolManager bossWeaponPoolManager;
        private float nextFireTime;
        private IFiringStrategy firingStrategy;
        private BorgStarDestroyer borgStarDestroyer;
        private DarkShotController darkShotController;
        private EnemyWeaponManager enemyWeaponManager;
        private List<Transform> firePoints;
        private Coroutine infiniteShotRoutine;
        private bool isFiringEnabled = true;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            StartCoroutine(DelayedInitialization());
        }

        private IEnumerator DelayedInitialization()
        {
            while (GetComponentInParent<BorgStarDestroyer>() == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
            InitializeComponents();
        }

        private void Update()
        {
            HandleFiring();
        }

        private void OnDestroy()
        {
            Cleanup();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            bossWeaponPoolManager = BossWeaponPoolManager.Instance;
            borgStarDestroyer = GetComponentInParent<BorgStarDestroyer>();
            darkShotController = GetComponentInParent<DarkShotController>();
            enemyWeaponManager = GetComponentInParent<EnemyWeaponManager>();

            ValidateComponents();
            RegisterAsObserver();
            InitializeFirePoints();
            SetFiringStrategy(new NormalFiringStrategy());
        }

        private void ValidateComponents()
        {
            if (borgStarDestroyer == null || darkShotController == null || enemyWeaponManager == null)
            {
                Debug.LogError("Required component(s) not found in parent.");
            }
        }

        private void RegisterAsObserver()
        {
            borgStarDestroyer?.RegisterObserver(this);
        }

        private void InitializeFirePoints()
        {
            firePoints = new List<Transform> { defaultFirePoint, firePointLeft, firePointRight };
        }
        #endregion

        #region Firing Logic
        // ... [Firing Logic methods unchanged]
        #endregion

        #region State Observation
        // ... [State Observation methods unchanged]
        #endregion

        #region Controller Integration
        public void InitializeWithController(DarkShotController controller)
        {
            darkShotController = controller;
        }

        public void TryFireDarkShot()
        {
            if (isFiringEnabled)
            {
                FireDarkShot(defaultFirePoint);
            }
        }
        #endregion

        #region Cleanup
        private void Cleanup()
        {
            borgStarDestroyer?.UnregisterObserver(this);
        }
        #endregion

        #region Custom Methods
        public void ToggleFiring(bool enable)
        {
            isFiringEnabled = enable;
        }

        public void AdjustFireRate(float newRate)
        {
            settings.fireRate = Mathf.Max(newRate, 0.1f);
        }

        public Transform GetRandomFirePoint()
        {
            if (firePoints == null || firePoints.Count == 0)
            {
                Debug.LogWarning("No fire points available.");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, firePoints.Count);
            return firePoints[randomIndex];
        }

        public void AssignFirePoints(List<Transform> newFirePoints)
        {
            if (newFirePoints == null || newFirePoints.Count == 0)
            {
                Debug.LogError("AssignFirePoints called with null or empty list.");
                return;
            }

            firePoints = newFirePoints;
        }

        public void ActivateFirePoint(string firePointName, bool isActive)
        {
            Transform firePoint = firePoints.Find(fp => fp.name == firePointName);
            if (firePoint != null)
            {
                firePoint.gameObject.SetActive(isActive);
            }
            else
            {
                Debug.LogWarning($"Fire point '{firePointName}' not found.");
            }
        }
        #endregion
    }
}
