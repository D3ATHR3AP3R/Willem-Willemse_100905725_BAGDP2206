using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _NetworkManager;

    public Action<string> OnClientLeft;

    private Dictionary<ulong, string> _ClietnIDTOAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _AuthIDToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        _NetworkManager = networkManager;

        _NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        _NetworkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        _ClietnIDTOAuth[request.ClientNetworkId] = userData.UserAuthID;
        _AuthIDToUserData[userData.UserAuthID] = userData;

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPos();
        response.Rotation = quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        _NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (_ClietnIDTOAuth.TryGetValue(clientId, out string authID))
        {
            _ClietnIDTOAuth.Remove(clientId);
            _AuthIDToUserData.Remove(authID);

            OnClientLeft?.Invoke(authID);
        }
    }

    public UserData GetUserData(ulong clientId)
    {
        if(_ClietnIDTOAuth.TryGetValue(clientId, out string authID))
        {
            if(_AuthIDToUserData.TryGetValue(authID, out UserData userData))
            {
                return userData;
            }

            return null;
        }

        return null;
    }

    public void Dispose()
    {
        if (_NetworkManager != null)
        {
            _NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _NetworkManager.OnServerStarted -= OnNetworkReady;
            _NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;

            if (_NetworkManager.IsListening)
            {
                _NetworkManager.Shutdown();
            }
        }
    }
}
