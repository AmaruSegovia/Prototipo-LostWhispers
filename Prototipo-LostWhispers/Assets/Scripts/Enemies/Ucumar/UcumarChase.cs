using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float patrolRadius = 10f; // Radio de patrullaje
    public float patrolInterval = 10f; // Tiempo entre cambio de posiciones de patrullaje
    public float detectionRange = 50f;
    public float attackRange = 2f;
    public Animator animator;
    public Transform player;

    private NavMeshAgent agent;
    private bool isChasing;
    private bool isShouting;
    private Vector3 patrolTarget;
    private float patrolTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomPatrolTarget();
    }

    void Update()
    {
        if (!isChasing && !isShouting)
        {
            Patrol();
        }
        else if (isChasing)
        {
            ChasePlayer();
        }

        DetectPlayer();
    }

    void Patrol()
    {
        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolInterval || agent.remainingDistance < 0.5f)
        {
            SetRandomPatrolTarget();
            patrolTimer = 0f;
        }

        agent.destination = patrolTarget;

        if (agent.remainingDistance > 0.5f)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, -1);
        patrolTarget = navHit.position;
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= detectionRange && !isChasing && !isShouting)
        {
            isShouting = true;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            LookAtPlayer(); // Asegúrate de que el enemigo mire al jugador al iniciar el grito
            animator.SetTrigger("Shout");
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Mantener solo la rotación en el plano horizontal
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void JumpTowardsPlayer()
    {
        Vector3 jumpTarget = player.position + (transform.position - player.position).normalized * 10f; // Ajusta la distancia del salto según sea necesario
        agent.destination = jumpTarget;
        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);
        Invoke("StartChasing", 3f); // Ajusta el tiempo para que coincida con la animación de salto
    }

    void StartChasing()
    {
        isChasing = true;
        isShouting = false;
        agent.isStopped = false;
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }
}
