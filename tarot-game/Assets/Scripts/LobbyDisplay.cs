using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyDisplay : MonoBehaviour, IPlayerListener
{
    [SerializeField] private TextMeshProUGUI playerDisplayPrefab;
    [SerializeField] private GameObject playerDisplayLayout;
    [SerializeField] private TextMeshProUGUI lobbyKeyDisplay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManagers.player.registerListener(this);
        lobbyKeyDisplay.text = LobbyManager.getKey();
    }

    private void OnDestroy()
    {
        EventManagers.player.unregisterListener(this);
    }

    public void onPlayerJoin(string username)
    {
        var playerDisplay = Instantiate(playerDisplayPrefab);
        playerDisplay.name = username;
        playerDisplay.text = username;
        Helpers.SetParent(playerDisplay.transform, playerDisplayLayout);
    }

    public void onPlayerJoin(List<string> users)
    {
        var children = playerDisplayLayout.transform.getChildren().map(it => it.name);
        foreach (string username in users)
        {
            if (children.Contains(username)) continue;
            var playerDisplay = Instantiate(playerDisplayPrefab);
            playerDisplay.name = username;
            playerDisplay.text = username;
            Helpers.SetParent(playerDisplay.transform, playerDisplayLayout);
        }
    }

    public void onPlayerLeft(string username)
    {
        var toDelete = playerDisplayLayout.transform.getChildren().Find(it => it.name == username);
        Destroy(toDelete);
    }
}
