using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Commander : NetworkBehaviour
{
    [SerializeField] private Building oilPump;
    [SerializeField] private Building tankSpawner;

    private Controls inputActions;
    private RTSPlayer localPlayer;

    private UnitSelectionHandler selectionHandler;
    private UnitCommandGiver commandGiver;

    private List<GameObject> waypoints;

    private void Start()
    {
        selectionHandler = FindObjectOfType<UnitSelectionHandler>();
        commandGiver = FindObjectOfType<UnitCommandGiver>();
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").ToList();

        inputActions = new Controls();

        inputActions.Player.BuildMine.performed += SendCommandBuildMine;
        inputActions.Player.BuildSpawner.performed += SendCommandBuildSpawner;
        inputActions.Player.SpawnUnit.performed += SendCommandSpawnUnit;
        inputActions.Player.SendUnits.performed += SendCommandSendUnits;
        
        if (isClient)
        {
            var players = (NetworkManager.singleton as RTSNetworkManager).Players;
            localPlayer = players.First(player => player.isLocalPlayer);
        }

        inputActions.Enable();
    }

    private void SendCommandBuildSpawner(InputAction.CallbackContext obj)
    {
        CmdTellClientsToBuild(tankSpawner.GetID());
    }

    private void SendCommandBuildMine(InputAction.CallbackContext obj)
    {
        CmdTellClientsToBuild(oilPump.GetID());
    }

    [Command(requiresAuthority = false)]
    private void CmdTellClientsToBuild(int buildingId)
    {
        RpcClientsBuild(buildingId);
    }

    [ClientRpc]
    private void RpcClientsBuild(int buildingId)
    {
        StartCoroutine(Building(buildingId));
    }

    public IEnumerator Building(int buildingId)
    {
        Building spawnBase = localPlayer.GetMyBuildings().First();

        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            Vector3 samplePos = spawnBase.transform.position + UnityEngine.Random.insideUnitSphere * 25f;
            if (!NavMesh.SamplePosition(samplePos, out NavMeshHit hit, 25f, NavMesh.AllAreas))
            {
                continue;
            }

            if (!localPlayer.CanPlaceBuilding(oilPump.GetComponent<BoxCollider>(), hit.position))
            {
                continue;
            }

            localPlayer.MustPlaceBuilding(buildingId, hit.position);
            break;
        }
    }

    private void SendCommandSpawnUnit(InputAction.CallbackContext obj)
    {
        CmdTellClientsToSpawnUnits();
    }

    [Command(requiresAuthority = false)]
    private void CmdTellClientsToSpawnUnits()
    {
        RpcClientsSpawnUnit();
    }

    [ClientRpc]
    private void RpcClientsSpawnUnit()
    {
        MakingUnit();
    }

    private void MakingUnit()
    {
        var spawners = localPlayer.GetMyBuildings().Where(build => build.GetComponent<UnitSpawner>() != null);

        var spawnIndex = UnityEngine.Random.Range(0, spawners.Count());

        spawners.ElementAt(spawnIndex).GetComponent<UnitSpawner>().CmdSpawnUnit();
    }

    private void SendCommandSendUnits(InputAction.CallbackContext obj)
    {
        CmdTellClientsToMoveUnits();
    }

    [Command(requiresAuthority = false)]
    private void CmdTellClientsToMoveUnits()
    {
        RpcClientsSendUnits();
    }

    [ClientRpc]
    private void RpcClientsSendUnits()
    {
        var playerUnits = localPlayer.GetMyUnits();
        
        if (playerUnits.Count() == 0)
        {
            return;
        }

        var averagePoint = playerUnits.First().transform.position;

        foreach (var unit in playerUnits)
        {
            selectionHandler.AddUnitToSelection(unit);
            averagePoint += unit.transform.position;
            averagePoint /= 2;
        }

        var furthest = new Tuple<GameObject,float>(waypoints[0],-1f);
        
        foreach (var waypoint in waypoints)
        {
            var currDist = Vector3.Distance(averagePoint, waypoint.transform.position);

            if (currDist > furthest.Item2)
            {
                furthest = new Tuple<GameObject, float>(waypoint, currDist);
            }
        }

        if (!NavMesh.SamplePosition(furthest.Item1.transform.position, out NavMeshHit hit, 25f, NavMesh.AllAreas))
        {
            return;
        }      

        commandGiver.TryMove(hit.position);
    }
}
