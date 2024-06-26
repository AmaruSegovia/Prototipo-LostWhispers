using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventorySystem : MonoBehaviour
{
    public InventoryObject inventory;

    // Este método se llamará cuando ocurra una colisión
    private void OnCollisionEnter(Collision collision)
    {
        var item = collision.gameObject.GetComponent<GroundItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.item), 1);
            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {

            inventory.Save();
            Debug.Log("Guardado");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventory.Load();
            Debug.Log("Cargado");
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Items.Clear();

    }
}
