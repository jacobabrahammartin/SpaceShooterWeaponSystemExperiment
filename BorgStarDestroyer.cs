using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SpaceShooterFinal;


public class BorgStarDestroyer : MonoBehaviour, BorgStarDestroyer.IStateObserver
{
    #region Serialized Fields
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float descendTargetY = 8.85f;
    [SerializeField] private float waitDurationAtTarget = 3.0f;

    [Header("Boundary Settings")]
    [SerializeField] private float horizontalBoundary = 11f;

    [Header("Health Settings")]
    [SerializeField] private int health = 200;

    [Header("Patrol Settings")]
    [SerializeField] public PatrolType patrolType = PatrolType.Stationary;
    [SerializeField] public float patrolSpeed = 2f;
    [SerializeField] public float patrolRange = 5f;

    [Header("Weapon Settings")]
    [SerializeField] private DarkShotController darkShotController;
    [SerializeField] public float fireRate = 0.5f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject darkShotPrefab;
    #endregion

    #region Enums
    public enum PatrolType { Stationary, Jitter, SmoothAcrossScreen, SmoothSideToSide, LethargicCreep, GlacialDrift }
    public enum FiringState { Normal, TripleDarkShot, InfiniteTripleDarkShot, InfiniteDarkShot }
    #endregion

    #region Events
    public event Action<FiringState> OnFiringStateChanged;
    public event Action<Transform> OnFirePointActivated;
    #endregion

    #region Private Fields
    private IEnemyState currentState;
    private readonly List<IStateObserver> observers = new List<IStateObserver>();
    private float lastFireTime;
    private bool hasReachedTarget;
    private Vector3 targetPosition;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        targetPosition = new Vector3(transform.position.x, descendTargetY, transform.position.z);
        ChangeState(new PatrolState());
    }

    private void Start()
    {
        StartCoroutine(ApproachTargetPosition());
    }

    private void Update()
    {
        currentState?.Execute(this);
    }

    #region Initialization
    public void Initialize()
    {
        // Set initial health
        health = 200;

        // Set initial patrol type and parameters
        patrolType = PatrolType.Stationary;
        patrolSpeed = 2f;
        patrolRange = 5f;

        // Set initial firing rate
        fireRate = 0.5f;

        // Any other initial setup can be added here
    }
    #endregion
    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }
    #endregion

    #region Movement Methods
    private IEnumerator ApproachTargetPosition()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        hasReachedTarget = true;
        yield return new WaitForSeconds(waitDurationAtTarget);
        hasReachedTarget = false;
        ChangeState(new AttackState());
    }

    public void PatrolBehaviour()
    {
        switch (patrolType)
        {
            case PatrolType.Stationary:
                break;
            case PatrolType.Jitter:
                JitterMovement();
                break;
            case PatrolType.SmoothAcrossScreen:
                SmoothAcrossScreenMovement();
                break;
            case PatrolType.SmoothSideToSide:
                SmoothSideToSideMovement();
                break;
            case PatrolType.LethargicCreep:
                LethargicCreepMovement();
                break;
            case PatrolType.GlacialDrift:
                GlacialDriftMovement();
                break;
        }
    }

    private void JitterMovement()
    {
        float jitterAmount = 0.1f;
        Vector3 jitter = new Vector3(UnityEngine.Random.Range(-jitterAmount, jitterAmount), 0, 0);
        transform.position += jitter;
    }

    private void SmoothAcrossScreenMovement()
    {
        Vector3 targetPosition = new Vector3(Mathf.Sin(Time.time) * horizontalBoundary, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
    }

    private void SmoothSideToSideMovement()
    {
        Vector3 sideMovement = new Vector3(Mathf.Sin(Time.time) * patrolRange, 0, 0);
        transform.position += sideMovement;
    }

    private void LethargicCreepMovement()
    {
        Vector3 creepDirection = new Vector3(0, -patrolSpeed * Time.deltaTime, 0);
        transform.position += creepDirection;
    }

    private void GlacialDriftMovement()
    {
        Vector3 driftDirection = new Vector3(0, -patrolSpeed * Time.deltaTime, 0);
        transform.position += driftDirection;
    }
    #endregion

    #region Firing Methods
    public void HandleWeaponFiring()
    {
        if (Time.time > lastFireTime + 1f / fireRate)
        {
            lastFireTime = Time.time;
            darkShotController.FireAllDarkShots();
            OnFirePointActivated?.Invoke(transform);
        }
    }

    public void NotifyFirePointActivated(Transform firePoint)
    {
        FireWeaponAtPoint(firePoint.position);
    }

    private void FireWeaponAtPoint(Vector3 point)
    {
        GameObject projectile = Instantiate(darkShotPrefab, point, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(Vector3.forward);
            projectileScript.SetSpeed(10f);
            projectileScript.SetDamage(20);
        }
    }

    public void IncreaseAggression()
    {
        fireRate *= 1.5f;
        speed *= 1.2f;
    }

    public void ResetAttackMode()
    {
        fireRate = 0.5f;
        speed = 2.5f;
    }
    #endregion

    #region Collision Handling
    private void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Lazer"))
        {
            TakeDamage(10);
            OnFiringStateChanged?.Invoke(FiringState.Normal);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region State Pattern Implementation
    private void ChangeState(IEnemyState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
        NotifyStateChange();
    }

    private void NotifyStateChange()
    {
        foreach (var observer in observers)
        {
            observer.OnStateChange(currentState);
        }
    }

    public void RegisterObserver(IStateObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void UnregisterObserver(IStateObserver observer)
    {
        observers.Remove(observer);
    }
    #endregion

    #region Patrol State Management
    public void ResetPatrolParameters()
    {
        patrolType = PatrolType.Stationary;
        patrolSpeed = 2f;
        patrolRange = 5f;
    }

    public void SetAggressivePatrolParameters()
    {
        patrolType = PatrolType.SmoothSideToSide;
        patrolSpeed = 3f;
        patrolRange = 7f;
    }

    public void SetFiringState(FiringState state)
    {
        OnFiringStateChanged?.Invoke(state);
    }
    #endregion

    #region State Pattern Interfaces and Classes
    public interface IEnemyState
    {
        void Enter(BorgStarDestroyer borgStarDestroyer);
        void Execute(BorgStarDestroyer borgStarDestroyer);
        void Exit(BorgStarDestroyer borgStarDestroyer);
    }

    public interface IStateObserver
    {
        void OnStateChange(IEnemyState newState);


    }


    public class PatrolState : IEnemyState
    {
        public void Enter(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.ResetPatrolParameters();
        }

        public void Execute(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.PatrolBehaviour();
        }

        public void Exit(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.PrepareForNextState();
        }
    }

    public class NormalState : IEnemyState
    {
        public void Enter(BorgStarDestroyer borgStarDestroyer)
        {
            // Reset the patrol and attack modes to their default settings.
            borgStarDestroyer.ResetPatrolParameters();
            borgStarDestroyer.ResetAttackMode();

            // Additional setup when entering the Normal State, if needed.
            // For example, setting the enemy's appearance to a normal state.
        }

        public void Execute(BorgStarDestroyer borgStarDestroyer)
        {
            // Execute the patrol behavior.
            borgStarDestroyer.PatrolBehaviour();

            // Handle normal weapon firing.
            // This could be a simple shooting mechanism or no shooting at all, depending on your game design.
            borgStarDestroyer.HandleWeaponFiring();

            // You can also add other behaviors typical for the Normal State.
            // For example, checking for player proximity, environmental interactions, etc.
        }

        public void Exit(BorgStarDestroyer borgStarDestroyer)
        {
            // Prepare the enemy for the next state.
            // This could involve resetting certain parameters or setting up new ones for the next state.
            borgStarDestroyer.PrepareForNextState();

            // Any additional cleanup or setup before exiting the Normal State.
            // For example, triggering animations or events that signify state transition.
        }
    }

    public class AttackState : IEnemyState
    {
        public void Enter(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.IncreaseAggression();
        }

        public void Execute(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.HandleWeaponFiring();
        }

        public void Exit(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.ResetAttackMode();
        }
    }

    public class AggressiveState : IEnemyState
    {
        public void Enter(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.SetAggressivePatrolParameters();
            borgStarDestroyer.SetFiringState(FiringState.InfiniteTripleDarkShot);
        }

        public void Execute(BorgStarDestroyer borgStarDestroyer)
        {
            AggressivePatrolBehaviour(borgStarDestroyer);
            borgStarDestroyer.HandleWeaponFiring();
        }

        public void Exit(BorgStarDestroyer borgStarDestroyer)
        {
            borgStarDestroyer.ResetPatrolParameters();
            borgStarDestroyer.ResetAttackMode();
        }

        private void AggressivePatrolBehaviour(BorgStarDestroyer borgStarDestroyer)
        {
            //Vector3 playerPosition = Player.Instance.transform.position;
            //Vector3 directionToPlayer = (playerPosition - borgStarDestroyer.transform.position).normalized;
            float aggressiveSpeed = borgStarDestroyer.patrolSpeed * 1.5f;
            //borgStarDestroyer.transform.position += directionToPlayer * aggressiveSpeed * Time.deltaTime;
        }
    }

    // Additional state implementations as needed

    #endregion

    // Additional methods for PrepareForNextState and other functionalities
    public void PrepareForNextState()
    {
        // Logic to prepare for the next state
    }
}




