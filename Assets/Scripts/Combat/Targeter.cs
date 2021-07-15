using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    #region Server
    
    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        Targetable newTarget;
        Debug.Log("setting target");
        if (!targetGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            return;
        }

        target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    #region Client

    #endregion

    public Targetable GetTarget()
    {
        return target;
    }

}
