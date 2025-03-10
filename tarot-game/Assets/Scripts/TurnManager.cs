using System;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour, ITurnListener
{
    [SerializeField] private TextMeshProUGUI turnDisplay;

    private void Awake()
    {
        EventManagers.turn.registerListener(this);
    }

    private void OnDestroy()
    {
        EventManagers.turn.unregisterListener(this);
    }

    public void onPlayerTurn(string username)
    {
        turnDisplay.SetText($"{username}'s turn");
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
