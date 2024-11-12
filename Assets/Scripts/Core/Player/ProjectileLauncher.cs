using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CoinPickUp _CoinWallet;
    [SerializeField] private InputReader _InputReader;
    [SerializeField] private Transform _ProjectileSpawnPoint;
    [SerializeField] private GameObject _ServerProjectilePrefab;
    [SerializeField] private GameObject _ClientProjectilePrefab;
    [SerializeField] private Collider2D _PlayerCol;
    [SerializeField] private AudioSource _AttackAudio;

    [Header("Settings")]
    [SerializeField] private float _ProjectileSpeed;
    [SerializeField] private float _FireRate;
    [SerializeField] private int _ShotCost;

    private bool _ShouldFire;
    private float _FireTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _InputReader.PrimaryFireEvent += AttackHandle;
    }


    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _InputReader.PrimaryFireEvent -= AttackHandle;
    }

    private void AttackHandle(bool shouldFire)
    {
        this._ShouldFire = shouldFire;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!_ShouldFire)
        {
            return;
        }

        if (_FireTime > _FireRate)
        {
            if (_CoinWallet.TotalCoins.Value < _ShotCost)
            {
                return;
            }

            SpawnDummyProjectile(_ProjectileSpawnPoint.position, _ProjectileSpawnPoint.up);
            PrimaryFireServerRPC(_ProjectileSpawnPoint.position, _ProjectileSpawnPoint.up);

            _AttackAudio.Play();

            _FireTime = 0;
        }

        if (_FireTime < _FireRate)
        {
            _FireTime += Time.deltaTime;
        }
    }

    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (_CoinWallet.TotalCoins.Value < _ShotCost)
        {
            return;
        }

        _CoinWallet.SpemdCoins(_ShotCost);

        GameObject ProjectileInstance = Instantiate(_ServerProjectilePrefab, spawnPos, Quaternion.identity);
        ProjectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_PlayerCol, ProjectileInstance.GetComponent<Collider2D>());

        if (ProjectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damage))
        {
            damage.SetOwner(OwnerClientId);
        }

        if (ProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _ProjectileSpeed;
        }

        PrimaryFireClientRPC(spawnPos, direction);
    }

    [ClientRpc]
    private void PrimaryFireClientRPC(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner)
        {
            return;
        }

        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject ProjectileInstance = Instantiate(_ClientProjectilePrefab, spawnPos, Quaternion.identity);
        ProjectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_PlayerCol, ProjectileInstance.GetComponent<Collider2D>());

        if (ProjectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _ProjectileSpeed;
        }
    }
}
