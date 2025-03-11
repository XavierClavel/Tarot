using System.Collections.Generic;

[System.Serializable]
public class WebSocketMessage {
    public string type;
}

[System.Serializable]
public class PlayerJoined : WebSocketMessage {
    public List<string> users;
}

[System.Serializable]
public class PlayerLeft : WebSocketMessage {
    public string username;

    public PlayerLeft(string username)
    {
        this.type = "player_left";
        this.username = username;
    }
}

[System.Serializable]
public class CardPlayed : WebSocketMessage {
    public int card;

    public CardPlayed(int card)
    {
        this.type = "card_played";
        this.card = card;
    }
}

[System.Serializable]
public class HandDealt : WebSocketMessage
{
    public List<int> cards;
}

[System.Serializable]
public class StartGame : WebSocketMessage
{
    public string parameters = "";

    public StartGame()
    {
        this.type = "start_game";
    }
}

[System.Serializable]
public class GameReady : WebSocketMessage
{
    public GameReady()
    {
        this.type = "game_ready";
    }
}

[System.Serializable]
public class PlayerTurn : WebSocketMessage
{
    public string username;
}

[System.Serializable]
public class BidMade : WebSocketMessage
{
    public string bid;

    public BidMade(Bid bid)
    {
        this.type = "bid_made";
        this.bid = bid.ToString();
    }
}

[System.Serializable]
public class TurnWon : WebSocketMessage
{
    public string username;
}

[System.Serializable]
public class GameOver : WebSocketMessage
{
    public bool victory;
    public int score;
    public Dictionary<string, int> playerScores;
}

[System.Serializable]
public class DogReveal : WebSocketMessage
{
    public List<int> cards;
    public string attacker;
}

[System.Serializable]
public class DogMade : WebSocketMessage
{
    public List<int> cards;

    public DogMade(List<int> cards)
    {
        this.type = "dog_made";
        this.cards = cards;
    }
}