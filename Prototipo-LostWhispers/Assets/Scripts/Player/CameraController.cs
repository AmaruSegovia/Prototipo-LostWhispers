using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform flashlight;
    public Transform _camera;
    public float cameraSensitivity = 200.0f;
    public float flashlightDelay = 0.5f; // Ajusta este valor para el efecto de retraso

    private Vector3 lastCameraPosition;
    private Vector3 flashlightRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lastCameraPosition = _camera.position;
        flashlightRotation = flashlight.localEulerAngles;
    }

    void Update()
    {
        Vector3 currentCameraPosition = _camera.position;

        Vector3 cameraMovement = currentCameraPosition - lastCameraPosition;

        float mouseX = cameraMovement.x * cameraSensitivity * Time.deltaTime;
        float mouseY = cameraMovement.y * cameraSensitivity * Time.deltaTime;

        flashlightRotation += new Vector3(-mouseY, mouseX, 0) * flashlightDelay;
        flashlightRotation.x = Mathf.Clamp(flashlightRotation.x, -90.0f, 90.0f);

        flashlight.localRotation = Quaternion.Euler(flashlightRotation);

        lastCameraPosition = currentCameraPosition;
    }
}