using UnityEngine;

public class HordeFollower : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;

    Vector3 targetPosition;
    HordeBrain brain;

    public void SetBrain(HordeBrain h)
    {
        brain = h;
    }

    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        Vector3 dir = targetPosition - transform.position;
        if (dir.sqrMagnitude > 0.01f)
            transform.forward = dir.normalized;
    }

    void OnDisable()
    {
        if (brain != null)
            brain.Unregister(this);
    }
}