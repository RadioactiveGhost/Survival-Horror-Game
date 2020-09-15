using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMouseHover : MonoBehaviour
{
    private Image image;
    private void Start()
    {
        image = this.GetComponent<Image>();
        image.enabled = false;
    }

    private void OnMouseOver()
    {
        image.enabled = true;
        Debug.Log("hovering");
    }

    private void OnMouseExit()
    {
        image.enabled = false;
        Debug.Log("hoveringover");
    }


}
