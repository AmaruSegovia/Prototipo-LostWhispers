using TreeEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.XR;

public abstract class Enemy : MonoBehaviour
{
    protected EnemyStatus currentStatus;
    protected float maxHealth;
    protected float currentHealth;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform target;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        ChangeState(EnemyStatus.Idle);
    }
    protected virtual void Update()
    {
        StateMachine();
    }
    protected abstract void StateMachine();
    protected virtual void ChangeState(EnemyStatus newStatus) 
    {
        currentStatus = newStatus;
    }
    public virtual void TakeDamage(float damage) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            ChangeState(EnemyStatus.Dead);
        }
    }
    protected virtual void Died() 
    {
        Destroy(gameObject);
    }
    protected Vector3 RandomNavMeshLocation(float radius) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);
        return hit.position;
    }
}