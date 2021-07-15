using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private List<Unit> myUnits = new List<Unit>();

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }    

    [Server]
    private void ServerHandleUnitSpawned(Unit unit)
    {
        // Check if the same person who owns this player also owns this unit
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();

        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    [Server]
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartClient()
    {
        if (!isClientOnly)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    [Client]
    public override void OnStopClient()
    {
        if (!isClientOnly)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    [Client]
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (!hasAuthority)
        {
            return;
        }

        myUnits.Add(unit);
    }

    [Client]
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (!hasAuthority)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    [Client]
    public IEnumerable<Unit> GetMyUnits()
    {
        return myUnits;
    }

    #endregion
}
