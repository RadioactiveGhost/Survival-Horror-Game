using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public float jumpForce, moveForce, maxVelocity;
    float myheight, positionY, positionX, positionZ, currentYVel;
    bool jumping, moving;
    [HideInInspector]
    public bool grounded, movementAllowed;

    //Stats
    Common common;
    public int hunger, thirst, health, armor, strength;
    public float hungerTimer, thirstTimer;
    float currentHungerTimer, currentThirstTimer;
    Rigidbody rb;

    private void Awake()
    {
        movementAllowed = true;
        grounded = true;
        myheight = transform.localScale.y * gameObject.GetComponent<CapsuleCollider>().height;
    }

    void Start()
    {
        currentHungerTimer = Time.time;
        currentThirstTimer = Time.time;
        common = new Common(health, strength, armor);

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() //Physics stuff
    {
        if (movementAllowed)
        {
            RBMovement();
            Jumping();
        }
    }

    void Update()
    {
        ManageThirstHunger();

        if(common.dead)
        {
            Die();
        }
    }

    public void ManageThirstHunger()
    {
        currentHungerTimer += Time.deltaTime;

        if (currentHungerTimer > hungerTimer)
        {
            currentHungerTimer = 0;
            hunger -= 1;
            if (hunger <= 0)
            {
                hunger = 0;
                common.TakeTrueDamage(1, gameObject, false);
            }
        }

        currentThirstTimer += Time.deltaTime;

        if (currentThirstTimer > thirstTimer)
        {
            currentThirstTimer = 0;
            thirst -= 1;
            if (thirst <= 0)
            {
                thirst = 0;
                common.TakeTrueDamage(1, gameObject, false);
            }
        }
    }

    public void SetPlayerInitialPos(TerrainGenerator terrainScript)
    {
        positionX = (terrainScript.mapSizeX * terrainScript.sizeXtile) / 2;
        positionZ = (terrainScript.mapSizeY * terrainScript.sizeZtile) / 2;
        positionY = terrainScript.HeightAt(new Vector2(positionX, positionZ)) + myheight / 2;
        transform.position = new Vector3(positionX, positionY, positionZ);
    }

    public void Die()
    {
        //Only stops things for now
        Time.timeScale = 0;
    }

    void TerrainCustomMovement(TerrainGenerator terrainScript)
    {
        if (Input.GetKey(KeyCode.W))//front
        {
            gameObject.transform.position += gameObject.transform.forward * moveForce * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        else if (Input.GetKey(KeyCode.S))//back
        {
            gameObject.transform.position -= gameObject.transform.forward * moveForce * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        if (Input.GetKey(KeyCode.A))//left
        {
            gameObject.transform.position -= gameObject.transform.right * moveForce * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        else if (Input.GetKey(KeyCode.D))//right
        {
            gameObject.transform.position += gameObject.transform.right * moveForce * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
    }

    void RBMovement()
    {
        //Extra Gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        if (!jumping)//set up movement check
        {
            moving = false; 
        }

        if (grounded)
        {
            if (Input.GetKey(KeyCode.W))//front
            {
                rb.AddForce(gameObject.transform.forward * moveForce * Time.deltaTime);
                //Debug.Log("Forward");
                moving = true;
            }
            else if (Input.GetKey(KeyCode.S))//back
            {
                rb.AddForce(-gameObject.transform.forward * moveForce * Time.deltaTime);
                //Debug.Log("Back");
                moving = true;
            }

            if (Input.GetKey(KeyCode.A))//left
            {
                rb.AddForce(-gameObject.transform.right * moveForce * Time.deltaTime);
                //Debug.Log("Left");
                moving = true;
            }
            else if (Input.GetKey(KeyCode.D))//right
            {
                rb.AddForce(gameObject.transform.right * moveForce * Time.deltaTime);
                //Debug.Log("Right");
                moving = true;
            }
        }

        if(!moving && grounded) //not moving
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            //Debug.Log("Stopped");
        }

        if (new Vector2(rb.velocity.x, rb.velocity.z).magnitude > maxVelocity)//velocity check (including diagonals) horizontal movement
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized * maxVelocity + new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded && !jumping)
            {
                rb.AddForce(Vector3.up * jumpForce * Time.deltaTime * 10);
                jumping = true;
                //Debug.Log("Jumped");
                moving = true;
            }
        }
    }

    void TerrainCustomJumping(TerrainGenerator terrainScript)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                Debug.Log("Jumping");
                currentYVel = jumpForce;
                transform.position += Vector3.up * Time.deltaTime * jumpForce;
                jumping = true;
            }
        }
        if (jumping)
        {
            currentYVel -= jumpForce * 0.1f;
            if (currentYVel > 0)
            {
                Debug.Log("Rising");
            }
            else
            {
                Debug.Log("Falling");
            }
            positionY = terrainScript.HeightAt(new Vector2(transform.position.x, transform.position.z)) + myheight / 2;
            if ((transform.position + (Vector3.up * Time.deltaTime * currentYVel)).y >= positionY)//is on or above terrain
            {
                transform.position += Vector3.up * Time.deltaTime * currentYVel;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
            }
        }
    }

    void MapFallOffSecurity(TerrainGenerator terrainScript)
    {
        positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
        positionY += myheight / 2;
        if (transform.position.y < positionY)//fell under map
        {
            transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
            Debug.LogError("Was falling off map on: " + transform.position);
        }

        //BorderLimit
        float minZ = 0;
        float maxZ = terrainScript.sizeZtile * terrainScript.mapSizeY;
        float minX = 0;
        float maxX = terrainScript.sizeXtile * terrainScript.mapSizeX;

        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
        if (transform.position.z < minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ);
        }
        else if (transform.position.z > maxZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
            jumping = false;
            //Debug.Log("Grounded");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            grounded = false;
            //Debug.Log("Took off");
        }
    }
}