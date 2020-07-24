﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpManager : MonoBehaviour
{
    Camera cameraRef;
    public float pickUpMaxDist;
    public LayerMask mask;
    public GameObject pointerGameObject, hotbarGameObject, InventoryGameObject;
    public Sprite defaultSprite, pickUpSprite;
    Image pointerImage;
    Hotbar hotbar;
    NewInventory inventory;
    [HideInInspector]
    public GameObject pickUpPossible;

    private void Start()
    {
        cameraRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        pointerImage = pointerGameObject.GetComponent<Image>();
        hotbar = hotbarGameObject.GetComponent<Hotbar>();
        inventory = InventoryGameObject.GetComponent<NewInventory>();
    }

    private void Update()
    {
        pickUpPossible = RaycastResource();
    }

    public GameObject RaycastResource()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out hit, pickUpMaxDist, mask))
        {
            //Debug.Log(hit.transform.root.gameObject.tag);
            if (hit.transform.root.gameObject.tag == "PickUp")
            {
                pointerImage.sprite = pickUpSprite;
                return hit.transform.root.gameObject;
            }
        }
        pointerImage.sprite = defaultSprite;
        return null;
    }

    public void PickUpResource()
    {
        if (pickUpPossible)
        {
            Pickable p = pickUpPossible.GetComponent<Pickable>();
            string test = hotbar.GetItemSelectedName();
            List<ItemCount> itemlist = p.AttemptToPickUp(test);
            if (itemlist != null)
            {
                inventory.AddItemsToInventory(itemlist);
                Destroy(pickUpPossible);
            }
        }
        else
        {
            Debug.Log("Can't pick up anything");
        }
    }
}