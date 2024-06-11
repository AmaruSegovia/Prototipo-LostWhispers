using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float lookRadius = 50f;
    public float coneVisionRadius = 30f; // Radio del cono de visión
    public float attackRadius = 5f;
    public float patrolRadius = 20f; // Radio dentro del cual el enemigo patrulla aleatoriamente
    public float idleTime = 10f; // Tiempo que el enemigo se queda quieto
    public float jumpHeight = 10f; // Altura del salto
    public float visionConeAngle = 45f; // Ángulo del cono de visión
    public float chaseSpeed = 100f; // Velocidad de persecución

    private Animator animator;
    private NavMeshAgent agent;
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool isChasing = false;
    private PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = 8;
        agent.acceleration = 8f;
        agent.angularSpeed = 360f;
        agent.stoppingDistance = attackRadius;

        // Obtener referencia al script del jugador
        playerController = player.GetComponent<PlayerController>();

        // Start the first patrol
        PatrolToRandomPoint();
    }

    void PatrolToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            Vector3 finalPosition = hit.position;
            agent.SetDestination(finalPosition);
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
    }

    void Update()
    {
        if (isChasing)
        {
            agent.SetDestination(player.position);
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (distanceToPlayer <= attackRadius)
            {
                AttackPlayer();
            }
            else
            {
                animator.SetBool("isAttacking", false);
                animator.SetBool("isWalking", true);
                agent.isStopped = false;
            }
        }
        else
        {
            float distance = Vector3.Distance(player.position, transform.position);

            // Calcular si el jugador está dentro del cono de visión
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // Revisar si el jugador está dentro del rango esférico o dentro del cono de visión
            bool isInLookRadius = distance <= lookRadius;
            bool isInConeVision = distance <= coneVisionRadius && angleToPlayer <= visionConeAngle / 2f;

            if (isInLookRadius || isInConeVision)
            {
                agent.isStopped = true;
                FacePlayer();
                animator.SetTrigger("Shout");
            }
            else
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!isIdle)
                    {
                        if (Random.value > 0.5f)
                        {
                            isIdle = true;
                            idleTimer = idleTime;
                            animator.SetBool("isWalking", false);
                            animator.SetBool("isIdle", true);
                        }
                        else
                        {
                            PatrolToRandomPoint();
                        }
                    }
                }

                if (isIdle)
                {
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0f)
                    {
                        isIdle = false;
                        PatrolToRandomPoint();
                    }
                }

                if (agent.velocity.sqrMagnitude > 0f)
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
        }
    }

    void JumpAttack()
    {
        animator.SetTrigger("JumpAttack");
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", false);
        StartCoroutine(ExecuteJumpAttack());
    }

    private IEnumerator ExecuteJumpAttack()
    {
        // Elevar al enemigo 10 metros hacia arriba
        Vector3 jumpTarget = transform.position + Vector3.up * 10;
        float jumpDuration = 1f; // Duración del salto hacia arriba
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(transform.position, jumpTarget, elapsedTime / jumpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Caer hacia la posición del jugador
        Vector3 fallTarget = player.position;
        float fallDuration = 0.7f; // Duración de la caída
        elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            transform.position = Vector3.Lerp(jumpTarget, fallTarget, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Llamar a la sacudida de cámara del jugador
        playerController.ShakeCamera(1.55f,0.7f);
        // Asegurarse de que el enemigo llegue exactamente a la posición del jugador
        transform.position = fallTarget;

        ApplyExplosionForce();

    }


    private void ApplyExplosionForce()
    {
        float explosionRadius = 10f;
        float explosionForce = 200f;
        Vector3 explosionPosition = transform.position;

        float distanceToPlayer = Vector3.Distance(explosionPosition, player.position);

        if (distanceToPlayer <= explosionRadius)
        {
            Vector3 explosionDirection = (player.position - explosionPosition).normalized;
            playerController.ApplyExplosionForce(explosionDirection, explosionForce);
        }
    }


    void MoverCamaraGrito() 
    {
        playerController.ShakeCamera(0.09f, 1.95f);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        FacePlayer();
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
    }


    void StartChasing()
    {
        agent.isStopped = false;
        animator.SetBool("isRunning", true);
        // Actualizar constantemente el destino del agente al jugador
        isChasing = true;
        // Llamar a FacePlayer() si quieres que el enemigo gire hacia el jugador mientras lo persigue
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Dibujar el cono de visión
        Vector3 forward = transform.forward * coneVisionRadius;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionConeAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionConeAngle / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    void OnTriggerEnter(Collider other)
    {
        if (agent.stoppingDistance == 3)
        {
            animator.SetTrigger("Attack");
        }
    }
}
