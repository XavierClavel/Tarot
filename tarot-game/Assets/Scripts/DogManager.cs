
using System;
using System.Collections.Generic;
using UnityEngine;

public class DogManager: MonoBehaviour, IDogListener, ITurnListener
{
    [SerializeField] private Transform canvas;
    [SerializeField] private TarotCard cardPrefab;
    [SerializeField] private DogSlot slotPrefab;
    [SerializeField] private Transform dogLayout;
    [SerializeField] private GameObject submitButton;
    private List<DogSlot> slots = new List<DogSlot>();
    public static bool dogPhase = false;
    
    private void Awake()
    {
        dogPhase = false;
        EventManagers.dog.registerListener(this);
        EventManagers.turn.registerListener(this);
        submitButton.SetActive(false);
    }

    private void Start()
    {
        //generateCards(new List<int>{1,2,3,4,5,6});
    }

    private void OnDestroy()
    {
        EventManagers.dog.unregisterListener(this);
        EventManagers.turn.unregisterListener(this);
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

    public void onDogReveal(List<int> cards, string attacker)
    {
        dogPhase = true;
        generateCards(cards);
        if (attacker != LobbyManager.getUsername())
        {
            foreach (var slot in slots)
            {
                ((TarotCard)slot.itemSelected).disableDrag();
            }
        }
        else
        {
            submitButton.SetActive(true);
        }
    }

    public void submitDog()
    {
        submitButton.SetActive(false);
        var dog = slots.map(it => ((TarotCard)it.itemSelected).card.index);
        foreach (int card in dog)
        {
            Debug.Log($"{card} in dog");
        }
        LobbyManager.sendWebSocketMessage(new DogMade(dog));
    }

    public void onPlayerTurn(string username)
    {
        if (!dogPhase) return;
        dogPhase = false;
        dogLayout.gameObject.SetActive(false);
    }

    public void onMyTurnStart()
    {
        
    }

    public void onMyTurnEnd()
    {
        
    }

    public void onTurnWon(string username)
    {
        
    }
}
