using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace SpaceShooterFinal
{
    public class WeaponManager : MonoBehaviour
    {
        #region Singleton Pattern
        public static WeaponManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Serialized Fields
        [Header("Weapon Prefabs")]
        [SerializeField] private AssetReference darkShotPrefab;

        [Header("Dependencies")]
        [SerializeField] private BossWeaponPoolManager bossWeaponPoolManager;
        #endregion

        #region Public Methods
        public async Task<GameObject> GetProjectileAsync()
        {
            if (bossWeaponPoolManager == null || darkShotPrefab == null)
            {
                Debug.LogError("Cannot get projectile: Pool Manager or Prefab is null.");
                return null;
            }

            return await bossWeaponPoolManager.SpawnFromPool(darkShotPrefab, Vector3.zero, Quaternion.identity);
        }

        public void ReturnProjectile(GameObject projectile)
        {
            if (bossWeaponPoolManager == null || darkShotPrefab == null || projectile == null)
            {
                Debug.LogError("Cannot return projectile: Invalid parameters.");
                return;
            }

            bossWeaponPoolManager.ReturnToPool(darkShotPrefab, projectile);
        }
        #endregion
    }
}