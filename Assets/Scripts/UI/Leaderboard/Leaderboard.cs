using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform _LeaderboardItemHolder;
    [SerializeField] private LeaderboardItem _LeaderboardItemPrefab;
    [SerializeField] private int _LeaderboardItemCount = 10;

    private NetworkList<LeaderboardItemState> leaderboardItems;
    private List<LeaderboardItem> itemDisplay = new List<LeaderboardItem>();

    private void Awake()
    {
        leaderboardItems = new NetworkList<LeaderboardItemState>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderboardItems.OnListChanged += HandleLeaderboardItemsChanged;
            foreach(LeaderboardItemState item in leaderboardItems)
            {
                HandleLeaderboardItemsChanged(new NetworkListEvent<LeaderboardItemState>
                {
                    Type = NetworkListEvent<LeaderboardItemState>.EventType.Add,
                    Value = item
                });
            }
        }

        if(IsServer)
        {
            PlayerSettings[] players = FindObjectsByType<PlayerSettings>(FindObjectsSortMode.None);

            foreach (PlayerSettings p in players)
            {
                HandlePlayerSpawn(p);
            }

            PlayerSettings.OnPlayerSpawn += HandlePlayerSpawn;
            PlayerSettings.OnPlayerDespawn += HandlePlayerDespawn;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardItems.OnListChanged -= HandleLeaderboardItemsChanged;
        }

        if (IsServer)
        {
            PlayerSettings.OnPlayerSpawn -= HandlePlayerSpawn;
            PlayerSettings.OnPlayerDespawn -= HandlePlayerDespawn;
        }
    }

    private void HandleLeaderboardItemsChanged(NetworkListEvent<LeaderboardItemState> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardItemState>.EventType.Add:
                if(!itemDisplay.Any(x => x._ClientID == changeEvent.Value.ClientID))
                {
                    LeaderboardItem leaderboardItem = Instantiate(_LeaderboardItemPrefab, _LeaderboardItemHolder);
                    leaderboardItem.Initialise(changeEvent.Value.ClientID, changeEvent.Value.PlayerName, changeEvent.Value.Coins);
                    itemDisplay.Add(leaderboardItem);
                }
                break;
            case NetworkListEvent<LeaderboardItemState>.EventType.Remove:
                LeaderboardItem toRemove = itemDisplay.FirstOrDefault(x => x._ClientID == changeEvent.Value.ClientID);
                if(toRemove != null)
                {
                    toRemove.transform.SetParent(null);
                    Destroy(toRemove.gameObject);
                    itemDisplay.Remove(toRemove);
                }
                break;
            case NetworkListEvent<LeaderboardItemState>.EventType.Value:
                LeaderboardItem toUpdate = itemDisplay.FirstOrDefault(x => x._ClientID == changeEvent.Value.ClientID);
                if(toUpdate != null)
                {
                    toUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        itemDisplay.Sort((x, y) => y._Coins.CompareTo(x._Coins));

        for (int i = 0; i < itemDisplay.Count; i++)
        {
            itemDisplay[i].transform.SetSiblingIndex(i);
            itemDisplay[i].UpdateText();

            bool shouldShow = i <= _LeaderboardItemCount - 1;
            itemDisplay[i].gameObject.SetActive(shouldShow);
        }

        LeaderboardItem item = itemDisplay.FirstOrDefault(x => x._ClientID == NetworkManager.Singleton.LocalClientId);

        if(item != null)
        {
            if(item.transform.GetSiblingIndex() >= _LeaderboardItemCount)
            {
                _LeaderboardItemHolder.GetChild(_LeaderboardItemCount -1).gameObject.SetActive(false);
                item.gameObject.SetActive(true);
            }
        }
    }

    private void HandlePlayerSpawn(PlayerSettings player)
    {
        leaderboardItems.Add(new LeaderboardItemState
        {
            ClientID = player.OwnerClientId,
            PlayerName = player.playerName.Value,
            Coins = 0
        }) ;

        player._Wallet.TotalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinChange(player.OwnerClientId, newCoins);
    }

    private void HandlePlayerDespawn(PlayerSettings player)
    {
        if(leaderboardItems == null)
        {
            return;
        }

        foreach (LeaderboardItemState item in leaderboardItems)
        {
            if(item.ClientID != player.OwnerClientId)
            {
                continue;
            }

            leaderboardItems.Remove(item);
            break;
        }

        player._Wallet.TotalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinChange(player.OwnerClientId, newCoins);
    }

    private void HandleCoinChange(ulong clientID, int newCoins)
    {
        for(int i = 0; i < leaderboardItems.Count; i++)
        {
            if (leaderboardItems[i].ClientID != clientID)
            {
                continue;
            }

            leaderboardItems[i] = new LeaderboardItemState
            {
                ClientID = leaderboardItems[i].ClientID,
                PlayerName = leaderboardItems[i].PlayerName,
                Coins = newCoins
            };

            return;
        }
    }
}
