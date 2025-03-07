using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager: MonoBehaviour
{
    private static LobbyManager instance = null;
    private WebSocket websocket = null;
    private string lobbyKey = "";
    private string username = "";
    private List<string> players = new List<string>(); 
  
    // Start is called before the first frame update
    public async void join(string lobby, string username)
    {
        instance = this;
        Debug.Log($"'{lobbyKey}'");
        var cleanedLobby = lobby.Trim().Replace("\u200B", "");
        var cleanedUsername = username.Trim().Replace("\u200B", "");
        lobbyKey = cleanedLobby;
        this.username = cleanedUsername;
        var url = $"ws://{Vault.url}/lobby/{cleanedLobby}/{cleanedUsername}";
        Debug.Log($"'{url}'");
      
        websocket = new WebSocket(url);
    
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };
    
        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };
    
        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };
    
        websocket.OnMessage += (bytes) =>
        {
            onMessageReceived(System.Text.Encoding.Default.GetString(bytes));
              //Debug.Log("OnMessage!");
              //Debug.Log(bytes);
        
              // getting the message as a string
               //var message = System.Text.Encoding.UTF8.GetString(bytes);
               //Debug.Log("OnMessage! " + message);
        };
    
        // waiting for messages
        await websocket.Connect();
    }
  
    void Update()
    {
      #if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
      #endif
    }
  
    public static async void sendWebSocketMessage(WebSocketMessage message)
    {
        if (instance.websocket.State != WebSocketState.Open) return;
        await instance.websocket.SendText(JsonUtility.ToJson(message));
    }

    private void OnDestroy()
    {
        Debug.Log("destroyed");
        if (websocket == null) return;
        websocket.Close();
        websocket = null;
    }

    private async void OnApplicationQuit()
    {
        if (websocket == null) return;
        websocket.Close();
        websocket = null;
    }

    private void onMessageReceived(string json)
    {
        WebSocketMessage baseMessage = JsonUtility.FromJson<WebSocketMessage>(json);

        switch (baseMessage.type)
        {
            case "player_joined":
                PlayerJoined playerJoined = JsonUtility.FromJson<PlayerJoined>(json);
                Debug.Log($"{playerJoined.users.Last()} joined the lobby");
                players.Add(playerJoined.users.Last());
                EventManagers.player.dispatchEvent(it => it.onPlayerJoin(playerJoined.users));
                break;
            
            case "player_left":
                PlayerLeft playerLeft = JsonUtility.FromJson<PlayerLeft>(json);
                Debug.Log($"{playerLeft.username} left the lobby");
                players.Remove(playerLeft.username);
                EventManagers.player.dispatchEvent(it => it.onPlayerLeft(playerLeft.username));
                if (playerLeft.username == username)
                {
                    SceneManager.LoadScene(Vault.scene.TitleScreen);
                    Destroy(this);
                }
                break;
            
            case "start_game":
                SceneManager.LoadScene(Vault.scene.Game);
                break;
            
            case "hand_dealt":
                HandDealt handDealt = JsonUtility.FromJson<HandDealt>(json);
                EventManagers.game.dispatchEvent(it => it.onHandReceived(handDealt.cards));
                break;
        }
    }

    public static string getKey() => instance.lobbyKey;
    public static string getUsername() => instance.username;
}
