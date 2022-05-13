using Mirror;
using UnityEngine;

/// <summary>
/// Generates resources for the player that spawned it with certain interval and ammount per interval. Resource generation happens on the server.
/// </summary>
public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    #region Server
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

#if UNITY_SERVER

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            player.AddResources(resourcesPerInterval);
            timer += interval;
        }
    }

#endif

#endregion
}
