using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscToOpen : MonoBehaviour
{
    public GameObject objectToOpen;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!objectToOpen.activeSelf)
            {
                objectToOpen.SetActive(true);
            }
        }
    }
}