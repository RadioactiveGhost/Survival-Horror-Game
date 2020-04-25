using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
   // public GameObject[] targets;
    public SphereCollider sphereCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        sphereCollider.radius = this.GetComponent<MobBeaviour>().lookradius * 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.GetComponent<MobBeaviour>().IsPrey(other.GetComponent<MobBeaviour>().thisanimal))
        {
            this.GetComponent<MobBeaviour>().target.position = other.gameObject.transform.position;
            
        }
    }

    //void SetTargetList()
    //{
    //    foreach(GameObject gameObject)
    //    {
    //       if (gameObject.CompareTag("Player") || gameObject.CompareTag("Animal"))
    //        {
    //            if (MobBeaviour.IsPrey(gameObject)) //prey???
    //            {
    //                targets[i] =
    //            }


    //        }

    //    }


    //}
}
