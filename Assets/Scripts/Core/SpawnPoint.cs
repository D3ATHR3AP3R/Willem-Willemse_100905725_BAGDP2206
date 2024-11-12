using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> _SpawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        _SpawnPoints.Add(this);
    }

    private void OnDisable()
    {
        _SpawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPos()
    {
        if (_SpawnPoints.Count == 0)
        {
            return Vector3.zero;
        }
        return _SpawnPoints[Random.Range(0, _SpawnPoints.Count)].transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
