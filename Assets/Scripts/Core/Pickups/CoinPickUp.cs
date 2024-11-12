using Unity.Netcode;
using UnityEngine;

public class CoinPickUp : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public void SpemdCoins(int shotCost)
    {
        TotalCoins.Value -= shotCost;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin))
        {
            return;
        }

        int value = coin.Collect();

        if (!IsServer)
        {
            return;
        }

        TotalCoins.Value += value;

    }
}
