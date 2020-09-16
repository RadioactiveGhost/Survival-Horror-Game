using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameObject player;
    public float distToEnter;
    public GameObject baseMenu, pointer;
    public bool insideBase = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        baseMenu = GameObject.FindGameObjectWithTag("BaseMenu");
        pointer = GameObject.FindGameObjectWithTag("Pointer");
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        if(dist <= distToEnter && !insideBase)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                pointer.SetActive(false);
                insideBase = true;
                baseMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }
}
