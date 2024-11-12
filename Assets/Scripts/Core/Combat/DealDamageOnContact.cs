using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _Damage = 2;
    [SerializeField] private AudioSource _DamageAudio;

    private ulong _OwnerClientID;

    public void SetOwner(ulong ownerClientID)
    {
        this._OwnerClientID = ownerClientID;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) return;

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject obj))
        {
            if (_OwnerClientID == obj.OwnerClientId)
            {
                return;
            }
        }

        if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_Damage);
        }

        _DamageAudio.Play();
    }
}
