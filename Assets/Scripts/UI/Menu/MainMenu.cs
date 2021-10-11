using Mirror;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

#if UNITY_SERVER
    private void Start()
    {
        HostLobbyCallback();
    }
#endif

    public void HostLobbyCallback()
    {
        Debug.Log("Session started");
        landingPagePanel.SetActive(false);

        NetworkManager.singleton.StartServer();
    }
}
