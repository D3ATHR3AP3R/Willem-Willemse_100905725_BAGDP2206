using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int _MaxHealth { get; private set; } = 10;

    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private bool _IsDead;

    public Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        currentHealth.Value = _MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int healthValue)
    {
        if (_IsDead)
        {
            return;
        }

        int newHealth = currentHealth.Value += healthValue;
        currentHealth.Value = Mathf.Clamp(newHealth, 0, _MaxHealth);

        if (currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            _IsDead = true;
        }
    }
}
