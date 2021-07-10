using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent = null;

    private Camera mainCamera;

    #region Server

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

    [Client]
    public override void OnStartAuthority() // Start method for client person who owns this object
    {
        mainCamera = Camera.main;
    }

    [ClientCallback] // Callback that only clients will run this update
    private void Update()
    {
        Debug.Log("moving0");
        if (!hasAuthority)
        { 
            return; 
        }
        Debug.Log("moving1");
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return;
        }

        Debug.Log("moving2");
        CmdMove(hit.point);
    }

    #endregion
}
