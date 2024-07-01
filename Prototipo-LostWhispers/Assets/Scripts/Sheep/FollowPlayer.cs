using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform player;  // Asigna el objeto jugador desde el inspector.
    public bool isCarried = false; // Indica si la oveja está siendo cargada.
    public float followDistance = 13f; // Distancia máxima a la que la oveja seguirá al jugador.
    private bool hasDetectedPlayer = false; // Indica si la oveja ha detectado al jugador.

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!isCarried && player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= followDistance)
            {
                hasDetectedPlayer = true; // Marca que el jugador ha sido detectado.
            }

            if (hasDetectedPlayer)
            {
                agent.enabled = true;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            agent.enabled = false;
        }
    }
}
