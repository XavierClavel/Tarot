
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand: MonoBehaviour, IGameListener, ITurnListener
{
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private RectTransform slotPrefab;
    [SerializeField] private TarotSprites tarotSprites;
    [SerializeField] private List<Sprite> sprites;
    private List<TarotCard> cards = new List<TarotCard>();
    private List<Card> hand = new List<Card>();
    private List<Transform> slots = new List<Transform>();
    private RectTransform rectTransform;
    private static Hand instance;
    private bool isBiddingOver = false;
    
    
    public static Sprite getSprite(int card) => instance.tarotSprites.getSprite(card);

    public static void onBiddingOver() => instance.isBiddingOver = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        instance = this;
        EventManagers.game.registerListener(this);
        EventManagers.turn.registerListener(this);
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
        EventManagers.turn.unregisterListener(this);
    }

    private void generateCards(List<int> hand)
    {
        int amount = hand.Count;
        float deltaRotation = 5 * (1 - (float)amount / 50);
        float deltaX = 50 * (1 - (float)amount / 50);
        for (int i = 0; i < amount; i++)
        {
            var index =  i - (float)(amount-1) / 2;
            var slot = Instantiate(slotPrefab, rectTransform);
            slot.position += deltaX * index * Vector3.right;
            slot.position += 5 * Mathf.Pow(Mathf.Abs(index)/3.3f, 2) * Vector3.down;
            slot.eulerAngles += deltaRotation * index * Vector3.back;
            
            var card = Instantiate(cardPrefab, slot);
            card.transform.localPosition = Vector3.zero;
            card.setup(rectTransform, slot);
            card.setValue(hand[i]);
            card.disableDrag();
            cards.Add(card);
        }
    }

    public void onHandReceived(List<int> cards)
    {
        generateCards(cards);
        hand = cards.map(it => new Card(it));
    }

    public void onCardPlayed(string username, int card)
    {
        
    }

    public void onCardPlayedByOther(string username, int card)
    {
        
    }

    public void onCardPlayedByMe(int card)
    {
        cards.removeIf(it => it.card.value == card);
        hand.removeIf(it => it.value == card);
    }

    public void onTurnWon(string username)
    {
        
    }

    public void onFirstTurn(string username)
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

    public static List<Card> getCards() => instance.hand;
}
