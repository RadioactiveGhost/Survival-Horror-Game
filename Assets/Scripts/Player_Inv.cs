using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inv: MonoBehaviour
{

    public float speed;

    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        HandleMovement();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleMovement()
    {
        float translation = speed * Time.deltaTime;

        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * translation, 0, Input.GetAxis("Vertical") * translation));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            inventory.Additem(other.GetComponent<Item>());
        }
    }
}
