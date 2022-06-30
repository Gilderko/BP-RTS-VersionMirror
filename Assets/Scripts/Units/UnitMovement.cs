using Mirror;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Takes care of setting the path of the NavMeshAgent on the server bases on the target or the command to move to a certain place.
/// </summary>
public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    [Server]
    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

#if UNITY_SERVER

    [ServerCallback]
    private void Update()
    {
        if (isServer)
        {
            Targetable target = targeter.GetTarget();

            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(targeter.GetTarget().transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
            }
            else
            {
                if (!agent.hasPath || agent.remainingDistance >= agent.stoppingDistance)
                {
                    return;
                }

                agent.ResetPath();
            }
        }
    }

#endif

    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(position);
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    #endregion
}
