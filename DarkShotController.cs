using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SpaceShooterFinal;



public class DarkShotController : MonoBehaviour, BorgStarDestroyer.IStateObserver
{
    #region Serialized Fields
    [Header("Dark Shot Management")]
    [SerializeField] private List<DarkShot> darkShots;
    #endregion

    #region Private Fields
    private BorgStarDestroyer borgStarDestroyer;
    #endregion

    #region Unity Methods
    private void Start()
    {
        InitializeController();
    }

    private void OnDestroy()
    {
        if (borgStarDestroyer != null)
        {
            borgStarDestroyer.UnregisterObserver(this);
        }
    }
    #endregion

    #region Initialization Methods
    private void InitializeController()
    {
        borgStarDestroyer = FindBorgStarDestroyerInParents(transform);
        if (borgStarDestroyer == null)
        {
            Debug.LogError("BorgStarDestroyer not found in parent objects.");
            return;
        }

        borgStarDestroyer.RegisterObserver(this);
        InitializeDarkShots();
    }

    private void InitializeDarkShots()
    {
        foreach (var darkShot in darkShots)
        {
            darkShot.InitializeWithController(this);
        }
    }

    private BorgStarDestroyer FindBorgStarDestroyerInParents(Transform currentTransform)
    {
        while (currentTransform != null)
        {
            var borgStarDestroyer = currentTransform.GetComponent<BorgStarDestroyer>();
            if (borgStarDestroyer != null)
            {
                return borgStarDestroyer;
            }
            currentTransform = currentTransform.parent;
        }
        return null;
    }
    #endregion

    #region Firing Methods
    public void FireAllDarkShots()
    {
        foreach (var darkShot in darkShots)
        {
            darkShot.TryFireDarkShot();
        }
    }
    #endregion

    #region IStateObserver Implementation
    public void OnStateChange(BorgStarDestroyer.IEnemyState newState)
    {
        foreach (var darkShot in darkShots)
        {
            darkShot.OnStateChange(newState);
        }
    }
    #endregion
}
