using UnityEngine;

public class EscToClose : MonoBehaviour
{
    public bool pauseMenu;
    void Update()
    {
        if(CustomGameManager.pauseIsWorking)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }
        else if(!pauseMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameObject.SetActive(false);
            }
        }
       
    }
}