using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab = null;

    #region Server

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var spawner = Instantiate(unitSpawnerPrefab, conn.identity.transform.position,conn.identity.transform.rotation);
        NetworkServer.Spawn(spawner, conn);
    }

    #endregion

    #region Client

    #endregion
}
