using System.Collections;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{

    public GameObject prefabToSpawn;

    public void ShootBullet()
    {
        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
    }
}
