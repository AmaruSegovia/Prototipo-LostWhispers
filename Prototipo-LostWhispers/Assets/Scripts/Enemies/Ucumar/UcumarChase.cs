using UnityEngine;
using UnityEngine.AI;

public class UcumarChase : MonoBehaviour
{
    public Transform player; // El objeto jugador al que seguir� Ucumar
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
