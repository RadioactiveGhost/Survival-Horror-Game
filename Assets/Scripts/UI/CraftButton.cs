using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    Crafting_Manager CraftingUIManager;
    void Start()
    {
        CraftingUIManager = GameObject.FindGameObjectWithTag("CraftingLocal").GetComponent<Crafting_Manager>();
    }

    public void ButtonPress()
    {
        CraftingUIManager.Craft();
    }
}