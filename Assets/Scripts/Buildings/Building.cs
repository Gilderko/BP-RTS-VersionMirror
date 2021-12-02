using Mirror;
using System;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int buildingID = -1;
    [SerializeField] private int price = 100;
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private string buildingName = "";

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

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

    public string GetName()
    {
        return buildingName;
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
