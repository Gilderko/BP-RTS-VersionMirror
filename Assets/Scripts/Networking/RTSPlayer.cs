using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private LayerMask buildingBlockLayer = new LayerMask();

    [SerializeField] 
    private Building[] buildings = new Building[0];

    [SerializeField]
    private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))] 
    private int resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    [SerializeField] 
    private List<Unit> myUnits = new List<Unit>();

    [SerializeField] 
    private List<Building> myBuildings = new List<Building>();    

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    [Server]
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    [Server]
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
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
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
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

    [Server]
    public void AddResources(int resourcesToAdd)
    {
        resources += resourcesToAdd;
    }


    [Command]
    public void CmdTryPlaceBuilding(int buildingID, Vector3 positionToSpawn)
    {
        Debug.Log(buildingID);
        Debug.Log(buildings.Count());

        Building buildingToPlace = buildings.First(build => build.GetID() == buildingID);

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();        

        if (!CanPlaceBuilding(buildingCollider, positionToSpawn))
        {
            return;
        }

        GameObject building = Instantiate(buildingToPlace.gameObject, positionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(building, connectionToClient);

        AddResources(-buildingToPlace.GetPrice());
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingDespawned;
    }

    [Client]
    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Add(building);
    }

    [Client]
    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    [Client]
    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        base.OnStartClient();

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingDespawned;
    }

    [Client]
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    [Client]
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    } 

    [Client]
    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResourcesUpdated?.Invoke(newValue);
    }
    #endregion

    public int GetResources()
    {
        return resources;
    }

    public IEnumerable<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public IEnumerable<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 positionToSpawn)
    {
        if (Physics.CheckBox(positionToSpawn + buildingCollider.center,
            buildingCollider.size / 2, Quaternion.identity,
            buildingBlockLayer))
        {
            return false;
        }

        foreach (Building build in myBuildings)
        {
            if ((positionToSpawn - build.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }
}
