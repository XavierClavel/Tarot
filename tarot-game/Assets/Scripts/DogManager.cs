
using System;
using System.Collections.Generic;
using UnityEngine;

public class DogManager: MonoBehaviour, IDogListener
{
    [SerializeField] private Transform canvas;
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private DogSlot slotPrefab;
    [SerializeField] private Transform dogLayout;
    private List<DogSlot> slots = new List<DogSlot>();
    
    private void Awake()
    {
        EventManagers.dog.registerListener(this);
    }

    private void Start()
    {
        generateCards(new List<int>{1,2,3,4,5,6});
    }

    private void OnDestroy()
    {
        EventManagers.dog.unregisterListener(this);
    }
    
    private void generateCards(List<int> hand)
    {
        int amount = hand.Count;
        Debug.Log(amount);
        float deltaX = 50 * (1 - (float)amount / 50);
        for (int i = 0; i < amount; i++)
        {
            Debug.Log(i);
            var index =  i - (float)(amount-1) / 2;
            var slot = Instantiate(slotPrefab, dogLayout);
            slot.name = $"Slot {i + 1}";
            slot.transform.position += deltaX * index * Vector3.right;
            
            var card = Instantiate(cardPrefab, slot.transform);
            card.transform.localPosition = Vector3.zero;
            card.setup(canvas, slot);
            card.setValue(hand[i]);
            //cards.Add(card);
            
            slots.Add(slot);
        }
    }

    public void onDogReveal(List<int> cards)
    {
        
    }

    public void submitDog()
    {
        var dog = slots.map(it => ((TarotCard)it.itemSelected).card.index);
        foreach (int card in dog)
        {
            Debug.Log($"{card} in dog");
        }
        LobbyManager.sendWebSocketMessage(new DogMade(dog));
    }
}
