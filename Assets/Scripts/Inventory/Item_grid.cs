using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Item_grid : MonoBehaviour
{
    GameObject InventoryMenu;
    NewInventory inventoryScript;
    GameObject[] slotsparents;
    PickUpManager pickUpManagerScript;

    List<InvSlot> [] childrenSlotsList;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Inventory_Test_Scene")
        {
            slotsparents = GameObject.FindGameObjectsWithTag("SlotsParent");
            InventoryMenu = GameObject.FindGameObjectWithTag("InventoryMenu");
            childrenSlotsList = new List<InvSlot>[slotsparents.Length];
            for (int i = 0; i < childrenSlotsList.Length; i++)
            {
                childrenSlotsList[i] = new List<InvSlot>();
            }
            inventoryScript = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<NewInventory>();
            pickUpManagerScript = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<PickUpManager>();

            for (int i = 0; i < slotsparents.Length; i++)
            {
                foreach (Transform child in slotsparents[i].transform)
                {
                    if (child.GetComponent<InvSlot>())
                    {
                        childrenSlotsList[i].Add(child.GetComponent<InvSlot>());
                    }
                }
            }

            StartCoroutine(NextFrameDelay());
        }
    }

    IEnumerator NextFrameDelay()
    {
        yield return 0;

        for (int i = 0; i < slotsparents.Length; i++)
        {
            slotsparents[i].SetActive(false);
        }

        InventoryMenu.transform.GetChild(0).transform.gameObject.SetActive(true);
        InventoryMenu.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Inventory_Test_Scene")
        {
            //Not efficient CHANGE
            PopulateAll();

            if (CustomGameManager.pauseIsWorking)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    InventoryMenu.SetActive(!InventoryMenu.activeSelf);
                    if (InventoryMenu.activeSelf)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        pickUpManagerScript.CheckPickups = false;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        pickUpManagerScript.CheckPickups = true;
                    }
                }
            }
        }
    }

    void PopulateOnIndex(int index)
    {
        childrenSlotsList[0][index].SetItem(inventoryScript.inventory[index]);
    }

    void PopulateAll()
    {
        for (int i = 0; i < slotsparents.Length; i++)
        {
            for (int j = 0; j < childrenSlotsList[i].Count; j++)
            {
                if (j < inventoryScript.inventory.Count)
                {
                    childrenSlotsList[i][j].SetItem(inventoryScript.inventory[j]);
                }
                else
                {
                    childrenSlotsList[i][j].EmptyItem();
                }
            }
        }
    }
}