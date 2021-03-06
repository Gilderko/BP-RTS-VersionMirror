using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The core component of this game. Created by deriving from the base NetworkManager.
/// 
/// Stores the players and handles logic related to new player connecting and disconnecting + starting the game.
/// </summary>
public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerBase = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;
    [SerializeField] private Commander commander = null;

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;

    public override void Awake()
    {
        base.Awake();
        Application.quitting += DeinitialiseNetwork;
    }

    private void DeinitialiseNetwork()
    {
        if (NetworkServer.active)
        {
            StopServer();
        }
        else
        {
            StopClient();
        }
    }

    #region Server

    [Server]
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) { return; }

        conn.Disconnect();

        base.OnServerConnect(conn);
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    [Server]
    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;

        base.OnStopServer();
    }

    [Server]
    public void StartGame()
    {
        if (Players.Count < 2) { return; }

        isGameInProgress = true;

        ServerChangeScene("Scene_Map");
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Add(player);

        player.SetPlayerName($"Player {Players.Count}");

        player.SetTeamColor(Random.ColorHSV());

        player.SetPartyOwner(Players.Count == 1);
    }

    [Server]
    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            Commander commanderInstance = Instantiate(commander);
            NetworkServer.Spawn(commanderInstance.gameObject);

            foreach (RTSPlayer player in Players)
            {
                GameObject baseInstance = Instantiate(playerBase, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(baseInstance, player.connectionToClient);

                player.ChangeStartingPosition(baseInstance.transform.position);
            }
        }
    }

    #endregion

    #region Client

    [Client]
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    [Client]
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    [Client]
    public override void OnStopClient()
    {
        base.OnStopClient();

        Players.Clear();
    }
    #endregion
}
