using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseMenu : MonoBehaviour
{
    public Button b_storage, b_bed, b_crafting, b_closeC, b_closeS, b_door;
    public GameObject daySprite, nightSprite, lighternightSprite, craftingMenu, storageMenu;
    public bool day;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(day)
        {
            daySprite.SetActive(true);
            nightSprite.SetActive(false);
        }
        else
        {
            daySprite.SetActive(false);
            nightSprite.SetActive(true);
        }
        b_door.onClick.AddListener(Leave);
        b_storage.onClick.AddListener(OpenStorage);
        b_bed.onClick.AddListener(LongRest);
        b_crafting.onClick.AddListener(OpenCraftin);
        b_closeC.onClick.AddListener(CloseCraftin);
        b_closeS.onClick.AddListener(CloseStorage);
    }

    private void Leave()
    {
        SceneManager.LoadScene("Game");
    }

    private void CloseCraftin()
    {
        craftingMenu.SetActive(false);
    }

    private void CloseStorage()
    {
        storageMenu.SetActive(false);
    }

    void OpenStorage()
    {
        storageMenu.SetActive(true);
    }

    void OpenCraftin()
    {
        craftingMenu.SetActive(true);
    }

    void LongRest()
    {
        //SLEEP

    }
}
