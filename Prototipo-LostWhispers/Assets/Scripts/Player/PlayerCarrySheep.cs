using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarrySheep : MonoBehaviour
{
    public Transform carryPosition; // Posición donde la oveja será cargada.
    public float angleOffset = 45f; // Ángulo fijo respecto al frente del jugador.
    private GameObject carriedSheep; // Referencia a la oveja que se está cargando.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedSheep != null)
            {
                DropSheep();
            }
            else
            {
                TryPickUpSheep();
            }
        }

        // Si estamos cargando una oveja, asegúrate de que está en la posición correcta.
        if (carriedSheep != null)
        {
            UpdateSheepPositionAndRotation();
        }
    }

    void TryPickUpSheep()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Sheep"))
            {
                PickUpSheep(hit.collider.gameObject);
            }
        }
    }

    void PickUpSheep(GameObject sheep)
    {
        carriedSheep = sheep;
        sheep.GetComponent<Rigidbody>().isKinematic = true; // Desactiva la física de la oveja.
        sheep.transform.parent = carryPosition;

        // Desactiva el seguimiento.
        FollowPlayer followScript = sheep.GetComponent<FollowPlayer>();
        if (followScript != null)
        {
            followScript.isCarried = true;
        }

        // Asegúrate de que la oveja está en la posición correcta.
        UpdateSheepPositionAndRotation();
    }

    void DropSheep()
    {
        if (carriedSheep != null)
        {
            carriedSheep.GetComponent<Rigidbody>().isKinematic = false; // Activa la física de la oveja.
            carriedSheep.transform.parent = null;

            // Reactiva el seguimiento.
            FollowPlayer followScript = carriedSheep.GetComponent<FollowPlayer>();
            if (followScript != null)
            {
                followScript.isCarried = false;
            }

            carriedSheep = null;
        }
    }

    void UpdateSheepPositionAndRotation()
    {
        // Asegúrate de que la oveja está en la posición y rotación correctas cada frame.
        carriedSheep.transform.position = carryPosition.position;

        // Calcula la rotación fija respecto al jugador.
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angleOffset, 0);
        carriedSheep.transform.rotation = targetRotation;
    }
}
