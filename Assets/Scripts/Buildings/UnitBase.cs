using Mirror;
using System;
using UnityEngine;

/// <summary>
/// The base for every player. Fires the event when it gets destroyed.
/// </summary>
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
    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= ServerHandleDeath;
        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }
    #endregion
}
