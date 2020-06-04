using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Function
{
    Inventory,
    CraftingInventory,
    CraftingSlot,
    Hotbar
}

public class InvSlot : MonoBehaviour
{
    ItemCount iC;
    public Image image;
    public TextMeshProUGUI text;
    public Function purpose;
    Crafting_Manager craftingManager;
    Hotbar hotbar;

    private void Start()
    {
        craftingManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<Crafting_Manager>();
        hotbar = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<Hotbar>();
    }

    void SetImage(Sprite sprite)
    {
        image.enabled = true;
        image.sprite = sprite;
        //gameObject.GetComponent<Image>().sprite = i.sprite;
    }

    void SetText(int count)
    {
        text.enabled = true;
        text.text = count.ToString();
    }

    public void SetItem(ItemCount ic)
    {
        iC = ic;
        SetImage(iC.item.sprite);
        SetText(iC.count);
    }

    public void EmptyItem()
    {
        iC = null;
        image.enabled = false;
        text.enabled = false;
    }

    public void ButtonPress(Button b)
    {
        switch(purpose)
        {
            case Function.Inventory:
                hotbar.SetSlotCurrent(iC);
                break;

            case Function.CraftingSlot:
                craftingManager.SelectItem(b);
                break;

            case Function.CraftingInventory:
                craftingManager.SetItemManuallyOnCurrentIndex(iC);
                break;

            case Function.Hotbar:
                //Finish
                hotbar.EmptySlot(iC);
                break;
        }
    }

    public ItemCount GetItemCount()
    {
        return iC;
    }
}