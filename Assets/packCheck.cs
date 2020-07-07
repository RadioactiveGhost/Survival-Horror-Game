using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class packCheck : MonoBehaviour
{
    public int packCount = 0;
    public int packTreshHold;
    public bool packFormed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (packCount == packTreshHold)
            packFormed = true;
        else
            packFormed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Enemy"))
        {
            if(other.gameObject.GetComponent<mobAi>().type == mobAi.Type.huntinPack)
            {
                packCount += 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<mobAi>().type == mobAi.Type.huntinPack)
            {
                packCount -= 1;
            }
        }
    }
}
