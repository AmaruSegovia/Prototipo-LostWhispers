using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HordeBrain : MonoBehaviour
{
    [Header("Horde Setup")]
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] int zombieCount = 50;
    [SerializeField] float spawnRadius = 6f;

    [Header("Movement")]
    [SerializeField] float spacing = 1.2f;

    List<HordeFollower> followers = new List<HordeFollower>();

    NavMeshAgent agent;
    Transform player;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.autoRepath = false;
    }

    void Start()
    {
        SpawnZombies();
    }

    void Update()
    {
        agent.SetDestination(player.position);
        UpdateFollowers();
    }
    void SpawnZombies()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y;

            GameObject z = Instantiate(zombiePrefab, pos, Quaternion.identity);
            HordeFollower follower = z.GetComponent<HordeFollower>();

            follower.SetBrain(this);
            followers.Add(follower);
        }
    }
    void UpdateFollowers()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            Vector3 offset = Random.insideUnitSphere * spacing * (i * 0.1f);
            offset.y = 0f;
            Vector3 targetPos = transform.position - transform.forward * (i * 0.4f) + offset;
            followers[i].MoveTo(targetPos);
        }
    }
    public void Unregister(HordeFollower z)
    {
        followers.Remove(z);
    }
}