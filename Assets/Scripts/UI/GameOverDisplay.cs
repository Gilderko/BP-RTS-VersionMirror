using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIParent = null;
    [SerializeField] private TextMeshProUGUI winnerText = null;

#if !UNITY_SERVER

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

#endif

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopServer();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string playerName)
    {
        winnerText.text = $"{playerName} has won the game!";
        gameOverUIParent.SetActive(true);
    }
}
