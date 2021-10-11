using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;


    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    #region Server

#if UNITY_SERVER
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        NetworkIdentity identity;
        if (!other.TryGetComponent<NetworkIdentity>(out identity))
        {
            return;
        }

        if (identity.connectionToClient == connectionToClient)
        {
            return;
        }

        Health health;
        if (!other.TryGetComponent<Health>(out health))
        {
            return;
        }

        health.DealDamage(damageToDeal);

        DestroySelf();
    }
#endif

    [Server]
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion
}
