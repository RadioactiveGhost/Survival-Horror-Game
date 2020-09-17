using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Crafting_Manager : MonoBehaviour
{
    GameObject CraftingList;
    GameObject[] craftItemSlots;
    ItemCount[] itemsOnSlots;
    int selected;
    [HideInInspector]
    public recipe craftable;
    NewInventory inventory;

    private void Start()
    {
        CraftingList = GameObject.FindGameObjectWithTag("RecipeList");
        inventory = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<NewInventory>();

        craftItemSlots = GameObject.FindGameObjectsWithTag("CraftSlots");
        itemsOnSlots = new ItemCount[craftItemSlots.Length];

        craftable = null;

        selected = 0;
        Select(0);
        UpdateRecipeList();

        foreach (GameObject g in craftItemSlots)
        {
            g.transform.Find("Icon").GetComponent<Image>().enabled = false;
            g.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = false;
        }

        StartCoroutine(NextFrameDisable());
    }

    IEnumerator NextFrameDisable()
    {
        yield return 0;

        GameObject.FindGameObjectWithTag("CraftMenu").SetActive(false);
    }

    public void UpdateRecipeList()
    {
        bool pass;
        CraftSlot[] temp = CraftingList.GetComponentsInChildren<CraftSlot>();
        for (int i = 0; i < Recipes.recipeInstance.Count; i++)
        {
            pass = false;
            for (int j = 0; j < temp.Length; j++)
            {
                if (temp[j].GetRecipe().Equals(Recipes.recipeInstance[i])) //found list member on recipe list
                {
                    pass = true;
                    break;
                }
            }
            if (!pass) //instantiate new list member
            {
                GameObject g = GameObject.Instantiate((GameObject)Resources.Load("UI/Slot_Crafting"));
                g.transform.SetParent(CraftingList.transform, false);
                g.GetComponent<CraftSlot>().SetRecipe(Recipes.recipeInstance[i]);
                g.transform.Find("Image").Find("Icon").GetComponent<Image>().sprite = Recipes.recipeInstance[i].sprite;
                g.transform.GetComponentInChildren<TextMeshProUGUI>().text = Recipes.recipeInstance[i].name;
            }
        }
    }

    public void SetItemManuallyOnCurrentIndex(ItemCount itemCount) //button press
    {
        SetSlot(selected, itemCount);

        if (selected < craftItemSlots.Length - 2) //1 less bcs array, 1 less bcs last slot can't set stuff there
        {
            Select(selected + 1);
        }
        else
        {
            Select(0);
        }
    }

    public void SelectItem(Button b)
    {
        int i = 0;
        foreach (GameObject g in craftItemSlots)
        {
            if (b.gameObject == g)
            {
                if (selected == i) //Empty
                {
                    EmptySlot(i);
                }
                else //select
                {
                    Select(i);
                }
                return;
            }
            i++;
        }
        Debug.LogError("No gameObject found for button " + b.gameObject.name);
    }

    public void Select(int selection)
    {
        //Debug.Log(selected + " exists? Max length " + craftItemSlots.Length);
        craftItemSlots[selected].GetComponent<Image>().color = Color.white;
        craftItemSlots[selection].GetComponent<Image>().color = Color.grey;
        selected = selection;
    }

    public void SetSlot(int index, ItemCount itemCount)
    {
        //Debug.Log("Item " + itemCount.item.name + " on " + index);
        craftItemSlots[index].transform.Find("Icon").GetComponent<Image>().enabled = true;
        try
        {
            craftItemSlots[index].transform.Find("Icon").GetComponent<Image>().sprite = itemCount.item.sprite;
        }
        catch
        {
            Debug.LogError("Sprite missing for " + itemCount.item.name);
        }
        craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = true;
        craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = itemCount.count.ToString();
        itemsOnSlots[index] = itemCount;

        if (index != craftItemSlots.Length - 1) //no need to recipe search on last slot
        {
            PossibleRecipes();
        }
    }

    public void SetSlot(int index, ItemCount itemCount, bool able)
    {
        //Debug.Log("Item " + itemCount.item.name + " on " + index);
        craftItemSlots[index].transform.Find("Icon").GetComponent<Image>().enabled = true;
        try
        {
            craftItemSlots[index].transform.Find("Icon").GetComponent<Image>().sprite = itemCount.item.sprite;
        }
        catch
        {
            Debug.LogError("Sprite missing for " + itemCount.item.name);
        }
        craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = true;
        craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = itemCount.count.ToString();
        itemsOnSlots[index] = itemCount;

        if (index != craftItemSlots.Length - 1) //no need to recipe search on last slot
        {
            PossibleRecipes();
        }

        if(able)
        {
            craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
    }

    public void EmptySlot(int index)
    {
        //Debug.Log("Null on " + index);
        craftItemSlots[index].transform.Find("Icon").GetComponent<Image>().enabled = false;
        craftItemSlots[index].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = false;
        itemsOnSlots[index] = null;
        if (index != craftItemSlots.Length - 1) //no need to recipe search on last slot
        {
            PossibleRecipes();
        }
    }

    public void SetRecipeOnCraftSlots(recipe r)
    {
        bool recipepossible = true;
        int i = 0;
        for (; i < r.ingredients.Count; i++)
        {
            //Debug.Log("Slot " + i + " with " + r.ingredients[i].item.name);
            if (inventory.HasItem(r.ingredients[i].item, r.ingredients[i].count))
            {
                SetSlot(i, inventory.GetItem(r.ingredients[i].item), true);
            }
            else
            {
                SetSlot(i, r.ingredients[i], false);
                recipepossible = false;
            }
        }
        for (; i < craftItemSlots.Length; i++)
        {
            //Debug.Log("Slot " + i + " empty");
            EmptySlot(i);
        }

        //set last slot with recipe
        //SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount));
        PossibleRecipeTest(r, recipepossible);
    }

    public void PossibleRecipeTest(recipe r) //NOT WORKING
    {
        int passes, count;

        passes = 0;
        for (int k = 0; k < r.ingredients.Count; k++)
        {
            count = 0;
            for (int j = 0; j < itemsOnSlots.Length; j++)
            {
                if (itemsOnSlots[j] != null)
                {
                    if (itemsOnSlots[j].item == r.ingredients[k].item)
                    {
                        count += itemsOnSlots[j].count;
                    }
                }
            }
            if (count >= r.ingredients[k].count)
            {
                //Debug.Log("Enough " + Recipes.recipeInstance[i].ingredients[k].item.name + " (" + count + "/" + Recipes.recipeInstance[i].ingredients[k].count + ")" + "for " + Recipes.recipeInstance[i].name);
                passes++;
            }
        }

        //See if it passes
        if (passes >= r.ingredients.Count)
        {
            SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount), true);
            craftable = r;
        }
        else
        {
            SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount), false);
            //craftItemSlots[craftItemSlots.Length - 1].transform.Find("Icon").GetComponent<Image>().enabled = false;
            //craftItemSlots[craftItemSlots.Length - 1].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = false;
            craftable = null;
        }
    }

    public void PossibleRecipeTest(recipe r, bool possible)
    {
        //See if it passes
        if (possible)
        {
            SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount), true);
            craftable = r;
        }
        else
        {
            SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount), false);
            //craftItemSlots[craftItemSlots.Length - 1].transform.Find("Icon").GetComponent<Image>().enabled = false;
            //craftItemSlots[craftItemSlots.Length - 1].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().enabled = false;
            craftable = null;
        }
    }

    public void PossibleRecipes()
    {
        bool possible = false;
        recipe r = new recipe();
        //int passes, count;

        for (int i = 0; i < Recipes.recipeInstance.Count; i++)
        {
            //passes = 0;
            for (int k = 0; k < Recipes.recipeInstance[i].ingredients.Count; k++)
            {
                //count = 0;
                for (int j = 0; j < itemsOnSlots.Length; j++)
                {
                    if (itemsOnSlots[j] != null && itemsOnSlots[j].item == Recipes.recipeInstance[i].ingredients[k].item && itemsOnSlots[j].count >= Recipes.recipeInstance[i].ingredients[k].count)
                    {
                        if (k == Recipes.recipeInstance[i].ingredients.Count - 1)
                        {
                            r = Recipes.recipeInstance[i];
                            possible = true;
                            break;
                        }
                        continue;
                    }
                    break;
                }
                if (possible)
                {
                    break;
                }
            }
            if (possible)
            {
                break;
            }
        }

        //See if it passes
        if (possible)
        {
            SetSlot(craftItemSlots.Length - 1, new ItemCount(Items.FindItemByString(r.name), r.amount));
            craftable = r;
        }
        else
        {
            EmptySlot(craftItemSlots.Length - 1);
            craftable = null;
        }
    }

    public void Craft()
    {
        if (craftable != null)
        {
            if (inventory.AddItem(itemsOnSlots[itemsOnSlots.Length - 1].item, itemsOnSlots[itemsOnSlots.Length - 1].count))
            {
                for (int i = 0; i < craftable.ingredients.Count; i++) //exclude last
                {
                    //Debug.Log(craftable.name);
                    //Debug.Log("Trying to remove " + " " + craftable.ingredients[i].count + " " +  craftable.ingredients[i].item.name);
                    inventory.RemoveItem(craftable.ingredients[i].item, craftable.ingredients[i].count);
                }
            }
            else
            {
                Debug.LogError("Couldn't craft item, full bag?");
            }
        }
        UpdateCraftSlot();
        PossibleRecipes();
    }

    public void UpdateCraftSlot()
    {
        for (int i = 0; i < craftItemSlots.Length; i++)
        {
            if (itemsOnSlots[i] != null && itemsOnSlots[i].count > 0)
            {
                craftItemSlots[i].transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = itemsOnSlots[i].count.ToString();
            }
            else
            {
                EmptySlot(i);
            }
        }
    }
}