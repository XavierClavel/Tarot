using System;
using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour, IGameListener, ITurnListener
{
    [SerializeField] private Transform cardsLayout;
    [SerializeField] private PlayerCard playerCardPrefab;
    private List<PlayerCard> playerCards = new List<PlayerCard>();
    private bool turnWon = false;

    private void Awake()
    {
        EventManagers.game.registerListener(this);
        EventManagers.turn.registerListener(this);
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
        EventManagers.turn.unregisterListener(this);
    }


    public void onHandReceived(List<int> cards)
    {
        
    }

    public void onCardPlayed(string username, int card)
    {
        PlayerCard playerCard = Instantiate(playerCardPrefab, cardsLayout);
        playerCard.setup(card, username);
        playerCards.Add(playerCard);
    }

    public void onCardPlayedByOther(string username, int card)
    {
        
    }

    public void onCardPlayedByMe(int card)
    {
        
    }


    public void onPlayerTurn(string username)
    {
        if (!turnWon) return;
        turnWon = false;
        foreach (var card in playerCards)
        {
            Destroy(card.gameObject);
        }

        playerCards = new List<PlayerCard>();
    }

    public void onMyTurnStart()
    {
        
    }

    public void onMyTurnEnd()
    {
        
    }

    public void onTurnWon(string username)
    {
        turnWon = true;
    }

}
