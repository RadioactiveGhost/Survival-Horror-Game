using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class item
{
    public string name;
    public Sprite sprite;
    public bool ingredient;
}

[System.Serializable]
public class ingredient
{
    public string name;
    public int number;
}

[System.Serializable]
public class ItemCount
{
    public item item;
    public int count;

    public ItemCount(item item, int count)
    {
        this.count = count;
        this.item = item;
    }
}

public class Items : MonoBehaviour
{
    public static List<item> itemsinstance;
    public List<item> items;

    private void Awake()
    {
        itemsinstance = items;

        for (int i = 0; i < Recipes.recipeInstance.Count; i++)
        {
            Recipes.recipeInstance[i].linkingredientsToItems();
        }
    }

    public static item FindItemByString(string s)
    {
        return itemsinstance.Find(item => item.name == s);
    }
}