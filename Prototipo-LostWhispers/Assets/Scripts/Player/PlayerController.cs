using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public GameObject flashlight_player;
    public Camera playerCamera;
    Vector3 moveDirection = Vector3.zero;
    bool isShaking = false;

    [HideInInspector]
    //public bool canMove = true;
    void Update()
    {
        /*LINTERNA*/
        if (Input.GetKeyDown(KeyCode.F) && flashlight_player == enabled)
        {
            flashlight_player.SetActive(!flashlight_player.activeSelf);
        }
        if (flashlight_player == enabled)
        {
            flashlight_player.transform.forward = playerCamera.transform.forward;
        }

    }

    public void ShakeCamera(float shakeMagnitude, float shakeDuration, float shakeDelay = 0f)
    {
        if (!isShaking)
        {
            StartCoroutine(Shake(shakeMagnitude, shakeDuration, shakeDelay));
        }
    }

    public void ApplyExplosionForce(Vector3 direction, float force)
    {
        direction.y = 0.5f;
        moveDirection = direction.normalized * force;
    }


    private IEnumerator Shake(float shakeMagnitude, float shakeDuration, float shakeDelay)
    {
        isShaking = true;

        if (shakeDelay > 0f)
        {
            yield return new WaitForSeconds(shakeDelay);
        }

        Vector3 originalPosition = playerCamera.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        playerCamera.transform.localPosition = originalPosition;
        isShaking = false;
    }
}
