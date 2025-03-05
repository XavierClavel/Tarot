using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NativeWebSocket;
using UnityEngine;

public class LobbyManager: MonoBehaviour
{
    private static LobbyManager instance = null;
    private WebSocket websocket = null;
    private string lobbyKey = "";
    private List<string> players = new List<string>(); 
  
    // Start is called before the first frame update
    public async void join(string lobby, string username)
    {
        instance = this;
        lobbyKey = lobby;
        Debug.Log($"'{lobbyKey}'");
        var cleanedLobby = lobby.Trim().Replace("\u200B", "");
        var cleanedUsername = username.Trim().Replace("\u200B", "");
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
  
    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
          // Sending bytes
          //await websocket.Send(new byte[] { 10, 20, 30 });
    
          // Sending plain text
          await websocket.SendText("plain text message");
        }
    }
  
    private async void OnApplicationQuit()
    {
      await websocket.Close();
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
                break;
        }
    }

    public static string getKey() => instance.lobbyKey;
}
