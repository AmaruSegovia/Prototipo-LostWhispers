using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatoryMovementAndEmittedLight : MonoBehaviour
{
    [SerializeField] float amplitude;
    [SerializeField] float frequency;
    [SerializeField] Light lightOrb;
    private void Update()
    {
        OscillatoryMovementY();
        effectPingPongLight();
    }
    void OscillatoryMovementY()
    {
        float time = Time.time;
        Vector3 actualPosition = transform.position;
        actualPosition.y += amplitude * Mathf.Sin(time * frequency) * Time.deltaTime;
        transform.position = actualPosition;
    }
    void effectPingPongLight()
    {
        lightOrb.intensity = Mathf.PingPong(Time.time, 1);
    }
}