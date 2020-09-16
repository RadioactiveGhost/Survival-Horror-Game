using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscToOpen : MonoBehaviour
{
    public bool pauseMenu;
    public GameObject objectToOpen;

    void Update()
    {

        if(CustomGameManager.pauseIsWorking)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!objectToOpen.activeSelf)
                {
                    objectToOpen.SetActive(true);
                }
            }
        }
        else if(!pauseMenu)
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
}