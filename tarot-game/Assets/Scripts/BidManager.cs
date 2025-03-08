
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BidManager: MonoBehaviour, IBidListener, ITurnListener
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
        EventManagers.turn.registerListener(this);
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
        EventManagers.turn.unregisterListener(this);
    }

    public void makeBid(int bid)
    {
        LobbyManager.sendWebSocketMessage(new BidMade((Bid)bid));
    }

    public void onBidMade(string username, Bid bid)
    {
       playerBidsDisplay[username].setBid(bid);
       Debug.Log($"Bid received: {bid}");
       if (bid == Bid.PASSE) return;
       for (int i = 1; i <= (int)bid; i++)
       {
           bids[i].interactable = false;
           Debug.Log($"Deactivated {i}");
       }

       bidsReceived++;
       if (bidsReceived == LobbyManager.getPlayers().Count)
       {
           Destroy(this);
       }
    }

    public void onPlayerTurn(string username)
    {

    }

    public void onMyTurnStart()
    {
        makeBidLayout.SetActive(true);
    }

    public void onMyTurnEnd()
    {
        makeBidLayout.SetActive(false);
    }
}
