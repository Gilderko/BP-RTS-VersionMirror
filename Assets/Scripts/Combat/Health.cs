using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount,0, maxHealth);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }

    #endregion

    #region Client

    private void HandeHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated(newHealth,maxHealth);
    }

    #endregion
}
