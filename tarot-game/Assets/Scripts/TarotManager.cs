
using System;
using System.Collections.Generic;using UnityEngine;

public class TarotManager: MonoBehaviour, IGameListener
{
    private void Awake()
    {
        EventManagers.game.registerListener(this);
    }

    private void Start()
    {
        LobbyManager.sendWebSocketMessage(new GameReady());
        Debug.Log("Sent 'ready' event");
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
    }

    public void onHandReceived(List<int> cards)
    {
        Debug.Log($"Received {cards.Count} cards.");
        foreach (var card in cards)
        {
            Debug.Log(card);
        }
    }

    public void onCardPlayedByOther(int card)
    {
        throw new System.NotImplementedException();
    }

    public void onCardPlayedByMe(int card)
    {
        throw new System.NotImplementedException();
    }

    public void onPlayerTurn(string username)
    {
        throw new System.NotImplementedException();
    }

    public void onTurnWon(string username)
    {
        throw new System.NotImplementedException();
    }
}
