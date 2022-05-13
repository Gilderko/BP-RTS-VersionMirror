using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base component to all units. Includes references to all other components such as UnitMovement, Targeter and Health.
/// 
/// Includes events for unit selection that enable the sprite to show that unit is selected.
/// </summary>
public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private Health health = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    [SerializeField] private int resourceCost = 5;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    [Server]
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    [Client]
    public override void OnStopClient()
    {
        if (!hasAuthority)
        {
            return;
        }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }

        onSelected.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }

        onDeselected.Invoke();
    }

    #endregion

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    public int GetResourceCost()
    {
        return resourceCost;
    }
}
