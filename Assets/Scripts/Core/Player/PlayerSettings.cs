using Cinemachine;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _Camera;

    [SerializeField] private int _Priority = 11;

    [field: SerializeField] public Health _Health { get; private set; }
    [field: SerializeField] public CoinPickUp _Wallet { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<PlayerSettings> OnPlayerSpawn;

    public static event Action<PlayerSettings> OnPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData userData = HostSingleton.Instance.GameManager._NetworkServer.GetUserData(OwnerClientId);

            playerName.Value = userData.UserName;

            OnPlayerSpawn?.Invoke(this);
        }

        if (IsOwner)
        {
            _Camera.Priority = _Priority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }
}
