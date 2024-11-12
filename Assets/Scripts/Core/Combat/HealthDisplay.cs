using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private Health _Health;
    [SerializeField] private Image _HealthBarFill;

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        _Health.currentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, _Health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
        {
            return;
        }

        _Health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        _HealthBarFill.fillAmount = (float)newHealth / _Health._MaxHealth;
    }
}
