using Mirror;
using UnityEngine;

/// <summary>
/// Needs to be added to every unit and building to distinguish their color. Color can be changed during the match and will be updated automatically.
/// </summary>
public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();

    #region Server

    [Server]
    public override void OnStartServer()
    {
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        teamColor = player.GetTeamColor();

        base.OnStartServer();
    }

    #endregion

    #region Client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach (Renderer render in colorRenderers)
        {
            foreach (Material material in render.materials)
            {
                material.SetColor("_BaseColor", newColor);
            }
        }
    }

    #endregion
}
