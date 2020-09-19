using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PickUpManager : MonoBehaviour
{
    Camera cameraRef;
    public float pickUpMaxDist;
    public LayerMask mask;
    public Sprite defaultSprite, pickUpSprite;
    Image pointerImage;
    Hotbar hotbar;
    NewInventory inventory;
    GameObject pickUpPossible;
    [HideInInspector]
    public bool CheckPickups;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Inventory_Test_Scene")
        {
            CheckPickups = true;
            cameraRef = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            pointerImage = GameObject.FindGameObjectWithTag("Pointer").GetComponent<Image>();
            hotbar = gameObject.GetComponent<Hotbar>();
            inventory = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<NewInventory>();
        }
        else
        {
            CheckPickups = false;
        }
    }

    private void Update()
    {
        if (CheckPickups)
        {
            pickUpPossible = RaycastResource();
        }
    }

    public GameObject RaycastResource()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out hit, pickUpMaxDist, mask))
        {
            //Debug.Log(hit.transform.root.gameObject.tag);
            if (hit.transform.tag == "PickUp")
            {
                pointerImage.sprite = pickUpSprite;
                return hit.transform.gameObject;
            }
            //Debug.Log("Hit " + hit.transform.name);
        }
        pointerImage.sprite = defaultSprite;
        return null;
    }

    public void PickUpResource()
    {
        if (pickUpPossible)
        {
            Pickable[] pickables = pickUpPossible.GetComponents<Pickable>();
            for (int i = 0; i < pickables.Length; i++)
            {
                Pickable p = pickables[i];
                string test = hotbar.GetItemSelectedName();
                List<ItemCount> itemlist = p.AttemptToPickUp(test);
                if (itemlist != null)
                {
                    inventory.AddItemsToInventory(itemlist);
                    Destroy(pickUpPossible);
                }
            }
        }
        else
        {
            //Debug.Log("Can't pick up anything");
        }
    }
}