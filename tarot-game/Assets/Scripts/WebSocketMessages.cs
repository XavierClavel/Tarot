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