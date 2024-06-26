using UnityEngine;

public class LowPolyWaterWaves : MonoBehaviour
{
    public float waveHeight = 0.5f; // Altura de las olas
    public float waveFrequency = 1.0f; // Frecuencia de las olas
    public float waveSpeed = 1.0f; // Velocidad de las olas
    public float waveNoiseScale = 0.5f; // Escala del ruido de las olas
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            float noise = Mathf.PerlinNoise(vertex.x * waveNoiseScale, vertex.z * waveNoiseScale);
            vertex.y = Mathf.Sin(Time.time * waveSpeed + originalVertices[i].x * waveFrequency) * waveHeight * noise;
            displacedVertices[i] = vertex;
        }

        meshFilter.mesh.vertices = displacedVertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
