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
                Debug.Log("hand dealt");
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
            
            //Bids
            
            case "await_bid":
                string playerUsername = JsonUtility.FromJson<UsernameWrapper>(json).username;
                EventManagers.bid.dispatchEvent(it => it.onAwaitBid(playerUsername));
                break;

            case "bid_made":
                BidMade bidMade = JsonUtility.FromJson<BidMade>(json);
                Debug.Log(bidMade.bid);
                EventManagers.bid.dispatchEvent(it => it.onBidMade(bidMade.username, Enum.Parse<Bid>(bidMade.bid)));
                break;
            
            case "bid_won":
                BidWon bidWon = JsonUtility.FromJson<BidWon>(json);
                EventManagers.bid.dispatchEvent(it => it.onBidWon(bidWon.username, bidWon.bid));
                break;
            
            case "fausse_donne":
                Debug.Log("fausse donne");
                EventManagers.fausseDonne.dispatchEvent(it => it.onFausseDonne());
                break;
            
            //Turns
            
            case "card_played":
                CardPlayed cardPlayed = JsonUtility.FromJson<CardPlayed>(json);
                Debug.Log($"{cardPlayed.username} played {new Card(cardPlayed.card).toString()}");
                EventManagers.game.dispatchEvent(it => it.onCardPlayed(cardPlayed.username, cardPlayed.card));
                if (cardPlayed.username == username)
                {
                    EventManagers.game.dispatchEvent(it => it.onCardPlayedByMe(cardPlayed.card));
                }
                else
                {
                    EventManagers.game.dispatchEvent(it => it.onCardPlayedByOther(cardPlayed.username, cardPlayed.card));
                }
                break;

            case "turn_won":
                TurnWon turnWon = JsonUtility.FromJson<TurnWon>(json);
                Debug.Log($"Turn won by {turnWon.username}");
                EventManagers.turn.dispatchEvent(it => it.onTurnWon(turnWon.username));
                break;
            
            case "game_over":
                GameOver gameOver = JsonUtility.FromJson<GameOver>(json);
                Debug.Log($"Victory ? {gameOver.victory} with {gameOver.score} points");
                foreach (var score in gameOver.playerScores)
                {
                    Debug.Log($"{score.username} -> {score.score} points");
                }
                break;
            
            case "dog_reveal":
                DogReveal dogReveal = JsonUtility.FromJson<DogReveal>(json);
                EventManagers.dog.dispatchEvent(it => it.onDogReveal(dogReveal.cards, dogReveal.attacker));
                break;
            
            case "appel":
                Appel appel = JsonUtility.FromJson<Appel>(json);
                EventManagers.appel.dispatchEvent(it => it.onAppel(appel.color, appel.username));
                break;
            
            case "await_appel":
                AwaitAppel awaitAppel = JsonUtility.FromJson<AwaitAppel>(json);
                EventManagers.appel.dispatchEvent(it => it.onAwaitAppel(awaitAppel.username));
                break;
        }
    }

    public static string getKey() => instance.lobbyKey;
    public static string getUsername() => instance.username;
}
