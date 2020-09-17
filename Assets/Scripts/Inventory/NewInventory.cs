using System.Collections.Generic;
using UnityEngine;

public class NewInventory : MonoBehaviour
{
    public int maxBagSlots;
    public int maxInSlot;

    [HideInInspector]
    public List<ItemCount> inventory;
    List<ItemCount> bcsUnity;

    bool once;
    void Start()
    {
        bcsUnity = new List<ItemCount>();
        once = true;
        inventory = new List<ItemCount>();
    }

    private void Update()
    {
        if(once)
        {
            //Test();
        }
    }

    void Test()
    {
        AddItem(Items.FindItemByString("Stick"), 20);
        AddItem(Items.FindItemByString("Metal"), 20);
        //AddItem(Items.FindItemByString("Pickaxe"), 1);
        once = !once;
    }

    public bool AddItem(item item, int count)
    {
        foreach (ItemCount itemC in inventory)
        {
            if (itemC.item == item)
            {
                itemC.count += count;
                if (itemC.count > maxInSlot)
                {
                    //Drop(itemC.item, itemC.count - maxInSlot); //drop extra
                    count = itemC.count - maxInSlot;
                    itemC.count = maxInSlot;
                    //Debug.Log("Maxed item count on slot " + item.name);
                    continue;
                }
                //Debug.Log("Existing slot was enough to house items " + item.name);
                return true;
            }
        }

        while (inventory.Count < maxBagSlots)
        {
            if (count > maxInSlot)
            {
                //Debug.Log("Maxed item count on new slot " + item.name);
                inventory.Add(new ItemCount(item, maxInSlot));
                count -= maxInSlot;
            }
            else
            {
                //Debug.Log("New slot was enough to house items " + item.name);
                inventory.Add(new ItemCount(item, count));
                return true;
            }
        }

        Drop(item, count);
        return false;
    }

    //FINISH
    public void Drop(item item, int count)
    {
        Debug.Log("Dropped " + count.ToString() + " " + item.name);
    }

    public void RemoveItem(item item, int count)
    {
        foreach (ItemCount itemC in inventory)
        {
            if (itemC.item == item)
            {
                itemC.count -= count;
                if (itemC.count <= 0)
                {
                    count = 0 - itemC.count;
                    itemC.count = 0;
                    bcsUnity.Add(itemC);
                    continue;
                }
                else
                {
                    count = 0;
                    break;
                }
            }
        }

        if(count > 0)
        {
            Debug.LogError("Tried to remove non-existant item");
        }

        foreach(ItemCount itemC in bcsUnity) //delete them
        {
            if(!inventory.Remove(itemC))
            {
                Debug.LogError("Failed to remove " + itemC.item.name);
            }
        }
        bcsUnity.Clear();
    }

    //UNTESTED
    public bool HasItem(item item, int count)
    {
        //Debug.Log(item.name + " " + count);
        int helper;
        foreach (ItemCount itemC in inventory)
        {
            helper = itemC.count;
            if (itemC.item == item)
            {
                helper -= count;
                if (helper < 0)
                {
                    count = itemC.count - helper;
                    //count = 0 - itemC.count;
                    //helper = 0;
                    continue;
                }
                else
                {
                    return true;
                }
            }
        }

        if (count > 0)
        {
            return false;
        }

        Debug.LogError("???");
        return false;
    }

    public ItemCount GetItem(item item)
    {
        foreach (ItemCount itemC in inventory)
        {
            if (itemC.item == item)
            {
                return itemC;
            }
        }

        return null;
    }

    public void AddItemsToInventory(List<ItemCount> list)
    {
        if(list == null)
        {
            Debug.LogError("This shouldn't happen");
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            item thing = Items.FindItemByString(list[i].item.name);
            AddItem(thing, list[i].count);
        }
    }
}