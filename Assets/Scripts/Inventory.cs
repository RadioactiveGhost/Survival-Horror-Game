using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    private RectTransform inventoryRect;

    private float inventoryWidth, inventoryHeight;

    public int nRows; // number of slot rows in the inventory

    public int nSlots; // total number of slots

    public float slotPadLeft, slotPadtop; // padding between slots

    public float slotSize; // the size of the slots on screen

    public GameObject slot; // for loading the slot prefab

    private List<GameObject> slots; // list for inventory

    private int emptySlot;
    // Start is called before the first frame update
    void Start()
    {
        CreateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreateInventory()
    {
        slots = new List<GameObject>();

        emptySlot = nSlots;

        inventoryWidth = (nSlots / nRows) * (slotSize + slotPadLeft) + slotPadLeft;

        inventoryHeight = nRows * (slotSize + slotPadLeft);

        inventoryRect = GetComponent<RectTransform>();

        //inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidth);
        //inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);

        int columns = nSlots / nRows;

        for (int i = 0; i < nRows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newSlot = (GameObject)Instantiate(slot);

                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";

                newSlot.transform.SetParent(this.transform.parent);

                slotRect.localPosition = inventoryRect.localPosition + new Vector3(slotPadLeft * (j + 1) + (slotSize * j), -slotPadtop * (i + 1) - (slotSize * i));

                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotSize);


                slots.Add(newSlot);
            }
        }
    }

    public bool Additem(Item item)
    {
        if (item.maxSize == 1)
        {
            PlaceEmpty(item);
            return true;
        }
        
        else
        {
            foreach (GameObject slot in slots)
            {
                Slot tmp = slot.GetComponent<Slot>();
                if (!tmp.IsEmpty)
                {
                    if (tmp.CurrentItem.type == item.type && tmp.isAvailable)
                    {
                        tmp.AddItem(item);
                        return true;
                    }

                }
            }
            if (emptySlot > 0)
            {
                PlaceEmpty(item);
            }
        }
        return false;
    }

    private bool PlaceEmpty(Item item)
    {
        if (emptySlot > 0)
        {
            foreach (GameObject slot in slots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (tmp.IsEmpty)
                {
                    tmp.AddItem(item);
                    emptySlot--;
                    return true;
                }
            }
        }

        return false;
    }
}
