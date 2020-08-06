using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTest : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.gameObject.GetComponent<Player>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            player.SetGroundBool(false);
            //Debug.Log("Took off");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            player.SetGroundBool(true);
            player.SetJumpingBool(false);
            //Debug.Log("Grounded");
        }
    }
}