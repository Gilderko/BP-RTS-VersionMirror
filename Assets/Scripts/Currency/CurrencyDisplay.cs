using Mirror;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the ammount of resources we have locally in UI.
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;

#if !UNITY_SERVER
    private void Start()
    {
        if (NetworkClient.connection != null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            ClientHandleResourcesUpdated(player.GetResources());            
        }        
    }

    private void OnDestroy()
    {
        if (NetworkClient.connection != null)
        {
            player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }
    }

#endif

    private void ClientHandleResourcesUpdated(int obj)
    {
        resourcesText.text = $"Gold {obj.ToString()}";
    }
}
