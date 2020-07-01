using System.Collections.Generic;
using UnityEngine;

public class Item_grid : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject inventoryScriptGameObject;
    NewInventory inventoryScript;
    public GameObject slotsParent, slotsParent2;

    List<InvSlot> childrenSlots, childrenSlots2;

    private void Update()
    {
        //Not efficient CHANGE
        PopulateAll();

        if(Input.GetKeyDown(KeyCode.I))
        {
            InventoryMenu.SetActive(!InventoryMenu.activeSelf);
            if(InventoryMenu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void Start()
    {
        childrenSlots = new List<InvSlot>();
        childrenSlots2 = new List<InvSlot>();
        inventoryScript = inventoryScriptGameObject.GetComponent<NewInventory>();

        foreach (Transform child in slotsParent.transform)
        {
            if (child.GetComponent<InvSlot>())
            {
                childrenSlots.Add(child.GetComponent<InvSlot>());
            }
        }

        foreach (Transform child in slotsParent2.transform)
        {
            if (child.GetComponent<InvSlot>())
            {
                childrenSlots2.Add(child.GetComponent<InvSlot>());
            }
        }
    }

    void PopulateOnIndex(int index)
    {
        //childrenSlots[index].SetImage(inventory.items[index].item.sprite);
        childrenSlots[index].SetItem(inventoryScript.inventory[index]);
    }

    void PopulateAll()
    {
        for (int i = 0; i < childrenSlots.Count; i++)
        {
            if (i < inventoryScript.inventory.Count)
            {
                childrenSlots[i].SetItem(inventoryScript.inventory[i]);
            }
            else
            {
                childrenSlots[i].EmptyItem();
            }
        }

        for (int i = 0; i < childrenSlots2.Count; i++)
        {
            if (i < inventoryScript.inventory.Count)
            {
                childrenSlots2[i].SetItem(inventoryScript.inventory[i]);
            }
            else
            {
                childrenSlots2[i].EmptyItem();
            }
        }
    }
}