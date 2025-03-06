
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand: MonoBehaviour
{
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private RectTransform slotPrefab;
    [SerializeField] private List<Sprite> sprites;
    private List<TarotCard> cards = new List<TarotCard>();
    private List<Transform> slots = new List<Transform>();
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        generateCards(16);
    }

    private void generateCards(int amount)
    {
        float deltaRotation = 5 * (1 - (float)amount / 50);
        float deltaX = 50 * (1 - (float)amount / 50);
        for (int i = 0; i < amount; i++)
        {
            var index =  i - (float)(amount-1) / 2;
            Debug.Log(index);
            var slot = Instantiate(slotPrefab, rectTransform);
            slot.position += deltaX * index * Vector3.right;
            slot.position += 5 * Mathf.Pow(Mathf.Abs(index)/3.3f, 2) * Vector3.down;
            slot.eulerAngles += deltaRotation * index * Vector3.back;
            
            var card = Instantiate(cardPrefab, slot);
            card.transform.localPosition = Vector3.zero;
            cards.Add(card);
            card.setup(rectTransform, slot);
            card.image.sprite = sprites.getRandom();
        }
    }
}
