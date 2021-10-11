using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    #region Server

    [Server]
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    [Server]
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    [Server]
    public Targetable GetTarget()
    {
        return target;
    }

    #endregion

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        Targetable newTarget;
        if (!targetGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            return;
        }

        target = newTarget;
    }
}
