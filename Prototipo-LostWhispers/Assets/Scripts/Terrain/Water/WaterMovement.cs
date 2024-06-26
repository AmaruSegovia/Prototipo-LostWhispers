using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovementShader : MonoBehaviour
{
    public float speed = 0.1f; // Velocidad del desplazamiento de la textura
    private Renderer rend;
    private Vector2 offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = new Vector2(0, 0);
    }

    void Update()
    {
        offset.x += Time.deltaTime * speed;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
