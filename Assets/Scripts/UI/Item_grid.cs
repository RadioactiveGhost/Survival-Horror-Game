using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_grid : MonoBehaviour
{
    public GameObject inventoryScriptGameObject;
    NewInventory inventory;
    public GameObject slotsParent, slotsParent2;

    List<InvSlot> childrenSlots, childrenSlots2;

    private void Update()
    {
        //Not efficient
        PopulateAll();
    }

    void Start()
    {
        childrenSlots = new List<InvSlot>();
        childrenSlots2 = new List<InvSlot>();
        inventory = inventoryScriptGameObject.GetComponent<NewInventory>();

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
        childrenSlots[index].SetItem(inventory.inventory[index]);
    }

    void PopulateAll()
    {
        for (int i = 0; i < childrenSlots.Count; i++)
        {
            if (i < inventory.inventory.Count)
            {
                childrenSlots[i].SetItem(inventory.inventory[i]);
            }
            else
            {
                childrenSlots[i].EmptyItem();
            }
        }

        for (int i = 0; i < childrenSlots2.Count; i++)
        {
            if (i < inventory.inventory.Count)
            {
                childrenSlots2[i].SetItem(inventory.inventory[i]);
            }
            else
            {
                childrenSlots2[i].EmptyItem();
            }
        }
    }
}