using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    public Button b_storage, b_bed, b_crafting;
    public GameObject daySprite, nightSprite, lighternightSprite;
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
        b_storage.onClick.AddListener(OpenStorage);
        b_bed.onClick.AddListener(LongRest);
        b_crafting.onClick.AddListener(OpenCraftin);
    }

    void OpenStorage()
    {

    }

    void OpenCraftin()
    {
       
    }

    void LongRest()
    {
        //SLEEP

    }
}
