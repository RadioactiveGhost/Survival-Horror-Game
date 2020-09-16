using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public GameObject player;
    public float distToEnter;
    public GameObject baseMenu, pointer;
    public bool insideBase = false;
    private RaycastHit hit;
    private Camera cameraC;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        baseMenu = GameObject.FindGameObjectWithTag("BaseMenu");
        pointer = GameObject.FindGameObjectWithTag("Pointer");
        baseMenu.GetComponent<BaseMenu>().base1 = this.gameObject;
        baseMenu.SetActive(false);
        cameraC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        if(dist <= distToEnter && !insideBase)
        {
            if(Physics.Raycast(cameraC.transform.position, cameraC.transform.forward, out hit ,5f))
            {
                if(hit.transform.tag == "Base 1")
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        CustomGameManager.pauseIsWorking = false;
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
    }
}
