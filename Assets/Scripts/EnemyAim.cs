using System.Collections;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 5f;

    private void Start()
    {
        StartCoroutine(SpawnPrefabWithInterval());
    }
    IEnumerator SpawnPrefabWithInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            // Spawn do prefab
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
    }
}
