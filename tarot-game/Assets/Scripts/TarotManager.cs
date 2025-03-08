
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TarotManager: MonoBehaviour, IGameListener
{
    private static TarotManager instance;
    private List<Card> levee = new List<Card>();
    private int turn = 1;
    private TarotColor? calledKing = null;
    private void Awake()
    {
        EventManagers.game.registerListener(this);
        instance = this;
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
        levee.Add(new Card(card));
    }

    public void onCardPlayedByMe(int card)
    {
        
    }

    public void onTurnWon(string username)
    {
        levee = new List<Card>();
        turn++;
    }

    public void onFirstTurn(string username)
    {
        
    }

    public static bool canBePlayed(Card card)
    {
        List<Card> cardsInHand = Hand.getCards();
        
        if (instance.levee.isEmpty())
        {
            if (instance.turn == 1 && card.color == instance.calledKing && card.isRoi())
            {
                return false;
            }

            return true;
        }

        if (card.isExcuse()) return true;
        Card firstCard = instance.levee.First();
        if (instance.levee.Count == 1 && firstCard.isExcuse()) return true;

        Card firstActiveCard = firstCard.isExcuse() ? instance.levee[1] : firstCard;
        
        //must play same color if possible
        if (firstActiveCard.color != card.color && cardsInHand.none(it => it.color == firstActiveCard.color))
        {
            return false;
        }

        if (card.color == TarotColor.ATOUT)
        {
            int? bestAtout = instance.levee.filter(it => it.color == TarotColor.ATOUT).Max(it => it.value);
            if (bestAtout == null) return true;
            if (card.value > bestAtout)
            {
                return true;
            }
            return !cardsInHand.Any(it => it.color == TarotColor.ATOUT && it.value > bestAtout);
        } 
        
        if (cardsInHand.Any(it => it.color == TarotColor.ATOUT))
        {
            return false;
        }
        return true;
    }
}
