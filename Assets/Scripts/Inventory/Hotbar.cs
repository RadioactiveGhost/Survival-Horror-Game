using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hotbar : MonoBehaviour
{
    GameObject hotbar;
    [HideInInspector]
    public int selected;
    InvSlot[] slots;
    PickUpManager pickUpManager;
    //ItemCount[] items;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Inventory_Test_Scene")
        {
            hotbar = GameObject.FindGameObjectWithTag("Hotbar");
            slots = hotbar.GetComponentsInChildren<InvSlot>();
            selected = 0;
            Select(0);
            //items = new ItemCount[slots.Length];
            UpdateBar();
            pickUpManager = transform.root.GetComponentInChildren<PickUpManager>();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "Game" || SceneManager.GetActiveScene().name == "Inventory_Test_Scene")
        {
            //CHANGE
            UpdateBar();

            //Select item on hotbar
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Select(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Select(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Select(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Select(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Select(4);
            }

            //PickUp/Use Item
            if (Input.GetMouseButtonDown(0))
            {
                pickUpManager.PickUpResource();

                //Change
                //Debug.Log("Use: " + GetItemSelectedName());
            }
        }
    }

    void Select(int selection)
    {
        slots[selected].gameObject.GetComponent<Image>().color = Color.white;
        slots[selection].gameObject.GetComponent<Image>().color = Color.grey;
        selected = selection;
    }

    void UpdateBar()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetItemCount() != null && slots[i].GetItemCount().count != 0)
            {
                SetSlot(slots[i].GetItemCount(), i);
            }
            else
            {
                EmptySlot(i);
            }
        }
    }

    void SetSlot(ItemCount itemCount, int index)
    {
        //Debug.Log("Item " + itemCount.item.name + " on " + index);
        slots[index].SetItem(itemCount);
    }

    void EmptySlot(int index)
    {
        //Debug.Log("Emptying " + index);
        slots[index].EmptyItem();
    }

    public void SetSlotCurrent(ItemCount itemCount)
    {
        SetSlot(itemCount, selected);
    }

    public void EmptySlot(ItemCount itemCount)
    {
        if(itemCount == null)
        {
            Debug.Log("NULL, is slot empty?");
            return;
        }
        for (int i = 0; i < slots.Length; i++)
        {
            //Debug.Log(slots[i].GetItemCount().item.name + " on " + i);
            if (itemCount == slots[i].GetItemCount())
            {
                EmptySlot(i);
                return;
            }
        }
        Debug.LogError("No matches found to remove from hotbar");
    }

    public bool ItemSelectedCheck(string s)
    {
        if(slots[selected].GetItemName() == s)
        {
            return true;
        }
        return false;
    }

    public string GetItemSelectedName()
    {
        return slots[selected].GetItemName();
    }
}