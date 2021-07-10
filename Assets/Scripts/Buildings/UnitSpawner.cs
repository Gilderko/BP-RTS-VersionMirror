using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;

    #region Server

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject spawnedUnit = Instantiate(unitPrefab, spawnLocation.position, spawnLocation.rotation);

        NetworkServer.Spawn(spawnedUnit, connectionToClient);
    }    

    #endregion

    #region Client

    [Client]
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }

        CmdSpawnUnit();
    }

    #endregion
}
