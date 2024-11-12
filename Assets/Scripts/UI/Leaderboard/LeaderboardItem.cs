using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _DisplayText;
    [SerializeField] private Color _Color;

    private FixedString32Bytes _PlayerName;

    public ulong _ClientID { get; private set; }
    public int _Coins { get; private set; }

    public void Initialise(ulong clientID, FixedString32Bytes playerName, int coins)
    {
        _ClientID = clientID;
        _PlayerName = playerName;

        if(clientID == NetworkManager.Singleton.LocalClientId)
        {
            _DisplayText.color = _Color;
        }

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins)
    {
        _Coins = coins;

        UpdateText();
    }

    public void UpdateText()
    {
        _DisplayText.text = $"{transform.GetSiblingIndex() + 1}. {_PlayerName} {_Coins}";
    }
}
