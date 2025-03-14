using UnityEngine;

public class LobbyActions : MonoBehaviour
{
    public void quitLobby()
    {
        LobbyManager.sendWebSocketMessage(new PlayerLeft(LobbyManager.getUsername()));
    }

    public void startGame()
    {
        LobbyManager.sendWebSocketMessage(new StartGame());
    }
}
