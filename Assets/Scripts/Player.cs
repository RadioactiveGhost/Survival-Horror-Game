using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject terrain;
    public float jumpForce, velocity;
    TerrainGenerator terrainScript;
    float myheight, positionY, positionX, positionZ, currentYVel;
    bool grounded, jumping;
    void Start()
    {
        grounded = true;

        myheight = transform.localScale.y * gameObject.GetComponent<CapsuleCollider>().height;
        terrainScript = terrain.GetComponent<TerrainGenerator>();
    }

    public void SetPlayerInitialPos()
    {
        positionX = (terrainScript.mapSizeX * terrainScript.sizeXtile) / 2;
        positionZ = (terrainScript.mapSizeY * terrainScript.sizeZtile) / 2;
        positionY = terrainScript.HeightAt(new Vector2(positionX, positionZ)) + myheight / 2;
        transform.position = new Vector3(positionX, positionY, positionZ);
    }

    void Update()
    {
        Movement();
        Jumping();
        MapFallOffSecurity();
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W))//front
        {
            gameObject.transform.position += gameObject.transform.forward * velocity * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        else if (Input.GetKey(KeyCode.S))//back
        {
            gameObject.transform.position -= gameObject.transform.forward * velocity * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        if (Input.GetKey(KeyCode.A))//left
        {
            gameObject.transform.position -= gameObject.transform.right * velocity * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
        else if (Input.GetKey(KeyCode.D))//right
        {
            gameObject.transform.position += gameObject.transform.right * velocity * Time.deltaTime;
            if (grounded)
            {
                positionY = terrainScript.HeightAt(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, positionY + myheight / 2, gameObject.transform.position.z);
            }
        }
    }

    void Jumping()
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

    void MapFallOffSecurity()
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
        if (collision.gameObject.tag == "Walkable")
        {
            grounded = true;
            jumping = false;
            Debug.Log("Grounded");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Walkable"))
        {
            grounded = false;
            Debug.Log("Took off");
        }
    }
}