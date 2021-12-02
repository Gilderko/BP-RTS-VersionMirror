using Mirror;
using TMPro;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;


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


    private void ClientHandleResourcesUpdated(int obj)
    {
        resourcesText.text = $"Gold {obj.ToString()}";
    }
}
