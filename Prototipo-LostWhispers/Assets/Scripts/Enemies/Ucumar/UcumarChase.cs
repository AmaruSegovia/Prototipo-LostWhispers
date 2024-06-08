using UnityEngine;
using UnityEngine.AI;

public class UcumarChase : MonoBehaviour
{
    public Transform player; // El objeto jugador al que seguirá Ucumar
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
