using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public Vector3 baseFlowDirection = new Vector3(1, 0, 0); // Dirección base del flujo del río
    public float baseFlowSpeed = 1.0f; // Velocidad base del flujo del río
    public float turbulenceStrength = 0.5f; // Fuerza de la turbulencia
    public float turbulenceFrequency = 1.0f; // Frecuencia de la turbulencia
    public float floatHeight = 1.0f; // Altura a la que el objeto debería flotar
    public float floatStrength = 10.0f; // Fuerza de flotación

    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcular la turbulencia
            Vector3 turbulence = new Vector3(
                Mathf.PerlinNoise(Time.time * turbulenceFrequency, 0) - 0.5f,
                0,
                Mathf.PerlinNoise(0, Time.time * turbulenceFrequency) - 0.5f
            ) * turbulenceStrength;

            // Dirección del flujo del río con turbulencia
            Vector3 flowDirection = baseFlowDirection + turbulence;

            // Aplicar la fuerza del flujo del río
            rb.AddForce(flowDirection * baseFlowSpeed);

            // Calcular la fuerza de flotación
            float waterSurfaceY = transform.position.y + floatHeight;
            float forceFactor = 1.0f - ((other.transform.position.y - waterSurfaceY) / floatHeight);
            if (forceFactor > 0)
            {
                Vector3 floatForce = -Physics.gravity * rb.mass * (forceFactor - rb.velocity.y * 0.5f);
                rb.AddForceAtPosition(floatForce, other.transform.position);
            }
        }
    }
}
