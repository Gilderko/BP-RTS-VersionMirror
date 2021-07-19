using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int buildingID = -1;
    [SerializeField] private int price = 100;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetID()
    {
        return buildingID;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnBuildingSpawned?.Invoke(this);
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerOnBuildingDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    [Client]
    public override void OnStopClient()
    {    
        if (!hasAuthority)
        {
            return;
        }

        base.OnStopClient();

        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    #endregion


}
