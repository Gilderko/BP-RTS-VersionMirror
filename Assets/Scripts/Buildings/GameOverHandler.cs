using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    private List<UnitBase> bases = new List<UnitBase>();

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        UnitBase.ServerOnBaseSpawnend += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        UnitBase.ServerOnBaseSpawnend -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        Debug.Log("Base added");
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        Debug.Log($"Base removed base count {bases.Count}");

        if (bases.Count != 1)
        {
            return;
        }

        Debug.Log("Game Over");
    }

    #endregion

    #region Client

    #endregion
}
