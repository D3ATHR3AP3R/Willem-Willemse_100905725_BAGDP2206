using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RaspawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerSettings _PlayerPrefab;
    [SerializeField] private float _CoinsKept;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        PlayerSettings[] players = FindObjectsByType<PlayerSettings>(FindObjectsSortMode.None);

        foreach (PlayerSettings p in players)
        {
            HandlePlayerSpawn(p);
        }

        PlayerSettings.OnPlayerSpawn += HandlePlayerSpawn;
        PlayerSettings.OnPlayerDespawn += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        PlayerSettings.OnPlayerSpawn -= HandlePlayerSpawn;
        PlayerSettings.OnPlayerDespawn -= HandlePlayerDespawn;
    }

    private void HandlePlayerSpawn(PlayerSettings player)
    {
        player._Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawn(PlayerSettings player)
    {
        player._Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(PlayerSettings player)
    {
        int keptCoins = (int) (player._Wallet.TotalCoins.Value * (_CoinsKept / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID, int keptCoins)
    {
        yield return null;

        PlayerSettings playerInstance = Instantiate(_PlayerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientID);

        playerInstance._Wallet.TotalCoins.Value += keptCoins;
    }
}
