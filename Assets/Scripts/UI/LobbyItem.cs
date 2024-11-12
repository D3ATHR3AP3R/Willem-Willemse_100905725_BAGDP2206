using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _LobbyNameText;
    [SerializeField] private TMP_Text _LobbyPlayersText;

    private LobbyList _LobbiesList;
    private Lobby _Lobby;

    public void Initialise(LobbyList lobbieslist, Lobby lobby)
    {
        _LobbiesList = lobbieslist;
        _Lobby = lobby;

        _LobbyNameText.text = lobby.Name;
        _LobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        _LobbiesList.JoinAsync(_Lobby);
    }
}
