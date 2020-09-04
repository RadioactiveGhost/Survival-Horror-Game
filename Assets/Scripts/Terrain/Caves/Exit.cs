using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ManagerScenes.LoadGame();
        }
    }
}