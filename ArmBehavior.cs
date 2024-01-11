using UnityEngine;

[CreateAssetMenu(fileName = "DarkShotSettings", menuName = "Weapon/DarkShotSettings")]
public class DarkShotSettings : ScriptableObject
{
    public GameObject darkShotPrefab;
    public float shotSpeed = 8f;
    public float shotLifetime = 5f;
    public float fireRate = 0.5f;
    public float infiniteShotFireRate = 0.05f;
    public float infiniteShotDuration = 5f;
    public float tripleShotAngle = 10f;
    public float tripleShotFireRate = 0.3f; // Added property for Triple Dark Shot fire rate

    // Add other settings as needed
}