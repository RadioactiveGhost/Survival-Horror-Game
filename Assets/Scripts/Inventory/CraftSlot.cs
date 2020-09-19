using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour
{
    Crafting_Manager craftingManager;
    recipe r;

    private void Start()
    {
        craftingManager = GameObject.FindGameObjectWithTag("CraftingLocal").GetComponent<Crafting_Manager>();
    }

    public void OnButtonClick()
    {
        craftingManager.SetRecipeOnCraftSlots(r);
    }

    public void SetRecipe(recipe r)
    {
        this.r = r;
    }

    public recipe GetRecipe()
    {
        return r;
    }
}