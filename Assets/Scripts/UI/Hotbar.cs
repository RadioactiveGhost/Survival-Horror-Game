using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public GameObject hotbar;
    [HideInInspector]
    public int selected;
    InvSlot[] slots;
    //ItemCount[] items;
    private void Start()
    {
        slots = hotbar.GetComponentsInChildren<InvSlot>();
        selected = 0;
        Select(0);
        //items = new ItemCount[slots.Length];
        UpdateBar();
    }

    private void Update()
    {
        //CHANGE
        UpdateBar();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Select(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Select(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Select(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Select(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Select(4);
        }
    }

    void Select(int selection)
    {
        slots[selected].gameObject.GetComponent<Image>().color = Color.white;
        slots[selection].gameObject.GetComponent<Image>().color = Color.grey;
        selected = selection;
    }

    void UpdateBar()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetItemCount() != null && slots[i].GetItemCount().count != 0)
            {
                SetSlot(slots[i].GetItemCount(), i);
            }
            else
            {
                EmptySlot(i);
            }
        }
    }

    void SetSlot(ItemCount itemCount, int index)
    {
        //Debug.Log("Item " + itemCount.item.name + " on " + index);
        slots[index].SetItem(itemCount);
    }

    void EmptySlot(int index)
    {
        //Debug.Log("Emptying " + index);
        slots[index].EmptyItem();
    }

    public void SetSlotCurrent(ItemCount itemCount)
    {
        SetSlot(itemCount, selected);
    }

    public void EmptySlot(ItemCount itemCount)
    {
        if(itemCount == null)
        {
            Debug.LogError("NULL");
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            //Debug.Log(slots[i].GetItemCount().item.name + " on " + i);
            if (itemCount == slots[i].GetItemCount())
            {
                EmptySlot(i);
                return;
            }
        }
        Debug.LogError("No matches found to remove from hotbar");
    }
}