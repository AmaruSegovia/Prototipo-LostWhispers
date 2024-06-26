using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarrySheep : MonoBehaviour
{
    public Transform carryPosition; // Posici�n donde la oveja ser� cargada
    public float angleOffset = 45f; // �ngulo fijo respecto al frente del jugador
    private GameObject carriedSheep; // Referencia a la oveja que se est� cargando

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
        sheep.GetComponent<Rigidbody>().isKinematic = true; // Desactiva la f�sica de la oveja
        sheep.transform.position = carryPosition.position;

        // Calcula la rotaci�n fija respecto al jugador
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angleOffset, 0);
        sheep.transform.rotation = targetRotation;

        sheep.transform.parent = carryPosition;
    }

    void DropSheep()
    {
        carriedSheep.GetComponent<Rigidbody>().isKinematic = false; // Activa la f�sica de la oveja
        carriedSheep.transform.parent = null;
        carriedSheep = null;
    }
}
