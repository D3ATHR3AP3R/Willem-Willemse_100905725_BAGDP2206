using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawnCoin _CoinPrefab;

    [SerializeField] private int _MaxCoins;
    [SerializeField] private int _CoinValue;
    [SerializeField] private Vector2 _XSpawnRange;
    [SerializeField] private Vector2 _YSpawnRange;
    [SerializeField] private LayerMask _LayerMask;

    private Collider2D[] _CoinBuffer = new Collider2D[1];

    private float _CoinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        _CoinRadius = _CoinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _MaxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawnCoin coinInstance = Instantiate(_CoinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(_CoinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCollectedCoin;
    }

    private void HandleCollectedCoin(RespawnCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while (true)
        {
            x = Random.Range(_XSpawnRange.x, _XSpawnRange.y);
            y = Random.Range(_YSpawnRange.x, _YSpawnRange.y);

            Vector2 spawnPoint = new Vector2(x, y);

            int numCol = Physics2D.OverlapCircleNonAlloc(spawnPoint, _CoinRadius, _CoinBuffer, _LayerMask);

            if (numCol == 0)
            {
                return spawnPoint;
            }
        }
    }
}
