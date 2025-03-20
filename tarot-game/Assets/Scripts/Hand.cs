
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Hand: MonoBehaviour, IGameListener, ITurnListener, IDogListener, IFausseDonneListener
{
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private DraggableHolder slotPrefab;
    [SerializeField] private TarotSprites tarotSprites;
    [SerializeField] private List<Sprite> sprites;
    private List<TarotCard> cards = new List<TarotCard>();
    private List<DraggableHolder> slots = new List<DraggableHolder>();
    [SerializeField] private RectTransform canvas;
    public static Hand instance;
    private bool isBiddingOver = false;

    public static void playCard(int card)
    {
        LobbyManager.sendWebSocketMessage(new CardPlayed(card));
        instance.removeCard(card);
    }
    
    
    public static Sprite getSprite(int card) => instance.tarotSprites.getSprite(card);

    public static void onBiddingOver() => instance.isBiddingOver = true;

    private void Awake()
    {
        instance = this;
        EventManagers.game.registerListener(this);
        EventManagers.turn.registerListener(this);
        EventManagers.dog.registerListener(this);
        EventManagers.fausseDonne.registerListener(this);
        //generateCards(new List<int>{10,11,12,13,14});
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
        EventManagers.turn.unregisterListener(this);
        EventManagers.dog.unregisterListener(this);
        EventManagers.fausseDonne.unregisterListener(this);
    }

    private void generateCards(List<int> hand)
    {
        foreach (var card in hand)
        {
            addCard(card);
        }
    }

    public void addCard(int cardId)
    {
        var slot = Instantiate(slotPrefab, transform);
        
        var newCard = Instantiate(cardPrefab, slot.transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.setup(canvas, slot);
        newCard.setValue(cardId);
        
        cards.Add(newCard);
        slots.Add(slot);
        if (isBiddingOver)
        {
            sortCards();
        }

        updateCardsPositions();
    }

    private void sortCards()
    {
        Debug.Log("Sorting cards");
        cards = cards.OrderBy(it => it.card.index).ToList();
        slots = slots.OrderBy(it => ((TarotCard)it.itemSelected).card.index).ToList();
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].transform.SetSiblingIndex(i);
        }
    }

    public void removeCard(int cardId)
    {
        var index = instance.cards.FindIndex(it => it.card.index == cardId);
        Debug.Log(index);
        cards.RemoveAt(index);
        Destroy(slots[index].gameObject);
        slots.RemoveAt(index);

        updateCardsPositions();
    }

    private void updateCardsPositions()
    {
        int amount = cards.Count;
        float deltaRotation = 5 * (1 - (float)amount / 50);
        float deltaX = 50 * (1 - (float)amount / 50);
        
        for (int i = 0; i < amount; i++)
        {
            var index =  i - (float)(amount-1) / 2;
            slots[i].transform.localPosition = deltaX * index * Vector3.right
                                       + 5 * Mathf.Pow(Mathf.Abs(index)/3.3f, 2) * Vector3.down;
            slots[i].transform.eulerAngles = deltaRotation * index * Vector3.back;
        }
    }

    public void onHandReceived(List<int> cards)
    {
        generateCards(cards);
        foreach (var card in this.cards)
        {
            card.disableDrag();
        }
    }

    public void onCardPlayed(string username, int card)
    {
        
    }

    public void onCardPlayedByOther(string username, int card)
    {
        
    }

    public void onCardPlayedByMe(int card)
    {

    }

    public void onTurnWon(string username)
    {
        
    }

    public void onPlayerTurn(string username)
    {
    }

    public void onMyTurnStart()
    {
        if (!isBiddingOver)
        {
            isBiddingOver = true;
            return;
        }
        Debug.Log("turn start !");
        foreach (var card in cards)
        {
            if (TarotManager.canBePlayed(card.card))
            {
                card.enableDrag();
                card.whitenImage();
            }
            else
            {
                card.disableDrag();
                card.darkenImage();
            }
            
        }
    }

    public void onMyTurnEnd()
    {
        foreach (var card in cards)
        {
            card.disableDrag();
            card.whitenImage();
        }
    }

    public static List<Card> getCards() => instance.cards.map(it => it.card);
    
    public void onDogReveal(List<int> cards, string attacker)
    {
        sortCards();
        updateCardsPositions();
        if (attacker != LobbyManager.getUsername()) return;
        foreach (var card in this.cards)
        {
            if (card.card.isRoi() || card.card.isOudler())
            {
                card.disableDrag();
                card.darkenImage();
            }
            else
            {
                card.enableDrag();
                card.whitenImage();
            }
        }
    }

    public void onFausseDonne()
    {
        var cardsToRemove = cards.map(it => it.card.index);
        cardsToRemove.ForEach(removeCard);
    }
}
