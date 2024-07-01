using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarrySheep : MonoBehaviour
{
    public Transform carryPosition; // Posici�n donde la oveja ser� cargada.
    public float angleOffset = 45f; // �ngulo fijo respecto al frente del jugador.
    private GameObject carriedSheep; // Referencia a la oveja que se est� cargando.

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

        // Si estamos cargando una oveja, aseg�rate de que est� en la posici�n correcta.
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
        sheep.GetComponent<Rigidbody>().isKinematic = true; // Desactiva la f�sica de la oveja.
        sheep.transform.parent = carryPosition;

        // Desactiva el seguimiento.
        FollowPlayer followScript = sheep.GetComponent<FollowPlayer>();
        if (followScript != null)
        {
            followScript.isCarried = true;
        }

        // Aseg�rate de que la oveja est� en la posici�n correcta.
        UpdateSheepPositionAndRotation();
    }

    void DropSheep()
    {
        if (carriedSheep != null)
        {
            carriedSheep.GetComponent<Rigidbody>().isKinematic = false; // Activa la f�sica de la oveja.
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
        // Aseg�rate de que la oveja est� en la posici�n y rotaci�n correctas cada frame.
        carriedSheep.transform.position = carryPosition.position;

        // Calcula la rotaci�n fija respecto al jugador.
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angleOffset, 0);
        carriedSheep.transform.rotation = targetRotation;
    }
}
