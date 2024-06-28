using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatoryMovementAndEmittedLight : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;
    private void Update()
    {
        OscillatoryMovementY();
    }
    void OscillatoryMovementY()
    {
        float time = Time.time;
        Vector3 actualPosition = transform.position;
        actualPosition.y += amplitude * Mathf.Sin(time * frequency) * Time.deltaTime;
        transform.position = actualPosition;
    }
}
