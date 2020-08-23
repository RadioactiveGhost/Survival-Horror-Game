using UnityEngine;

public class MouseActivation : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}