using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [Header("List of items to be obtained from this resource")]
    [Tooltip("Only name and number is needed, it will search name on item list and grab sprite")]
    public List<ItemCount> itemsToPickUp;
    public item requiredTool;

    public List<ItemCount> AttemptToPickUp(string s)
    {
        if (requiredTool.name == "" || (s != null && s == requiredTool.name))
        {
            //Debug.Log("Picking " + itemsToPickUp);
            return itemsToPickUp;
        }
        else
        {
            Debug.Log("Isn't using required tool " + requiredTool.name);
            return null;
        }
    }
}