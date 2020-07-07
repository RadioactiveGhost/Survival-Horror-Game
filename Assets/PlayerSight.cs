using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    public float fieldOfsight = 110f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Enemy"))
            if(other.gameObject.GetComponent<mobAi>().type == mobAi.Type.lookPhobic)
            {
                Vector3 direction = other.transform.position - transform.position;


                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = other.transform.position - transform.position;

                if (Vector3.Dot(forward, toOther) > 0)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position+transform.up, direction.normalized, out hit))
                    {
                        if (hit.collider.gameObject == other.gameObject)
                            other.gameObject.GetComponent<mobAi>().onPlayerSight = true;
                    }
                    other.gameObject.GetComponent<mobAi>().onPlayerSight = true;
                }
                else
                    other.gameObject.GetComponent<mobAi>().onPlayerSight = false;

                //float angle = Vector3.Angle(direction, transform.forward);

                //if(angle < fieldOfsight * 0.5f)
                //{
                //    RaycastHit hit;
                //    if(Physics.Raycast(transform.position+transform.up, direction.normalized, out hit))
                //    {
                //        if(hit.collider.gameObject == other.gameObject)
                //        {
                //            other.gameObject.GetComponent<mobAi>().onPlayerSight = true;
                //        }
                //        else
                //            other.gameObject.GetComponent<mobAi>().onPlayerSight = false;
                //    }
                //}
            }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            if (other.gameObject.GetComponent<mobAi>().type == mobAi.Type.lookPhobic)
                other.gameObject.GetComponent<mobAi>().onPlayerSight = false;
    }
}
