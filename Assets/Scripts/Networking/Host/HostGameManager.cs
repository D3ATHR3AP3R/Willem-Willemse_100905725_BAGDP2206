using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation _Allocation;
    private string _JoinCode;
    private string _LobbyID;

    public NetworkServer _NetworkServer { get; private set; }

    private const int _MaxConnections = 20;
    private const string _SceneName = "Game";

    public async Task startHostAsync()
    {
        try
        {
            _Allocation = await Relay.Instance.CreateAllocationAsync(_MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            _JoinCode = await Relay.Instance.GetJoinCodeAsync(_Allocation.AllocationId);
            Debug.Log($"{_JoinCode}");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(_Allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _JoinCode
                        )
                }
            };
            string playerName = PlayerPrefs.GetString(NameSelecter._PlayerNameKey, "Blank");
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", _MaxConnections, lobbyOptions);

            _LobbyID = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        _NetworkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userdata = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelecter._PlayerNameKey, "Missing Name"),
            UserAuthID = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userdata);
        byte[] payLoadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;

        NetworkManager.Singleton.StartHost();

        _NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(_SceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_LobbyID);
            yield return delay;
        }
    }

    public void Dispose()
    {
        ShutDown();
    }

    public async void ShutDown()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));

        if (!string.IsNullOrEmpty(_LobbyID))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_LobbyID);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            _LobbyID = string.Empty;
        }

        _NetworkServer.OnClientLeft -= HandleClientLeft;

        _NetworkServer?.Dispose();
    }

    private async void HandleClientLeft(string authID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_LobbyID, authID);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
