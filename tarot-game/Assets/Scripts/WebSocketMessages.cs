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
}