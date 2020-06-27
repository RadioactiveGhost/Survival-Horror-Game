using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeHandler : MonoBehaviour
{
    public bool playercheck = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playercheck = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playercheck = false;
    }
}
