using UnityEngine;

public class Grappling_hook : MonoBehaviour
{
    public float grappleSpeed, stopDist;
    public LayerMask mask;
    bool grappling, grappled;
    Vector3 helperVec;
    public GameObject cameraRef, hookTip, player;
    LineRenderer lr;
    Rigidbody rb;

    private void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
        grappling = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !grappling) //Shoot grappling
        {
            //Debug.Log("Shooting");
            Fire();
        }
        else if(Input.GetKeyDown(KeyCode.R) && grappling) //Release mid grappling
        {
            //Debug.Log("Release mid grapple");
            Release();
        }
        else if(Input.GetKeyDown(KeyCode.R) && grappled) //Release from wall
        {
            //Debug.Log("Release from wall");
            Release();
        }

        if(grappled && player.GetComponent<Player>().grounded) //Release if grounded
        {
            //Debug.Log("Release since touching ground");
            Release();
        }

        if (grappling)
        {
            //Debug.Log("Grappling...");
            Grapple(helperVec);
        }
    }

    void Fire()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraRef.transform.position, cameraRef.transform.forward, out hit, 20.0f, mask))
        {
            //Debug.DrawRay(transform.GetChild(0).position, transform.GetChild(0).transform.forward * hit.distance, Color.yellow);
            //Debug.Log("Did Hit on " + hit.collider.gameObject.name);
            grappling = true;
            lr.enabled = true;
            helperVec = hit.point;
            DisableGravity();
            player.GetComponent<Player>().movementAllowed = false;
        }
    }

    void Grapple(Vector3 destination)
    {
        if (Vector3.Distance(transform.position, destination) > stopDist)
        {
            player.transform.position += (destination - (player.transform.position)).normalized * Time.deltaTime * grappleSpeed;

            lr.SetPosition(0, hookTip.transform.position);
            lr.SetPosition(1, destination);
        }
        else
        {
            StopGrapple();
        }
    }

    void StopGrapple()
    {
        grappling = false;
        lr.enabled = false;
        grappled = true;
    }

    void Release()
    {
        grappled = false;
        grappling = false;
        lr.enabled = false;
        EnableGravity();
        player.GetComponent<Player>().movementAllowed = true;
    }

    void DisableGravity()
    {
        //Debug.Log("Disabling gravity");
        //rb.detectCollisions = false;
        rb.useGravity = false;
    }

    void EnableGravity()
    {
        //Debug.Log("Enabling gravity");
        //rb.detectCollisions = true;
        rb.useGravity = true;
    }
}