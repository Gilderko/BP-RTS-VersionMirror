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


    #endregion

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }


}
