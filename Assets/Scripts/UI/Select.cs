using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Select : MonoBehaviour 
{

    private GameObject inventoryManager;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {        
        inventoryManager = GameObject.FindGameObjectWithTag("ProgressManager");
        inventory = inventoryManager.GetComponent<Inventory>();
    }

    // Update is called once per frame
    public void OnMouseDown()
    {        
        inventory.targetItem = this.gameObject;
        if (gameObject.tag == "InventorySlot")
        {
            inventory.ShowMenu();
        }
        else
        {
            inventory.ShowEquipMenu();
        }        
    }    
}
