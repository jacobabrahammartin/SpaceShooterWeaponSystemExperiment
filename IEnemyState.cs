using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface IEnemyState
{
    void Enter(BorgStarDestroyer borgStarDestroyer);
    void Execute(BorgStarDestroyer borgStarDestroyer);
    void Exit(BorgStarDestroyer borgStarDestroyer);
}


public class NormalState : IEnemyState
{
    public void Enter(BorgStarDestroyer borgStarDestroyer)
    {
        // Reset to normal behavior parameters
        borgStarDestroyer.ResetPatrolParameters();
        borgStarDestroyer.ResetAttackMode();
    }

    public void Execute(BorgStarDestroyer borgStarDestroyer)
    {
        // Perform normal patrol behavior
        borgStarDestroyer.PatrolBehaviour();

        // Handle normal weapon firing
        borgStarDestroyer.HandleWeaponFiring();
    }

    public void Exit(BorgStarDestroyer borgStarDestroyer)
    {
        // Prepare for the next state
        borgStarDestroyer.PrepareForNextState();
    }



 /*public class AggressiveState : IEnemyState
{
    public void Enter(BorgStarDestroyer borgStarDestroyer)
    {
        // Set aggressive patrol parameters
        borgStarDestroyer.SetAggressivePatrolParameters();

        // Set aggressive firing mode
        borgStarDestroyer.SetFiringState(BorgStarDestroyer.FiringState.InfiniteTripleDarkShot);
    }

    public void Execute(BorgStarDestroyer borgStarDestroyer)
    {
        // Perform aggressive patrol behavior
        //AggressivePatrolBehaviour(borgStarDestroyer);

        // Handle aggressive weapon firing
        borgStarDestroyer.HandleWeaponFiring();
    }

    public void Exit(BorgStarDestroyer borgStarDestroyer)
    {
        // Reset to normal behavior parameters
        borgStarDestroyer.ResetPatrolParameters();
        borgStarDestroyer.ResetAttackMode();
    }

    /*private void AggressivePatrolBehaviour(BorgStarDestroyer borgStarDestroyer)
    {
        // Implement aggressive patrol behavior here
        // Example: Move towards the player more directly and faster
        Vector3 playerPosition = Player.Instance.transform.position; // Assuming you have a Player singleton
        Vector3 directionToPlayer = (playerPosition - borgStarDestroyer.transform.position).normalized;
        float aggressiveSpeed = borgStarDestroyer.patrolSpeed * 1.5f; // Increase the speed for aggressive behavior
        borgStarDestroyer.transform.position += directionToPlayer * aggressiveSpeed * Time.deltaTime;
    }*/
}


// Add other state implementations here...
