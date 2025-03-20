
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BidManager: MonoBehaviour, IBidListener, IFausseDonneListener
{
    [SerializeField] private PlayerBid playerBidPrefab;
    [SerializeField] private Transform playerBidsLayout;
    [SerializeField] private GameObject makeBidLayout;
    [SerializeField] private Transform makeBidsPanel;
    [SerializeField] private List<Button> bids;
    private Dictionary<string, PlayerBid> playerBidsDisplay = new Dictionary<string, PlayerBid>();
    private int bidsReceived = 0;
    
    private void Awake()
    {
        EventManagers.bid.registerListener(this);
        EventManagers.fausseDonne.registerListener(this);
        makeBidLayout.SetActive(false);
        foreach (string player in LobbyManager.getPlayers())
        {
            PlayerBid playerBid = Instantiate(playerBidPrefab, playerBidsLayout);
            playerBid.setUsername(player);
            playerBidsDisplay[player] = playerBid;
        }
        makeBidsPanel.SetAsLastSibling();
    }

    private void OnDestroy()
    {
        EventManagers.bid.unregisterListener(this);
        EventManagers.fausseDonne.unregisterListener(this);
    }

    public void makeBid(int bid)
    {
        makeBidLayout.SetActive(false);
        LobbyManager.sendWebSocketMessage(new BidMade((Bid)bid));
    }

    public void onAwaitBid(string username)
    {
        if (username != LobbyManager.getUsername()) return;
        makeBidLayout.SetActive(true);
    }

    public void onBidMade(string username, Bid bid)
    {
       playerBidsDisplay[username].setBid(bid);
       Debug.Log($"Bid received: {bid}");
       if (bid != Bid.PASSE)
       {
           for (int i = 1; i <= (int)bid; i++)
           {
               bids[i].interactable = false;
               Debug.Log($"Deactivated {i}");
           }
       }
    }

    public void onBidWon(string username, Bid bid)
    {
        Debug.Log("Hiding bids");
        playerBidsLayout.gameObject.SetActive(false);
        Destroy(this);
    }

    public void onFausseDonne()
    {
        foreach (var playerBid in playerBidsDisplay)
        {
            playerBid.Value.clearBid();
        }
    }
}
