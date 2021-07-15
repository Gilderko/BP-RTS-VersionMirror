using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint = null;

    #region Server

    #endregion

    #region Client

    [Client]
    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }

    #endregion


}
