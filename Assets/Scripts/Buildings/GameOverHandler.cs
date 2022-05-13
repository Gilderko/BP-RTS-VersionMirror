using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles what happens when the last player base is despawned. Can include logic specific for both the server and the client.
/// </summary>
public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;


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
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1)
        {
            return;
        }

        int winnerIndex = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {winnerIndex.ToString()}");

        ServerOnGameOver?.Invoke();

        StartCoroutine(DelayedQuit(5));
    }

    private IEnumerator DelayedQuit(float timer)
    {
        yield return new WaitForSeconds(timer);

        Application.Quit();
    }

    #endregion


    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
