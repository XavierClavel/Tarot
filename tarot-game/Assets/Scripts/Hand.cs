
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hand: MonoBehaviour, IGameListener
{
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private RectTransform slotPrefab;
    [SerializeField] private TarotSprites tarotSprites;
    [SerializeField] private List<Sprite> sprites;
    private List<TarotCard> cards = new List<TarotCard>();
    private List<Transform> slots = new List<Transform>();
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        EventManagers.game.registerListener(this);
    }

    private void OnDestroy()
    {
        EventManagers.game.unregisterListener(this);
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
            card.image.sprite = tarotSprites.getSprite(hand[i]);
            cards.Add(card);
        }
    }

    public void onHandReceived(List<int> cards)
    {
        generateCards(cards);
    }

    public void onCardPlayedByOther(int card)
    {
        throw new NotImplementedException();
    }

    public void onCardPlayedByMe(int card)
    {
        throw new NotImplementedException();
    }

    public void onPlayerTurn(string username)
    {
        throw new NotImplementedException();
    }

    public void onTurnWon(string username)
    {
        throw new NotImplementedException();
    }
}
