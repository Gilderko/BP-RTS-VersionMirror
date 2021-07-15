using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;    

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
    }

    [Server]
    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartClient()
    {
        if (!hasAuthority || !isClientOnly)
        {
            return;
        }

        AuthorityOnUnitSpawned(this);
    }

    [Client]
    public override void OnStopClient()
    {
        if (!hasAuthority || !isClientOnly)
        {
            return;
        }

        AuthorityOnUnitDespawned(this);
    }

    [Client]
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    [Client]
    public Targeter GetTargeter()
    {
        return targeter;
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
}
