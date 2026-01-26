using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField] float walkRadius = 10f;
    [SerializeField] Vector2 patrolIdleTimeRange = new Vector2(5f, 8f);
    [SerializeField, Range(0f, 1f)] float idleChance = 0.4f;
    [SerializeField] float viewDistance = 12f;

    float idleTimer;
    float currentIdleTime;
    bool waitingAtPoint;

    protected override void StateMachine()
    {
        DetectPlayer();

        switch (currentStatus)
        {
            case EnemyStatus.Patrol:
                StatePatrol();
                break;
            case EnemyStatus.Idle:
                StateIdle();
                break;
            case EnemyStatus.Chase:
                StateChase();
                break;
            case EnemyStatus.Dead:
                Died();
                break;
        }
    }

    void StatePatrol()
    {
        animator.SetBool("isWalk?", true);
        animator.SetBool("isRun?", false);

        agent.isStopped = false;

        if (!agent.hasPath && !waitingAtPoint)
        {
            Vector3 randomPos = RandomNavMeshLocation(walkRadius);
            agent.SetDestination(randomPos);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();

            if (Random.value <= idleChance)
            {
                waitingAtPoint = true;
                PickNewIdleTime();
                ChangeState(EnemyStatus.Idle);
            }
            else
            {
                waitingAtPoint = false;
            }
        }
    }

    void StateIdle()
    {
        agent.isStopped = true;
        idleTimer += Time.deltaTime;

        animator.SetBool("isWalk?", false);
        animator.SetBool("isRun?", false);

        if (idleTimer >= currentIdleTime)
        {
            idleTimer = 0f;
            waitingAtPoint = false;
            ChangeState(EnemyStatus.Patrol);
        }
    }

    void StateChase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        animator.SetBool("isWalk?", false);
        animator.SetBool("isRun?", true);
    }

    void DetectPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= viewDistance)
        {
            waitingAtPoint = false;
            ChangeState(EnemyStatus.Chase);
        }
        else if (currentStatus == EnemyStatus.Chase)
        {
            ChangeState(EnemyStatus.Patrol);
        }
    }

    void PickNewIdleTime()
    {
        currentIdleTime = Random.Range(patrolIdleTimeRange.x, patrolIdleTimeRange.y);
    }
}