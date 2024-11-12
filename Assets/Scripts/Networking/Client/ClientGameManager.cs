using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private JoinAllocation _Allocation;

    private NetworkClient _Client;

    private const string _MenuScene = "MainMenu";

    public async Task<bool> InitAsync()
    {
        //Player Auth
        await UnityServices.InitializeAsync();

        _Client = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationHandler.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    internal void GoToMainMenu()
    {
        SceneManager.LoadScene(_MenuScene);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            _Allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(_Allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        UserData userdata = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelecter._PlayerNameKey, "Missing Name"),
            UserAuthID = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userdata);
        byte[] payLoadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payLoadBytes;

        NetworkManager.Singleton.StartClient();
    }
    public void Disconnect()
    {
        _Client.Disconnect();
    }

    public void Dispose()
    {
        _Client?.Dispose();
    }

}
