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
    private string currentPlayerTurn = "";

    public static List<string> getPlayers() => instance.players;
  
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
            SceneManager.LoadScene(Vault.scene.TitleScreen);
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
                players = playerJoined.users;
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

            case "player_turn":
                PlayerTurn playerTurn = JsonUtility.FromJson<PlayerTurn>(json);
                Debug.Log($"Player turn: {playerTurn.username}");
                EventManagers.turn.dispatchEvent(it => it.onPlayerTurn(playerTurn.username));
                if (currentPlayerTurn == username) {
                    EventManagers.turn.dispatchEvent(it => it.onMyTurnEnd());
                }
                if (playerTurn.username == username) {
                    EventManagers.turn.dispatchEvent(it => it.onMyTurnStart());
                }
                currentPlayerTurn = playerTurn.username;
                break;
            
            case "bid_made":
                BidMade bidMade = JsonUtility.FromJson<BidMade>(json);
                Debug.Log(bidMade.bid);
                EventManagers.bid.dispatchEvent(it => it.onBidMade(currentPlayerTurn, Enum.Parse<Bid>(bidMade.bid)));
                break;
            
            case "card_played":
                CardPlayed cardPlayed = JsonUtility.FromJson<CardPlayed>(json);
                Debug.Log($"{currentPlayerTurn} played {new Card(cardPlayed.card).toString()}");
                EventManagers.game.dispatchEvent(it => it.onCardPlayed(currentPlayerTurn, cardPlayed.card));
                if (currentPlayerTurn == username)
                {
                    EventManagers.game.dispatchEvent(it => it.onCardPlayedByMe(cardPlayed.card));
                }
                else
                {
                    EventManagers.game.dispatchEvent(it => it.onCardPlayedByOther(currentPlayerTurn, cardPlayed.card));
                }
                break;
                
            
            case "first_turn":
                FirstTurn firstTurn = JsonUtility.FromJson<FirstTurn>(json);
                Debug.Log("first turn");
                EventManagers.game.dispatchEvent(it => it.onFirstTurn(firstTurn.username));
                break;
            
            case "turn_won":
                TurnWon turnWon = JsonUtility.FromJson<TurnWon>(json);
                Debug.Log($"Turn won by {turnWon.username}");
                EventManagers.turn.dispatchEvent(it => it.onTurnWon(turnWon.username));
                EventManagers.turn.dispatchEvent(it => it.onPlayerTurn(turnWon.username));
                if (currentPlayerTurn == username) {
                    EventManagers.turn.dispatchEvent(it => it.onMyTurnEnd());
                }
                if (turnWon.username == username) {
                    EventManagers.turn.dispatchEvent(it => it.onMyTurnStart());
                }
                currentPlayerTurn = turnWon.username;
                break;
        }
    }

    public static string getKey() => instance.lobbyKey;
    public static string getUsername() => instance.username;
}
