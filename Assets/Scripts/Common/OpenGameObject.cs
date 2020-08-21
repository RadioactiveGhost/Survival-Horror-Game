using UnityEngine;

public class OpenGameObject : MonoBehaviour
{
    public GameObject objectToActivate;

    public void ActivateGameObject()
    {
        objectToActivate.SetActive(true);
    }
}