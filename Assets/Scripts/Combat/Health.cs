using Mirror;
using System;
using UnityEngine;

/// <summary>
/// Basic health component where all damage dealing is handled on server.
/// </summary>
public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandeHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();

        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionID)
    {
        if (connectionID != connectionToClient.connectionId)
        {
            return;
        }

        DealDamage(currentHealth);
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnDie?.Invoke();
    }

    #endregion

    #region Client

    [Client]
    private void HandeHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated(newHealth, maxHealth);
    }

    #endregion

}
