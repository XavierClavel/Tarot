[System.Serializable]
public class WebSocketMessage {
    public string type;
}

[System.Serializable]
public class PlayerJoined : WebSocketMessage {
    public string username;
}

[System.Serializable]
public class PlayerLeft : WebSocketMessage {
    public string username;
}