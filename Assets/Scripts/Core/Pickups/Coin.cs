using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer _SpriteRenderer;

    protected int _CoinValue = 10;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetValue(int coinValue)
    {
        _CoinValue = coinValue;
    }

    protected void Show(bool show)
    {
        _SpriteRenderer.enabled = show;
    }
}
