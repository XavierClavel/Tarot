using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private LobbyManager lobbyManagerPrefab;
    [SerializeField] private TextMeshProUGUI usernameInput;
    [SerializeField] private TextMeshProUGUI lobbyInput;
    private LobbyManager lobby;

    public async void createLobby()
    {
        using var request = UnityWebRequest.PostWwwForm($"http://{Vault.url}/lobby", "");
        await request.SendWebRequest();
        string key = request.downloadHandler.text;
        Debug.Log($"lobby created with key {key}");
        joinLobby(key);
    }

    public void joinLobby()
    {
        joinLobby(getLobbyKey());
    }


    private void joinLobby(string key)
    {
        lobby = Instantiate(lobbyManagerPrefab);
        DontDestroyOnLoad(lobby);
        lobby.join(key, getUsername());
        SceneManager.LoadScene(Vault.scene.Lobby);
    }

    private string getUsername() => usernameInput.text.Trim();
    private string getLobbyKey() => lobbyInput.text.Trim();

}
