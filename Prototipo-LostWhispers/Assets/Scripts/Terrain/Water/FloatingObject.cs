using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatStrength = 0.5f; // Fuerza del movimiento de flotación
    public float floatSpeed = 1.0f; // Velocidad del movimiento de flotación
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
