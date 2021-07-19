using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawnend;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public static event Action<int> ServerOnPlayerDie;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        health.ServerOnDie += ServerHandleDeath;
        ServerOnBaseSpawnend?.Invoke(this);
    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();        
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDeath;
    }

    #endregion

    #region Client

    #endregion
}
