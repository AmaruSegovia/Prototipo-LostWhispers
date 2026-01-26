using UnityEngine;

public class HordeSpawner : MonoBehaviour
{
    public GameObject brainPrefab;
    public GameObject zombiePrefab;

    public int zombieCount = 50;
    public float spawnRadius = 6f;

    void Start()
    {
        SpawnHorde();
    }

    void SpawnHorde()
    {
        GameObject brainObj = Instantiate(
            brainPrefab,
            transform.position,
            Quaternion.identity
        );

        for (int i = 0; i < zombieCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y;

            Instantiate(zombiePrefab, pos, Quaternion.identity);
        }
    }
}
