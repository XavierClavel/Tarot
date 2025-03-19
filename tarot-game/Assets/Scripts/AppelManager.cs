
using System;
using TMPro;
using UnityEngine;

public class AppelManager: MonoBehaviour, IAppelListener
{
    [SerializeField] private GameObject appelLayout;
    [SerializeField] private TextMeshProUGUI appelDisplay;
    private void Awake()
    {
        EventManagers.appel.registerListener(this);
    }

    private void OnDestroy()
    {
        EventManagers.appel.unregisterListener(this);
    }

    public void onAppel(TarotColor color, string username)
    {
        appelDisplay.SetText($"{username} appelle à {color.ToString()}");
    }

    public void onAwaitAppel(string username)
    {
        if (username != LobbyManager.getUsername()) return;
        appelLayout.SetActive(true);
    }

    public void makeAppel(int color)
    {
        var appel = (TarotColor)color;
        LobbyManager.sendWebSocketMessage(new Appel(appel, LobbyManager.getUsername()));
    }
    
}
