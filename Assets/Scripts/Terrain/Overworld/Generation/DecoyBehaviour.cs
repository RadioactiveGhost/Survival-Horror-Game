using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyBehaviour : MonoBehaviour
{
    [HideInInspector]
    public GameObject original;

    // Update is called once per frame
    public void CustomUpdate()
    {
        gameObject.SetActive(original.activeSelf);
    }
}