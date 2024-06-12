using UnityEngine;

public class DeformTerrain : MonoBehaviour
{
    public Terrain terrain;
    public float radius = 5f; // Radio del área a deformar y pintar
    public float depth = 2f;  // Profundidad del agujero
    public int textureIndex = 1; // Índice de la textura que se aplicará alrededor del cráter
    public float smoothness = 1f; // Suavizado de la transición de la textura

    private float[,] originalHeights; // Para almacenar el mapa de alturas original
    private float[,,] originalAlphamaps; // Para almacenar los datos originales de texturas

    void Start()
    {
        // Almacenar el mapa de alturas original
        originalHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);

        // Almacenar los datos originales de texturas
        originalAlphamaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
    }

    public void CreateCrater(Vector3 position)
    {
        // Convertir la posición de mundo a la posición de coordenadas del terreno
        Vector3 terrainPosition = GetTerrainPosition(position);

        // Obtener el tamaño del área a deformar
        int xStart = Mathf.Clamp((int)(terrainPosition.x - radius), 0, terrain.terrainData.heightmapResolution - 1);
        int xEnd = Mathf.Clamp((int)(terrainPosition.x + radius), 0, terrain.terrainData.heightmapResolution - 1);
        int zStart = Mathf.Clamp((int)(terrainPosition.z - radius), 0, terrain.terrainData.heightmapResolution - 1);
        int zEnd = Mathf.Clamp((int)(terrainPosition.z + radius), 0, terrain.terrainData.heightmapResolution - 1);

        // Obtener el mapa de alturas del terreno
        float[,] heights = terrain.terrainData.GetHeights(xStart, zStart, xEnd - xStart, zEnd - zStart);

        // Deformar el terreno
        for (int x = 0; x < xEnd - xStart; x++)
        {
            for (int z = 0; z < zEnd - zStart; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2((xEnd - xStart) / 2, (zEnd - zStart) / 2)) / radius;
                if (distance < 1.0f)
                {
                    heights[z, x] -= depth * (1.0f - distance) / terrain.terrainData.size.y; // Ajuste para la escala de altura del terreno
                }
            }
        }

        // Aplicar los cambios al terreno
        terrain.terrainData.SetHeights(xStart, zStart, heights);

        // Pintar la textura alrededor del cráter
        PaintTexture(terrainPosition, xEnd - xStart, zEnd - zStart);
    }

    private void PaintTexture(Vector3 terrainPosition, int width, int height)
    {
        int alphamapWidth = terrain.terrainData.alphamapWidth;
        int alphamapHeight = terrain.terrainData.alphamapHeight;
        float[,,] alphamaps = terrain.terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        // Convertir la posición y el radio de deformación a coordenadas del mapa de texturas
        int xCenter = Mathf.RoundToInt(terrainPosition.x * alphamapWidth / terrain.terrainData.heightmapResolution);
        int zCenter = Mathf.RoundToInt(terrainPosition.z * alphamapHeight / terrain.terrainData.heightmapResolution);
        int texRadius = Mathf.RoundToInt(radius * alphamapWidth / terrain.terrainData.heightmapResolution);

        // Obtener el rango del área a pintar en el mapa de texturas
        int xStart = Mathf.Clamp(xCenter - texRadius, 0, alphamapWidth - 1);
        int xEnd = Mathf.Clamp(xCenter + texRadius, 0, alphamapWidth - 1);
        int zStart = Mathf.Clamp(zCenter - texRadius, 0, alphamapHeight - 1);
        int zEnd = Mathf.Clamp(zCenter + texRadius, 0, alphamapHeight - 1);

        for (int x = xStart; x <= xEnd; x++)
        {
            for (int z = zStart; z <= zEnd; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(xCenter, zCenter)) / texRadius;
                if (distance < 1.0f)
                {
                    float blend = Mathf.SmoothStep(0, 1, (1.0f - distance) * smoothness);
                    for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                    {
                        if (i == textureIndex)
                        {
                            alphamaps[z, x, i] = blend;
                        }
                        else
                        {
                            alphamaps[z, x, i] *= (1.0f - blend);
                        }
                    }
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphamaps);
    }

    private Vector3 GetTerrainPosition(Vector3 worldPosition)
    {
        Vector3 terrainPosition = new Vector3();
        terrainPosition.x = (worldPosition.x - terrain.transform.position.x) / terrain.terrainData.size.x * terrain.terrainData.heightmapResolution;
        terrainPosition.z = (worldPosition.z - terrain.transform.position.z) / terrain.terrainData.size.z * terrain.terrainData.heightmapResolution;
        return terrainPosition;
    }

    public void RestoreTerrain()
    {
        // Restaurar el mapa de alturas original
        terrain.terrainData.SetHeights(0, 0, originalHeights);

        // Restaurar los datos de texturas originales
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphamaps);
    }
}
