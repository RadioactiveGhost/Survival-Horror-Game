using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighLight : MonoBehaviour
{
    public GameObject HighLightImage;
    // Start is called before the first frame update
    void Start()
    {
        HighLightImage.GetComponent<Image>().enabled = false;
    }

    public void OnMouseOver()
    {
        HighLightImage.GetComponent<Image>().enabled = true;
    }

    public void OnMouseExit()
    {
        HighLightImage.GetComponent<Image>().enabled = false;
    }
}
