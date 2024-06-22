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
        var item = collision.gameObject.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, 1);
            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {

            inventory.Save();
            Debug.Log("aa");
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventory.Load();
            Debug.Log("bb");
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Clear();

    }
}
