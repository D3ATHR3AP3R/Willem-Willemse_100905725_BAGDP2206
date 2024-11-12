using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader _InputReader;
    [SerializeField] private Transform _AimTransform;

    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector2 aimScreenPos = Input.mousePosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPos);

        _AimTransform.up = new Vector2(
            aimWorldPosition.x - _AimTransform.position.x,
            aimWorldPosition.y - _AimTransform.position.y);
    }
}
