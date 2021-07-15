using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent = null;

    #region Server

    [ServerCallback]
    private void Update()
    {        
        if (!agent.hasPath || agent.remainingDistance >= agent.stoppingDistance) 
        { 
            return; 
        }
        
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(position);
    }

    #endregion

    #region Client    

    #endregion
}
