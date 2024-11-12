using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] float lifetime;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
