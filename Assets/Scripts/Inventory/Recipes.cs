using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class recipe
{
    public string name;
    public int amount;
    [HideInInspector]
    public Sprite sprite;
    public List<ingredient> ingredientNames;
    [HideInInspector]
    public List<ItemCount> ingredients;
    public void linkingredientsToItems()
    {
        ingredients = new List<ItemCount>();
        for (int i = 0; i < ingredientNames.Count; i++)
        {
            ingredients.Add(new ItemCount(Items.FindItemByString(ingredientNames[i].name), ingredientNames[i].number));
        }
        sprite = Items.FindItemByString(name).sprite;
    }
}

public class Recipes : MonoBehaviour
{
    public List<recipe> recipes;
    public static List<recipe> recipeInstance;

    private void Awake()
    {
        recipeInstance = recipes;
    }
}