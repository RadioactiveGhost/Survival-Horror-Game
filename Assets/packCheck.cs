using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class packCheck : MonoBehaviour
{
    public int packCount = 0;
    public int packTreshHold;
    public bool packFormed = false;
    public List<GameObject> samePackGuys;
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
       if (other.CompareTag("Enemy"))
            if (other.gameObject.GetComponent<mobAi>().type == mobAi.Type.huntinPack)
                {
                if (!samePackGuys.Contains(other.gameObject))
                {
                    samePackGuys.Add(other.gameObject);
                    packCount = samePackGuys.Count;
                    if (samePackGuys.Count >= packTreshHold)
                        packFormed = true;
                    else
                        packFormed = false;
                }
            }
    }
        
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            if (other.gameObject.GetComponent<mobAi>().type == mobAi.Type.huntinPack)
            {
                if (samePackGuys.Contains(other.gameObject))
                {
                    samePackGuys.Remove(other.gameObject);
                    packCount = samePackGuys.Count;
                    if (samePackGuys.Count >= packTreshHold)
                        packFormed = true;
                    else
                        packFormed = false;
                }
            }
    }
}
