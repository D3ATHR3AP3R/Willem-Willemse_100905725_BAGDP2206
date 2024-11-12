using System;
using UnityEngine;

public class RespawnCoin : Coin
{
    public event Action<RespawnCoin> OnCollected;

    [SerializeField] private AudioSource _CoinCollect;

    private Vector3 _PrevPos;

    public override void OnNetworkSpawn()
    {
        _PrevPos = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;

        OnCollected?.Invoke(this);

        _CoinCollect.Play();

        return _CoinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }

    private void Update()
    {
        if (IsServer)
        {
            return;
        }

        if (_PrevPos != transform.position)
        {
            DisplayCoin();
        }
    }

    private void DisplayCoin()
    {
        Show(true);
        _PrevPos = transform.position;
    }
}
