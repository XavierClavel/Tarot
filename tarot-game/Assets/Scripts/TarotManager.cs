
using System;
using System.Collections.Generic;using UnityEngine;

public class TarotManager: MonoBehaviour, IGameListener
{
    private void Awake()
    {
        EventManagers.game.registerListener(this);
        LobbyManager.sendWebSocketMessage(new GameReady());
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
    }

    public void onHandReceived(List<int> cards)
    {
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
