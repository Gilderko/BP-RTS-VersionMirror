using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;

    [SerializeField] private UnityEvent onDeselected = null;

    [SerializeField] private UnitMovement unitMovement = null;

    #region Client

    [Client]
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }

        onSelected.Invoke();
    }

    [Client] 
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }

        onDeselected.Invoke();
    }

    #endregion

    #region Server

    #endregion
}
