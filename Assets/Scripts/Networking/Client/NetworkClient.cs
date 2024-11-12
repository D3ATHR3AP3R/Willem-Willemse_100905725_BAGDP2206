using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _NetworkManager;

    private const string _MenuName = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        _NetworkManager = networkManager;

        _NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId != 0 && clientId != _NetworkManager.LocalClientId)
        {
            return;
        }

        Disconnect();
    }

    public void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != _MenuName)
        {
            SceneManager.LoadScene(_MenuName);
        }

        if (_NetworkManager.IsConnectedClient)
        {
            _NetworkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (_NetworkManager != null)
        {
            _NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
